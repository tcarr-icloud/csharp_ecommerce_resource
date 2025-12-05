using csharp_ecommerce_resource.Services;

namespace csharp_ecommerce_resource.Orders;

public interface IOrderService
{
    OrderDto Create(OrderDto orderDto, string action = "CreateOrder");
    OrderDto Get(string id, string action = "GetOrder");
    List<string> GetAll(string action = "GetAllOrders");
    OrderDto Update(string id, OrderDto orderDto, string action = "UpdateOrder");
    void Delete(string id, string action = "DeleteOrder");
}

public class OrderService(
    IDynamodbService dynamodbService,
    IKafkaProducerService kafkaProducerService) : IOrderService
{
    public OrderDto Create(OrderDto orderDto, string action = "CreateOrder")
    {
        if (orderDto.Id != null) throw new Exception("OrderDto ID cannot be set manually.");
        orderDto.Id = Guid.NewGuid().ToString();

        if (orderDto.Timestamp != null) throw new Exception("OrderDto timestamp cannot be set manually.");
        orderDto.Timestamp = DateTime.UtcNow.ToBinary();

        dynamodbService.AddOrderAsync(orderDto);
        kafkaProducerService.SendOrderEvent(action, orderDto);

        return orderDto;
    }

    public OrderDto Get(string id, string action = "GetOrder")
    {
        var orderDto = new OrderDto();
        dynamodbService.GetEvents("orders", id).ForEach(attributeValues =>
        {
            orderDto.Id = attributeValues["Id"].S;
            orderDto.Timestamp = long.Parse(attributeValues["Timestamp"].S);
            orderDto.AccountId = attributeValues["AccountId"].NULL == true ? null : attributeValues["AccountId"].S;
            orderDto.CartId = attributeValues["CartId"].NULL == true ? null : attributeValues["CartId"].S;
            orderDto.Status = attributeValues["Status"].NULL == true ? null : attributeValues["Status"].S;
            orderDto.Active = attributeValues["Active"].NULL == true ? null : attributeValues["Active"].BOOL;
        });
        return orderDto;
    }

    public List<string> GetAll(string action = "GetAllOrders")
    {
        var list = new List<string>();
        var keys = dynamodbService.GetKeys("orders", action);
        foreach (var attributeValues in keys)
        {
            var key = attributeValues["Id"].S;
            list.Add(key);
        }

        return list;
    }

    public OrderDto Update(string id, OrderDto orderDto, string action = "UpdateOrder")
    {
        if (orderDto.Id == null) throw new Exception("OrderDto ID cannot be null.");
        if (orderDto.Timestamp != null) throw new Exception("OrderDto timestamp cannot be set manually.");
        orderDto.Timestamp = DateTime.UtcNow.ToBinary();

        dynamodbService.AddOrderAsync(orderDto);
        kafkaProducerService.SendOrderEvent(action, orderDto);

        return orderDto;
    }

    public void Delete(string id, string action = "DeleteOrder")
    {
        dynamodbService.GetEvents("orders", id).ForEach(attributeValues =>
        {
            dynamodbService.DeleteItem(
                "orders",
                attributeValues["Id"].S,
                attributeValues["Timestamp"].S);
        });
    }
}