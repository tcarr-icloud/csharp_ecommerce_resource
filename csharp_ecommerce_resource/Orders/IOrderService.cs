using csharp_ecommerce_resource.Services;

namespace csharp_ecommerce_resource.Orders;

public interface IOrderService
{
    OrderDto CreateOrder(OrderDto orderDto, string action = "CreateOrder");
    OrderDto GetOrder(string id, string action = "GetOrder");
    OrderDto[] GetAllOrders(string action = "GetAllOrders");
    OrderDto UpdateOrder(string id, OrderDto orderDto, string action = "UpdateOrder");
    void DeleteOrder(string id, string action = "DeleteOrder");
}

public class OrderService(
    IDynamodbService dynamodbService,
    IKafkaProducerService kafkaProducerService) : IOrderService
{
    public OrderDto CreateOrder(OrderDto orderDto, string action = "CreateOrder")
    {
        if (orderDto.Id != null) throw new Exception("OrderDto ID cannot be set manually.");
        orderDto.Id = Guid.NewGuid().ToString();

        if (orderDto.Timestamp != null) throw new Exception("OrderDto timestamp cannot be set manually.");
        orderDto.Timestamp = DateTime.UtcNow;

        dynamodbService.AddOrderAsync(orderDto);
        kafkaProducerService.SendOrderEvent(action, orderDto);

        return orderDto;
    }

    public OrderDto GetOrder(string id, string action = "GetOrder")
    {
        var orderDto = new OrderDto();
        dynamodbService.GetEvents("orders", id).ForEach(attributeValues =>
        {
            orderDto.Id = attributeValues["Id"].S;
            orderDto.Timestamp = DateTime.Parse(attributeValues["Timestamp"].S);
            orderDto.AccountId = attributeValues["AccountId"].NULL == true ? null : attributeValues["AccountId"].S;
            orderDto.CartId = attributeValues["CartId"].NULL == true ? null : attributeValues["CartId"].S;
            orderDto.Status = attributeValues["Status"].NULL == true ? null : attributeValues["Status"].S;
            orderDto.Active = attributeValues["Active"].NULL == true ? null : attributeValues["Active"].BOOL;
        });
        return orderDto;
    }

    public OrderDto[] GetAllOrders(string action = "GetAllOrders")
    {
        throw new NotImplementedException();
    }

    public OrderDto UpdateOrder(string id, OrderDto orderDto, string action = "UpdateOrder")
    {
        if (orderDto.Id == null) throw new Exception("OrderDto ID cannot be null.");
        if (orderDto.Timestamp != null) throw new Exception("OrderDto timestamp cannot be set manually.");
        orderDto.Timestamp = DateTime.UtcNow;
        
        dynamodbService.AddOrderAsync(orderDto);
        kafkaProducerService.SendOrderEvent(action, orderDto);

        return orderDto;
    }

    public void DeleteOrder(string id, string action = "DeleteOrder")
    {
        var orderDto = GetOrder(id);
        orderDto.Active = false;
        orderDto.Timestamp = null;
        UpdateOrder(id, orderDto, action);
    }
}