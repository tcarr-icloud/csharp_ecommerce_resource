using System.Text.Json;
using Confluent.Kafka;
using csharp_ecommerce_resource.Accounts;

namespace csharp_ecommerce_resource.Services;

public interface IKafkaAccountsProducerService
{
    void SendAccountEvent(string action, AccountDto accountDto);
}

public class KafkaAccountsProducerService : IKafkaAccountsProducerService
{
    private readonly ProducerConfig _config = new() { BootstrapServers = "localhost:29092" };

    private readonly string _topic = "accounts";

    public void SendAccountEvent(string action, AccountDto accountDto)
    {
        var message = new AccountMessage(action, accountDto);
        var stringMessage = JsonSerializer.Serialize(message);
        var producer = new ProducerBuilder<Null, string>(_config).Build();
        producer.ProduceAsync(_topic, new Message<Null, string> { Value = stringMessage });
    }
}