using System.Text;
using System.Text.Json;
using csharp_ecommerce_resource.Accounts;
using csharp_ecommerce_resource.Carts;
using csharp_ecommerce_resource.Orders;

namespace IntegrationTests;

[TestClass]
public sealed class AccountTests
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

    [TestMethod]
    public void AddAccount()
    {
        var httpResponseMessage = SharedClient.PostAsync("account",
                new StringContent(JsonSerializer.Serialize(_accountDto), Encoding.UTF8, "application/json")
            )
            .Result
            .EnsureSuccessStatusCode();
        var responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var accountDtoResponse = JsonSerializer.Deserialize<AccountDto>(responseContent);

        Assert.IsNotNull(accountDtoResponse);
        Assert.IsNotNull(accountDtoResponse.Id);
        Assert.IsNotNull(accountDtoResponse.Timestamp);
        Assert.AreEqual(_accountDto.CompanyName, accountDtoResponse.CompanyName);
        Assert.AreEqual(_accountDto.FirstName, accountDtoResponse.FirstName);
        Assert.AreEqual(_accountDto.LastName, accountDtoResponse.LastName);
        Assert.AreEqual(_accountDto.PhoneNumber, accountDtoResponse.PhoneNumber);
        Assert.AreEqual(_accountDto.Email, accountDtoResponse.Email);
        Assert.AreEqual(_accountDto.Type, accountDtoResponse.Type);
        Assert.AreEqual(_accountDto.Active, accountDtoResponse.Active);

        SharedClient.DeleteAsync("account/" + accountDtoResponse.Id).Result.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public void GetAccount()
    {
        var httpResponseMessage = SharedClient.PostAsync("account",
                new StringContent(JsonSerializer.Serialize(_accountDto), Encoding.UTF8, "application/json")
            )
            .Result
            .EnsureSuccessStatusCode();
        var responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var accountDtoResponse = JsonSerializer.Deserialize<AccountDto>(responseContent);
        var id = accountDtoResponse?.Id;

        httpResponseMessage = SharedClient.GetAsync("account/" + id).Result.EnsureSuccessStatusCode();
        responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        accountDtoResponse = JsonSerializer.Deserialize<AccountDto>(responseContent);

        Assert.IsNotNull(accountDtoResponse);
        Assert.IsNotNull(accountDtoResponse.Id);
        Assert.IsNotNull(accountDtoResponse.Timestamp);
        Assert.AreEqual(_accountDto.CompanyName, accountDtoResponse.CompanyName);
        Assert.AreEqual(_accountDto.FirstName, accountDtoResponse.FirstName);
        Assert.AreEqual(_accountDto.LastName, accountDtoResponse.LastName);
        Assert.AreEqual(_accountDto.PhoneNumber, accountDtoResponse.PhoneNumber);
        Assert.AreEqual(_accountDto.Email, accountDtoResponse.Email);
        Assert.AreEqual(_accountDto.Type, accountDtoResponse.Type);
        Assert.AreEqual(_accountDto.Active, accountDtoResponse.Active);

        SharedClient.DeleteAsync("account/" + accountDtoResponse.Id).Result.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public void GetAllAccounts()
    {
        var httpResponseMessage = SharedClient.GetAsync("account").Result.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public void UpdateAccount()
    {
        var httpResponseMessage = SharedClient.PostAsync("account",
                new StringContent(JsonSerializer.Serialize(_accountDto), Encoding.UTF8, "application/json")
            )
            .Result
            .EnsureSuccessStatusCode();
        var responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var accountDtoResponse = JsonSerializer.Deserialize<AccountDto>(responseContent);
        var id = accountDtoResponse?.Id;

        httpResponseMessage = SharedClient.GetAsync("account/" + id).Result.EnsureSuccessStatusCode();
        responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        accountDtoResponse = JsonSerializer.Deserialize<AccountDto>(responseContent);

        accountDtoResponse?.FirstName = "Updated";
        accountDtoResponse?.Timestamp = null;
        httpResponseMessage = SharedClient.PutAsync("account/" + id,
                new StringContent(JsonSerializer.Serialize(accountDtoResponse), Encoding.UTF8, "application/json")
            )
            .Result
            .EnsureSuccessStatusCode();
        responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        accountDtoResponse = JsonSerializer.Deserialize<AccountDto>(responseContent);
        Assert.IsNotNull(accountDtoResponse);

        SharedClient.DeleteAsync("account/" + accountDtoResponse.Id).Result.EnsureSuccessStatusCode();
    }

    [TestMethod]
    public void DeleteAccount()
    {
        var httpResponseMessage = SharedClient.PostAsync("account",
                new StringContent(JsonSerializer.Serialize(_accountDto), Encoding.UTF8, "application/json")
            )
            .Result
            .EnsureSuccessStatusCode();
        var responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var accountDtoResponse = JsonSerializer.Deserialize<AccountDto>(responseContent);
        var id = accountDtoResponse?.Id;
        Assert.IsTrue(accountDtoResponse?.Active);

        SharedClient.DeleteAsync("account/" + id).Result.EnsureSuccessStatusCode();
    }
}