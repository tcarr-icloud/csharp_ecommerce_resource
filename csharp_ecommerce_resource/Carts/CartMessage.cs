namespace csharp_ecommerce_resource.Carts;

public class CartMessage(string action, CartDto cartDto)
{
    public string Action { get; init; } = action;
    public CartDto CartDto { get; init; } = cartDto;
}