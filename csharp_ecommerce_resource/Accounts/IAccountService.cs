using Amazon.DynamoDBv2.Model;
using csharp_ecommerce_resource.Services;

namespace csharp_ecommerce_resource.Accounts;

public interface IAccountService
{
    AccountDto Create(AccountDto accountDto, string action = "CreateAccount");
    AccountDto Get(string id, string action = "GetAccount");
    List<string> GetAll(string action = "GetAllAccounts");
    AccountDto Update(string id, AccountDto accountDto, string action = "UpdateAccount");
    void Delete(string id, string action = "DeleteAccount");
}

public class AccountService(
    IDynamodbService dynamodbService,
    IKafkaProducerService kafkaProducerService) : IAccountService
{
    public AccountDto Create(AccountDto accountDto, string action = "CreateAccount")
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

    public AccountDto Get(string id, string action = "GetAccount")
    {
        var accountDto = new AccountDto();
        dynamodbService.GetEvents("accounts", id).ForEach(attributeValues =>
        {
            accountDto.Id = attributeValues["Id"].S;
            accountDto.Timestamp = long.Parse(attributeValues["Timestamp"].S);
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

    public List<string> GetAll(string action = "GetAllAccounts")
    {
        var list = new List<string>();
        var keys = dynamodbService.GetKeys("accounts", action);
        foreach (var attributeValues in keys)
        {
            var key = attributeValues["Id"].S;
            list.Add(key);
        }

        return list;
    }

    public AccountDto Update(string id, AccountDto accountDto, string action = "UpdateAccount")
    {
        if (accountDto.Id == null) throw new Exception("AccountDto ID cannot be null.");
        var existingAccount = Get(id);
        if (accountDto.Email != null && accountDto.Email != existingAccount.Email)
            if (dynamodbService.GetAccountByEmail(accountDto.Email).Count > 0)
                throw new Exception("Account with email already exists.");

        if (accountDto.Timestamp != null) throw new Exception("AccountDto timestamp cannot be set manually.");
        accountDto.Timestamp = DateTime.UtcNow.ToBinary();
        
        dynamodbService.AddAccountAsync(accountDto);
        kafkaProducerService.SendAccountEvent(action, accountDto);

        return accountDto;
    }

    public void Delete(string id, string action = "DeleteAccount")
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