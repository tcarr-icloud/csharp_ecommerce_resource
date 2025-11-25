using System.Text.Json;
using Confluent.Kafka;
using csharp_ecommerce_resource.Models;

namespace csharp_ecommerce_resource.Services;

public interface IKafkaAccountsProducerService
{
    void SendAccountEvent(string action, Account account);
}

public class KafkaAccountsProducerService : IKafkaAccountsProducerService
{
    private readonly ProducerConfig _config = new() { BootstrapServers = "localhost:29092" };

    private readonly string _topic = "accounts";

    public void SendAccountEvent(string action, Account account)
    {
        var message = new AccountMessage(action, account);
        var stringMessage = JsonSerializer.Serialize(message);
        var producer = new ProducerBuilder<Null, string>(_config).Build();
        producer.ProduceAsync(_topic, new Message<Null, string> { Value = stringMessage });
    }
}