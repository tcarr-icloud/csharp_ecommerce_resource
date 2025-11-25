using System.Text.Json;
using csharp_ecommerce_resource.Models;

namespace csharp_ecommerce_resource.Services;

public class AccountProcessor : IProcessor<AccountMessage>
{
    public void HandleMessage(string message)
    {
        var accountMessage = JsonSerializer.Deserialize<AccountMessage>(message);
        Console.WriteLine("HandleMessage: " + accountMessage?.Account?.Id);
    }
}