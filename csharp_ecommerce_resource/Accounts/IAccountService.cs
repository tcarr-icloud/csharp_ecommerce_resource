using csharp_ecommerce_resource.Services;

namespace csharp_ecommerce_resource.Accounts;

public interface IAccountService
{
    AccountDto AddAccount(AccountDto accountDto, string action = "AddAccount");
    AccountDto UpdateAccount(string id, AccountDto accountDto, string action = "UpdateAccount");
    AccountDto GetAccount(string id);
    void DeleteAccount(string id, string action = "DeleteAccount");
    List<AccountDto> GetAllAccounts();
}

public class AccountService(
    IDynamodbService dynamodbService,
    IKafkaProducerService kafkaProducerService) : IAccountService
{
    public AccountDto AddAccount(AccountDto accountDto, string action = "AddAccount")
    {
        if (accountDto.Id != null) throw new Exception("AccountDto ID cannot be set manually.");
        accountDto.Id = Guid.NewGuid().ToString();

        if (accountDto.Timestamp != null) throw new Exception("AccountDto timestamp cannot be set manually.");
        accountDto.Timestamp = DateTime.UtcNow;

        dynamodbService.AddAccountAsync(accountDto);
        kafkaProducerService.SendAccountEvent(action, accountDto);

        return accountDto;
    }

    public AccountDto GetAccount(string id)
    {
        var accountDto = new AccountDto();
        dynamodbService.GetEvents("accounts", id).ForEach(attributeValues =>
        {
            accountDto.Id = attributeValues["Id"].S;
            accountDto.Timestamp = DateTime.Parse(attributeValues["Timestamp"].S);
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

    public List<AccountDto> GetAllAccounts()
    {
        throw new NotImplementedException();
    }

    public AccountDto UpdateAccount(string id, AccountDto accountDto, string action = "UpdateAccount")
    {
        if (accountDto.Id == null) throw new Exception("AccountDto ID cannot be null.");
        if (accountDto.Timestamp != null) throw new Exception("AccountDto timestamp cannot be set manually.");
        accountDto.Timestamp = DateTime.UtcNow;

        dynamodbService.AddAccountAsync(accountDto);
        kafkaProducerService.SendAccountEvent(action, accountDto);

        return accountDto;
    }

    public void DeleteAccount(string id, string action = "DeleteAccount")
    {
        var accountDto = GetAccount(id);
        accountDto.Active = false;
        accountDto.Timestamp = null;
        UpdateAccount(id, accountDto, action);
    }
}