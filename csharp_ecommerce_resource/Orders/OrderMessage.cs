namespace csharp_ecommerce_resource.Orders;

public class OrderMessage(string action, OrderDto orderDto)
{
    public string Action { get; init; } = action;
    public OrderDto OrderDto { get; init; } = orderDto;
}