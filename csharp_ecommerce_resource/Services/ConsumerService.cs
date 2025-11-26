using Confluent.Kafka;

namespace csharp_ecommerce_resource.Services;

public class ConsumerService<T> : BackgroundService
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly ILogger<ConsumerService<T>> _logger;
    private readonly IProcessor<T> _processor;
    private readonly string? _topic;

    public ConsumerService(ConsumerServiceConfiguration consumerServiceConfiguration,
        IProcessor<T> processor, ILogger<ConsumerService<T>> logger)
    {
        _processor = processor;
        _topic = consumerServiceConfiguration.Topic;
        _logger = logger;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = consumerServiceConfiguration.BootstrapServers,
            GroupId = consumerServiceConfiguration.GroupId,
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

public class ConsumerServiceConfiguration
{
    public string? Topic { get; set; }
    public string? BootstrapServers { get; set; }
    public string? GroupId { get; set; }
}