using csharp_ecommerce_resource.Models;

namespace csharp_ecommerce_resource.Services;

public interface IAccountService
{
    Account? AddAccount(Account account);
    Account UpdateAccount(string id, string type);
    Account GetAccount(string id);
    void DeleteAccount(string id);
    List<Account> GetAllAccounts();
}

public class AccountService(
    IDynamodbService dynamodbService,
    IKafkaAccountsProducerService kafkaAccountsProducerService) : IAccountService
{
    public Account? AddAccount(Account account)
    {
        try
        {
            if (account.Id != null) throw new Exception("Account ID cannot be set manually.");
            account.Id = Guid.NewGuid().ToString();

            if (account.Timestamp != null) throw new Exception("Account timestamp cannot be set manually.");
            account.Timestamp = DateTime.UtcNow;

            dynamodbService.AddAccountAsync(account);
            kafkaAccountsProducerService.SendAccountEvent("AddAccount", account);

            return account;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public Account UpdateAccount(string id, string type)
    {
        throw new NotImplementedException();
    }

    public Account GetAccount(string id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAccount(string id)
    {
        throw new NotImplementedException();
    }

    public List<Account> GetAllAccounts()
    {
        throw new NotImplementedException();
    }
}