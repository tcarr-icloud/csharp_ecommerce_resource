namespace csharp_ecommerce_resource.Models;

public class AccountEvent
{
    public string? Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Action { get; set; }
    private HashSet<PropertyData>? Properties { get; set; }
}