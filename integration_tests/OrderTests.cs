using System.Text;
using System.Text.Json;
using csharp_ecommerce_resource.Accounts;
using csharp_ecommerce_resource.Carts;
using csharp_ecommerce_resource.Orders;

namespace IntegrationTests;

[TestClass]
public sealed class OrderTests
{
    private static readonly HttpClient SharedClient = new()
    {
        BaseAddress = new Uri("http://localhost:5201/api/")
    };

    private readonly AccountDto _accountDto = new()
    {
        CompanyName = "Hello",
        FirstName = "Test",
        LastName = "User",
        PhoneNumber = "1234567890",
        Email = "test.user@development.com",
        Type = "Personal",
        Active = true
    };

    private readonly CartDto _cartDto = new()
    {
        Id = null,
        Timestamp = null,
        AccountId = null,
        Items = null,
        Active = true
    };

    private readonly OrderDto _orderDto = new()
    {
        Id = null,
        Timestamp = null,
        AccountId = null,
        CartId = null,
        Status = null,
        Active = true
    };

    private string accountId;
    private string cartId;

    [TestInitialize]
    public void Initialize()
    {
        var httpResponseMessage = SharedClient.PostAsync("account",
                new StringContent(JsonSerializer.Serialize(_accountDto), Encoding.UTF8, "application/json")
            )
            .Result
            .EnsureSuccessStatusCode();
        var responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var accountDtoResponse = JsonSerializer.Deserialize<AccountDto>(responseContent);
        accountId = accountDtoResponse?.Id;

        _cartDto.AccountId = accountId;
        httpResponseMessage = SharedClient.PostAsync("cart",
                new StringContent(JsonSerializer.Serialize(_cartDto), Encoding.UTF8, "application/json")
            )
            .Result
            .EnsureSuccessStatusCode();
        responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var cartDtoResponse = JsonSerializer.Deserialize<CartDto>(responseContent);
        cartId = cartDtoResponse?.Id;
    }

    [TestCleanup]
    public void Cleanup()
    {
        SharedClient.DeleteAsync("cart/" + cartId).Result.EnsureSuccessStatusCode();
        SharedClient.DeleteAsync("account/" + accountId).Result.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public void AddOrder()
    {
        _orderDto.CartId = cartId;
        _orderDto.AccountId = accountId;

        var httpResponseMessage = SharedClient.PostAsync(
                "order",
                new StringContent(JsonSerializer.Serialize(_cartDto),
                    Encoding.UTF8, "application/json")
            )
            .Result
            .EnsureSuccessStatusCode();
        var responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var dtoResponse = JsonSerializer.Deserialize<OrderDto>(responseContent);

        Assert.IsNotNull(dtoResponse);

        SharedClient.DeleteAsync("order/" + dtoResponse.Id).Result.EnsureSuccessStatusCode();
    }
}