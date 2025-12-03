using csharp_ecommerce_resource.Services;

namespace csharp_ecommerce_resource.Accounts;

public interface IAccountService
{
    AccountDto CreateAccount(AccountDto accountDto, string action = "CreateAccount");
    AccountDto GetAccount(string id, string action = "GetAccount");
    AccountDto[] GetAllAccounts(string action = "GetAllAccounts");
    AccountDto UpdateAccount(string id, AccountDto accountDto, string action = "UpdateAccount");
    void DeleteAccount(string id, string action = "DeleteAccount");
}

public class AccountService(
    IDynamodbService dynamodbService,
    IKafkaProducerService kafkaProducerService) : IAccountService
{
    public AccountDto CreateAccount(AccountDto accountDto, string action = "CreateAccount")
    {
        if (accountDto.Id != null) throw new Exception("AccountDto ID cannot be set manually.");
        accountDto.Id = Guid.NewGuid().ToString();

        if (accountDto.Timestamp != null) throw new Exception("AccountDto timestamp cannot be set manually.");
        accountDto.Timestamp = DateTime.UtcNow.ToBinary();

        if (accountDto.Email == null) throw new Exception("AccountDto email cannot be null.");

        var ids = dynamodbService.GetAccountByEmail(accountDto.Email);
        if (ids.Count > 0) throw new Exception("Account with email already exists.");

        dynamodbService.AddAccountAsync(accountDto);
        kafkaProducerService.SendAccountEvent(action, accountDto);

        return accountDto;
    }

    public AccountDto GetAccount(string id, string action = "GetAccount")
    {
        var accountDto = new AccountDto();
        dynamodbService.GetEvents("accounts", id).ForEach(attributeValues =>
        {
            accountDto.Id = attributeValues["Id"].S;
            accountDto.Timestamp = long.Parse(attributeValues["Timestamp"].N);
            accountDto.CompanyName = attributeValues["CompanyName"].S;
            accountDto.FirstName = attributeValues["FirstName"].S;
            accountDto.LastName = attributeValues["LastName"].S;
            accountDto.PhoneNumber = attributeValues["PhoneNumber"].S;
            accountDto.Email = attributeValues["Email"].S;
            accountDto.Type = attributeValues["Type"].S;
            accountDto.Active = attributeValues["Active"].BOOL;
        });
        return accountDto;
    }

    public AccountDto[] GetAllAccounts(string action = "GetAllAccounts")
    {
        throw new NotImplementedException();
    }

    public AccountDto UpdateAccount(string id, AccountDto accountDto, string action = "UpdateAccount")
    {
        if (accountDto.Id == null) throw new Exception("AccountDto ID cannot be null.");
        var existingAccount = GetAccount(id);
        if (accountDto.Email != null && accountDto.Email != existingAccount.Email)
            if (dynamodbService.GetAccountByEmail(accountDto.Email).Count > 0)
                throw new Exception("Account with email already exists.");

        if (accountDto.Timestamp != null) throw new Exception("AccountDto timestamp cannot be set manually.");
        accountDto.Timestamp = DateTime.UtcNow.ToBinary();
        
        dynamodbService.AddAccountAsync(accountDto);
        kafkaProducerService.SendAccountEvent(action, accountDto);

        return accountDto;
    }

    public void DeleteAccount(string id, string action = "DeleteAccount")
    {
        dynamodbService.GetEvents("accounts", id).ForEach(attributeValues =>
        {
            dynamodbService.DeleteItem(
                "accounts", 
                attributeValues["Id"].S, 
                attributeValues["Timestamp"].S);
        });
    }
}