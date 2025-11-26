using csharp_ecommerce_resource.Services;

namespace csharp_ecommerce_resource.Accounts;

public interface IAccountService
{
    AccountDto AddAccount(AccountDto accountDto);
    AccountDto UpdateAccount(string id, AccountDto accountDto, string action = "UpdateAccount");
    AccountDto GetAccount(string id);
    void DeleteAccount(string id);
    List<AccountDto> GetAllAccounts();
}

public class AccountService(
    IDynamodbService dynamodbService,
    IKafkaAccountsProducerService kafkaAccountsProducerService) : IAccountService
{
    public AccountDto AddAccount(AccountDto accountDto)
    {
        if (accountDto.Id != null) throw new Exception("AccountDto ID cannot be set manually.");
        accountDto.Id = Guid.NewGuid().ToString();

        if (accountDto.Timestamp != null) throw new Exception("AccountDto timestamp cannot be set manually.");
        accountDto.Timestamp = DateTime.UtcNow;

        dynamodbService.AddAccountAsync(accountDto);
        kafkaAccountsProducerService.SendAccountEvent("AddAccount", accountDto);

        return accountDto;
    }

    public AccountDto GetAccount(string id)
    {
        var account = new AccountDto();
        dynamodbService.GetAccount(id).ForEach(attributeValues =>
        {
            account.Id = attributeValues["Id"].S;
            account.Timestamp = DateTime.Parse(attributeValues["Timestamp"].S);
            account.AccountType = attributeValues["AccountType"].S;
            account.CompanyName = attributeValues["CompanyName"].S;
            account.FirstName = attributeValues["FirstName"].S;
            account.LastName = attributeValues["LastName"].S;
            account.PhoneNumber = attributeValues["PhoneNumber"].S;
            account.Email = attributeValues["Email"].S;
            account.Active = attributeValues["Active"].BOOL.ToString();
        });
        return account;
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
        kafkaAccountsProducerService.SendAccountEvent(action, accountDto);

        return accountDto;
    }

    public void DeleteAccount(string id)
    {
        var accountDto = GetAccount(id);
        accountDto.Active = false.ToString();
        accountDto.Timestamp = null;
        UpdateAccount(id, accountDto, "DeleteAccount");
    }
}