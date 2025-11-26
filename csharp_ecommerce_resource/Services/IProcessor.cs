using System.Text.Json;
using csharp_ecommerce_resource.Accounts;

namespace csharp_ecommerce_resource.Services;

public interface IProcessor<T>
{
    void HandleMessage(string message);
}

public class AccountProcessor : IProcessor<AccountMessage>
{
    public void HandleMessage(string message)
    {
        var accountMessage = JsonSerializer.Deserialize<AccountMessage>(message);
        Console.WriteLine("HandleMessage: " + accountMessage?.AccountDto?.Id);
    }
}