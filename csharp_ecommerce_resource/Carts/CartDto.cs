using csharp_ecommerce_resource.Items;

namespace csharp_ecommerce_resource.Carts;

public class CartDto
{
    public string? Id { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? AccountId { get; set; }
    public List<ItemDto>? Items { get; set; }
    public bool? Active { get; set; }
}