using System.Text.Json.Serialization;

namespace csharp_ecommerce_resource.Orders;

public class OrderDto
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("timestamp")] public long? Timestamp { get; set; }
    [JsonPropertyName("accountId")] public string? AccountId { get; set; }
    [JsonPropertyName("cartId")] public string? CartId { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("active")] public bool? Active { get; set; }
}