namespace csharp_ecommerce_resource.Items;

public class ItemDto
{
    public string? Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Variant { get; set; }
    public string? Uom { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public string? Active { get; set; }
}