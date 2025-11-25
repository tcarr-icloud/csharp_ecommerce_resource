namespace csharp_ecommerce_resource.Models;

public class AccountMessage
{
    public AccountMessage(string action, Account account)
    {
        Action = action;
        Account = account;
    }

    public AccountMessage()
    {
    }

    public string? Action { get; set; }
    public Account? Account { get; set; }
}