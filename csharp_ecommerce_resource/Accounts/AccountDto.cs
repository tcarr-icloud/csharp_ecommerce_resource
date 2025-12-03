using System.Text.Json.Serialization;

namespace csharp_ecommerce_resource.Accounts;

public class AccountDto
{
    public enum AccountTypes
    {
        Personal,
        Business
    }

    [JsonPropertyName("id")] public string? Id { get; set; }

    [JsonPropertyName("timestamp")] public long? Timestamp { get; set; }

    [JsonPropertyName("company_name")] public string? CompanyName { get; set; }

    [JsonPropertyName("first_name")] public string? FirstName { get; set; }

    [JsonPropertyName("last_name")] public string? LastName { get; set; }

    [JsonPropertyName("phone_number")] public string? PhoneNumber { get; set; }

    [JsonPropertyName("email")] public string? Email { get; set; }

    [JsonPropertyName("type")] public string? Type { get; set; }

    [JsonPropertyName("active")] public bool? Active { get; set; }
}