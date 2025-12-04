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

    [JsonPropertyName("companyname")] public string? CompanyName { get; set; }

    [JsonPropertyName("firstname")] public string? FirstName { get; set; }

    [JsonPropertyName("lastname")] public string? LastName { get; set; }

    [JsonPropertyName("phonenumber")] public string? PhoneNumber { get; set; }

    [JsonPropertyName("email")] public string? Email { get; set; }

    [JsonPropertyName("type")] public string? Type { get; set; }

    [JsonPropertyName("active")] public bool? Active { get; set; }
}