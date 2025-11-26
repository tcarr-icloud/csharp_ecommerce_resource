namespace csharp_ecommerce_resource.Orders;

public class OrderDto
{
    public string? Id { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? AccountId { get; set; }
    public string? CartId { get; set; }
    public string? Status { get; set; }
    public bool? Active { get; set; }
}