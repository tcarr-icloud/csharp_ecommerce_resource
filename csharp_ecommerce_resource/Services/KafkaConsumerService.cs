using Confluent.Kafka;
using csharp_ecommerce_resource.Models;

namespace csharp_ecommerce_resource.Services;

public class KafkaConsumerService<T> : BackgroundService
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly ILogger<KafkaConsumerService<T>> _logger;
    private readonly IProcessor<T> _processor;
    private readonly string? _topic;

    public KafkaConsumerService(HostedConsumerServiceConfiguration hostedConsumerServiceConfiguration,
        IProcessor<T> processor, ILogger<KafkaConsumerService<T>> logger, IConfiguration configuration)
    {
        _processor = processor;
        _topic = hostedConsumerServiceConfiguration.Topic;
        _logger = logger;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = hostedConsumerServiceConfiguration.BootstrapServers,
            GroupId = hostedConsumerServiceConfiguration.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer Service started");

        using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
        consumer.Subscribe(_topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    _processor.HandleMessage(consumeResult.Message.Value);
                    consumer.Commit(consumeResult);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError("Error consuming message: {ErrorReason}", ex.Error.Reason);
                }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka Consumer Service is stopping");
        }
        finally
        {
            consumer.Close();
        }
    }
}