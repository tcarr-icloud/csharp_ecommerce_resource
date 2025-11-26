namespace csharp_ecommerce_resource.Accounts;

public class AccountMessage
{
    public AccountMessage(string action, AccountDto accountDto)
    {
        Action = action;
        AccountDto = accountDto;
    }

    public AccountMessage()
    {
    }

    public string? Action { get; set; }
    public AccountDto? AccountDto { get; set; }
}