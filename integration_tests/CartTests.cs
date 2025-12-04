using System.Text;
using System.Text.Json;
using csharp_ecommerce_resource.Accounts;
using csharp_ecommerce_resource.Carts;
using csharp_ecommerce_resource.Orders;

namespace IntegrationTests;

[TestClass]
public sealed class Test1
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
    }

    [TestCleanup]
    public void Cleanup()
    {
        SharedClient.DeleteAsync("account/" + accountId).Result.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public void AddCart()
    {
        _cartDto.AccountId = accountId;
        var httpResponseMessage = SharedClient.PostAsync("cart",
                new StringContent(JsonSerializer.Serialize(_cartDto), Encoding.UTF8, "application/json")
            )
            .Result
            .EnsureSuccessStatusCode();
        var responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var cartDtoResponse = JsonSerializer.Deserialize<CartDto>(responseContent);

        Assert.IsNotNull(cartDtoResponse);

        SharedClient.DeleteAsync("cart/" + cartDtoResponse.Id).Result.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public void GetCart()
    {
        _cartDto.AccountId = accountId;
        var httpResponseMessage = SharedClient.PostAsync("cart",
                new StringContent(JsonSerializer.Serialize(_cartDto), Encoding.UTF8, "application/json")
            )
            .Result
            .EnsureSuccessStatusCode();
        var responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var cartDtoResponse = JsonSerializer.Deserialize<CartDto>(responseContent);

        httpResponseMessage = SharedClient.GetAsync("cart/" + cartDtoResponse.Id).Result.EnsureSuccessStatusCode();
        responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        cartDtoResponse = JsonSerializer.Deserialize<CartDto>(responseContent);

        Assert.IsNotNull(cartDtoResponse);

        SharedClient.DeleteAsync("cart/" + cartDtoResponse.Id).Result.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public void GetAllCarts()
    {
        var httpResponseMessage = SharedClient.GetAsync("cart").Result.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public void UpdateCart()
    {
        _cartDto.AccountId = accountId;
        var httpResponseMessage = SharedClient.PostAsync("cart",
                new StringContent(JsonSerializer.Serialize(_cartDto), Encoding.UTF8, "application/json")
            )
            .Result
            .EnsureSuccessStatusCode();
        var responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var cartDtoResponse = JsonSerializer.Deserialize<CartDto>(responseContent);

        cartDtoResponse?.Active = false;
        cartDtoResponse?.Timestamp = null;
        httpResponseMessage = SharedClient.PutAsync("cart/" + cartDtoResponse.Id,
                new StringContent(JsonSerializer.Serialize(cartDtoResponse), Encoding.UTF8, "application/json")
            )
            .Result
            .EnsureSuccessStatusCode();
        responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        cartDtoResponse = JsonSerializer.Deserialize<CartDto>(responseContent);
        Assert.IsNotNull(cartDtoResponse);

        SharedClient.DeleteAsync("cart/" + cartDtoResponse.Id).Result.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public void DeleteCart()
    {
        _cartDto.AccountId = accountId;
        var httpResponseMessage = SharedClient.PostAsync("cart",
                new StringContent(JsonSerializer.Serialize(_cartDto), Encoding.UTF8, "application/json")
            )
            .Result
            .EnsureSuccessStatusCode();
        var responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var cartDtoResponse = JsonSerializer.Deserialize<CartDto>(responseContent);

        SharedClient.DeleteAsync("cart/" + cartDtoResponse.Id).Result.EnsureSuccessStatusCode();
    }
}