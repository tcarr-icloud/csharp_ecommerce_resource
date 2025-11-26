namespace csharp_ecommerce_resource.Models;

public class Cart
{
    public string? Id { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? AccountId { get; set; }
    public List<Item>? Items { get; set; }
}