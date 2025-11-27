using System.Text.Json.Serialization;
using csharp_ecommerce_resource.Accounts;
using csharp_ecommerce_resource.Carts;
using csharp_ecommerce_resource.Orders;
using csharp_ecommerce_resource.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddSingleton<IAccountService, AccountService>();
builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddSingleton<IDynamodbService, DynamoDbService>();
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

builder.Services.AddSingleton<IProcessor<AccountMessage>, AccountProcessor>();
builder.Services.AddHostedService<ConsumerService<AccountMessage>>(sp => new ConsumerService<AccountMessage>(
    new ConsumerServiceConfiguration
        { Topic = "accounts", BootstrapServers = "localhost:29092", GroupId = "accounts-consumer-group" },
    sp.GetRequiredService<IProcessor<AccountMessage>>(),
    sp.GetRequiredService<ILogger<ConsumerService<AccountMessage>>>()));

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();