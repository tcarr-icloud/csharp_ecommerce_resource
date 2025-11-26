using System.Text.Json;
using csharp_ecommerce_resource.Items;
using csharp_ecommerce_resource.Services;

namespace csharp_ecommerce_resource.Carts;

public interface ICartService
{
    Task<CartDto> AddCart(CartDto cartDto, string action = "AddCart");
    Task<CartDto> GetCart(string id);
    Task<CartDto[]> GetAllCarts();
    Task<CartDto> UpdateCart(string id, CartDto cartDto, string action = "UpdateCart");
    Task DeleteCart(string id, string action = "DeleteCart");
}

public class CartService(
    IDynamodbService dynamodbService,
    IKafkaProducerService kafkaProducerService) : ICartService
{
    public Task<CartDto> AddCart(CartDto cartDto, string action = "AddCart")
    {
        if (cartDto.Id != null) throw new Exception("CartDto ID cannot be set manually.");
        cartDto.Id = Guid.NewGuid().ToString();

        if (cartDto.Timestamp != null) throw new Exception("CartDto timestamp cannot be set manually.");
        cartDto.Timestamp = DateTime.UtcNow;

        dynamodbService.AddCartAsync(cartDto);
        kafkaProducerService.SendCartEvent(action, cartDto);

        return Task.FromResult(cartDto);
    }

    public Task<CartDto> GetCart(string id)
    {
        var cartDto = new CartDto();
        dynamodbService.GetEvents("carts", id).ForEach(attributeValues =>
        {
            cartDto.Id = attributeValues["Id"].S;
            cartDto.Timestamp = DateTime.Parse(attributeValues["Timestamp"].S);
            cartDto.AccountId = attributeValues["AccountId"].NULL == true
                ? null
                : attributeValues["AccountId"].S;
            cartDto.Items = attributeValues["Items"].NULL == true
                ? null
                : JsonSerializer.Deserialize<List<ItemDto>>(attributeValues["Items"].S);
            cartDto.Active = attributeValues["Active"].BOOL;
        });
        return Task.FromResult(cartDto);
    }

    public Task<CartDto[]> GetAllCarts()
    {
        return Task.FromResult(Array.Empty<CartDto>());
    }

    public Task<CartDto> UpdateCart(string id, CartDto cartDto, string action = "UpdateCart")
    {
        if (cartDto.Id == null) throw new Exception("CartDto ID cannot be null.");
        if (cartDto.Timestamp != null) throw new Exception("CartDto timestamp cannot be set manually.");
        cartDto.Timestamp = DateTime.UtcNow;

        dynamodbService.AddCartAsync(cartDto);
        kafkaProducerService.SendCartEvent(action, cartDto);

        return Task.FromResult(cartDto);
    }

    public Task DeleteCart(string id, string action = "DeleteCart")
    {
        var cartDto = GetCart(id).Result;
        cartDto.Active = false;
        cartDto.Timestamp = null;
        UpdateCart(id, cartDto, action);
        return Task.CompletedTask;
    }
}