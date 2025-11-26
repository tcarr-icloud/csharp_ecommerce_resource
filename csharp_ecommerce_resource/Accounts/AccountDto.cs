namespace csharp_ecommerce_resource.Accounts;

public class AccountDto
{
    public enum AccountTypes
    {
        Personal,
        Business
    }

    public string? Id { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? CompanyName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Type { get; set; }
    public bool? Active { get; set; }
}
