using System.Text.Json;
using csharp_ecommerce_resource.Items;
using csharp_ecommerce_resource.Services;

namespace csharp_ecommerce_resource.Carts;

public interface ICartService
{
    CartDto AddCart(CartDto cartDto, string action = "AddCart");
    CartDto GetCart(string id, string action = "GetCart");
    List<string> GetAllCarts(string action = "GetAllCarts");
    CartDto UpdateCart(string id, CartDto cartDto, string action = "UpdateCart");
    void DeleteCart(string id, string action = "DeleteCart");
}

public class CartService(
    IDynamodbService dynamodbService,
    IKafkaProducerService kafkaProducerService) : ICartService
{
    public CartDto AddCart(CartDto cartDto, string action = "AddCart")
    {
        if (cartDto.Id != null) throw new Exception("CartDto ID cannot be set manually.");
        cartDto.Id = Guid.NewGuid().ToString();

        if (cartDto.Timestamp != null) throw new Exception("CartDto timestamp cannot be set manually.");
        cartDto.Timestamp = DateTime.UtcNow.ToBinary();

        dynamodbService.AddCartAsync(cartDto);
        kafkaProducerService.SendCartEvent(action, cartDto);

        return cartDto;
    }

    public CartDto GetCart(string id, string action = "GetCart")
    {
        var cartDto = new CartDto();
        dynamodbService.GetEvents("carts", id).ForEach(attributeValues =>
        {
            cartDto.Id = attributeValues["Id"].S;
            cartDto.Timestamp = long.Parse(attributeValues["Timestamp"].S);
            cartDto.AccountId = attributeValues["AccountId"].NULL == true
                ? null
                : attributeValues["AccountId"].S;
            cartDto.Items = attributeValues["Items"].NULL == true
                ? null
                : JsonSerializer.Deserialize<List<ItemDto>>(attributeValues["Items"].S);
            cartDto.Active = attributeValues["Active"].BOOL;
        });
        return cartDto;
    }

    public List<string> GetAllCarts(string action = "GetAllCarts")
    {
        var list = new List<string>();
        var keys = dynamodbService.GetKeys("carts", action);
        foreach (var attributeValues in keys)
        {
            var key = attributeValues["Id"].S;
            list.Add(key);
        }

        return list;
    }

    public CartDto UpdateCart(string id, CartDto cartDto, string action = "UpdateCart")
    {
        if (cartDto.Id == null) throw new Exception("CartDto ID cannot be null.");
        if (cartDto.Timestamp != null) throw new Exception("CartDto timestamp cannot be set manually.");
        cartDto.Timestamp = DateTime.UtcNow.ToBinary();

        dynamodbService.AddCartAsync(cartDto);
        kafkaProducerService.SendCartEvent(action, cartDto);

        return cartDto;
    }

    public void DeleteCart(string id, string action = "DeleteCart")
    {
        dynamodbService.GetEvents("carts", id).ForEach(attributeValues =>
        {
            dynamodbService.DeleteItem("carts", attributeValues["Id"].S, attributeValues["Timestamp"].S);
        });
    }
}