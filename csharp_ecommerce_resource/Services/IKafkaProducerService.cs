using System.Text.Json;
using Confluent.Kafka;
using csharp_ecommerce_resource.Accounts;
using csharp_ecommerce_resource.Carts;
using csharp_ecommerce_resource.Orders;

namespace csharp_ecommerce_resource.Services;

public interface IKafkaProducerService
{
    void SendAccountEvent(string action, AccountDto accountDto, string topic = "accounts");
    void SendOrderEvent(string action, OrderDto orderDto, string topic = "orders");
    void SendCartEvent(string action, CartDto cartDto, string topic = "carts");
}

public class KafkaProducerService : IKafkaProducerService
{
    private readonly ProducerConfig _config = new() { BootstrapServers = "localhost:29092" };
    
    public void SendAccountEvent(string action, AccountDto accountDto, string topic = "accounts")
    {
        var message = new AccountMessage(action, accountDto);
        var stringMessage = JsonSerializer.Serialize(message);
        var producer = new ProducerBuilder<Null, string>(_config).Build();
        producer.ProduceAsync(topic, new Message<Null, string> { Value = stringMessage });
    }
    
    public void SendOrderEvent(string action, OrderDto orderDto, string topic = "orders")
    {
        var message = new OrderMessage(action, orderDto);
        var stringMessage = JsonSerializer.Serialize(message);
        var producer = new ProducerBuilder<Null, string>(_config).Build();
        producer.ProduceAsync(topic, new Message<Null, string> { Value = stringMessage });
    }

    public void SendCartEvent(string action, CartDto cartDto, string topic = "carts")
    {
        var message = new CartMessage(action, cartDto);
        var stringMessage = JsonSerializer.Serialize(message);
        var producer = new ProducerBuilder<Null, string>(_config).Build();
        producer.ProduceAsync(topic, new Message<Null, string> { Value = stringMessage });
    }
}