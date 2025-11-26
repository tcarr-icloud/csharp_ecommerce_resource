namespace csharp_ecommerce_resource.Items;

public class ItemMessage(string action, ItemDto itemDto)
{
    public string Action { get; init; } = action;
    public ItemDto ItemDto { get; init; } = itemDto;
}