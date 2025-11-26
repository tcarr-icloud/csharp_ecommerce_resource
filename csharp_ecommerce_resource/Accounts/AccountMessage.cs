namespace csharp_ecommerce_resource.Accounts;

public class AccountMessage(string action, AccountDto accountDto)
{
    public string Action { get; init; } = action;
    public AccountDto AccountDto { get; init; } = accountDto;
}