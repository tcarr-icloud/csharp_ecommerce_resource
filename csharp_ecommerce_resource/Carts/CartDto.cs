using System.Text.Json.Serialization;
using csharp_ecommerce_resource.Items;

namespace csharp_ecommerce_resource.Carts;

public class CartDto
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("timestamp")] public long? Timestamp { get; set; }
    [JsonPropertyName("accountId")] public string? AccountId { get; set; }
    [JsonPropertyName("items")] public List<ItemDto>? Items { get; set; }
    [JsonPropertyName("active")] public bool? Active { get; set; }
}