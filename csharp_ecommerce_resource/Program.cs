using System.Text.Json.Serialization;
using csharp_ecommerce_resource.Models;
using csharp_ecommerce_resource.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddSingleton<IAccountService, AccountService>();
builder.Services.AddSingleton<IDynamodbService, DynamoDbService>();
builder.Services.AddSingleton<IKafkaAccountsProducerService, KafkaAccountsProducerService>();

builder.Services.AddSingleton<IProcessor<AccountMessage>, AccountProcessor>();
builder.Services.AddHostedService<KafkaConsumerService<AccountMessage>>(sp =>
{
    var accountHostedConfig = new HostedConsumerServiceConfiguration
        { Topic = "accounts", BootstrapServers = "localhost:29092", GroupId = "accounts-consumer-group" };
    return new KafkaConsumerService<AccountMessage>(accountHostedConfig,
        sp.GetRequiredService<IProcessor<AccountMessage>>(),
        sp.GetRequiredService<ILogger<KafkaConsumerService<AccountMessage>>>(),
        sp.GetRequiredService<IConfiguration>());
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();