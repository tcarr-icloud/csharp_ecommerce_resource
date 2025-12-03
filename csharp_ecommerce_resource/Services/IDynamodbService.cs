using System.Net;
using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using csharp_ecommerce_resource.Accounts;
using csharp_ecommerce_resource.Carts;
using csharp_ecommerce_resource.Orders;

namespace csharp_ecommerce_resource.Services;

public interface IDynamodbService
{
    Dictionary<string, AttributeValue> AddCartAsync(CartDto cartDto);
    Dictionary<string, AttributeValue> AddAccountAsync(AccountDto accountDto);
    Dictionary<string, AttributeValue> AddOrderAsync(OrderDto orderDto);
    List<Dictionary<string, AttributeValue>> GetEvents(string tableName, string id);
    HashSet<string> GetAccountByEmail(string email);
    void DeleteItem(string tableName, string partitionKey, string sortKey);
    List<Dictionary<string, AttributeValue>> GetKeys(string accounts, string action);
}

public class DynamoDbService : IDynamodbService
{
    private const string AccountsTableName = "accounts";
    private const string CartsTableName = "carts";
    private const string OrdersTableName = "orders";

    private readonly AmazonDynamoDBConfig _clientConfig = new()
        { RegionEndpoint = RegionEndpoint.USWest2, ServiceURL = "http://localhost:8000" };

    private readonly BasicAWSCredentials _credentials = new("DUMMYIDEXAMPLE", "DUMMYEXAMPLEKEY");

    public Dictionary<string, AttributeValue> AddCartAsync(CartDto cartDto)
    {
        var cartItem = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = cartDto.Id } },
            { "Timestamp", new AttributeValue { S = cartDto.Timestamp.ToString() } },
            {
                "AccountId",
                cartDto.AccountId != null
                    ? new AttributeValue { S = cartDto.AccountId }
                    : new AttributeValue { NULL = true }
            },
            {
                "Items",
                cartDto.Items != null
                    ? new AttributeValue { S = JsonSerializer.Serialize(cartDto.Items) }
                    : new AttributeValue { NULL = true }
            },
            {
                "Active",
                cartDto.Active != null
                    ? new AttributeValue { BOOL = cartDto.Active.Value }
                    : new AttributeValue { NULL = true }
            }
        };
        var request = new PutItemRequest { TableName = CartsTableName, Item = cartItem };
        var dynamoDbClient = new AmazonDynamoDBClient(_credentials, _clientConfig);
        var putItemResponse = dynamoDbClient.PutItemAsync(request).Result;
        return putItemResponse.HttpStatusCode != HttpStatusCode.OK
            ? throw new Exception("Failed to add item.")
            : cartItem;
    }

    public Dictionary<string, AttributeValue> AddAccountAsync(AccountDto accountDto)
    {
        var accountItem = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = accountDto.Id } },
            { "Timestamp", new AttributeValue { S = accountDto.Timestamp.ToString() } },
            {
                "CompanyName",
                accountDto.CompanyName != null
                    ? new AttributeValue { S = accountDto.CompanyName }
                    : new AttributeValue { NULL = true }
            },
            {
                "FirstName",
                accountDto.FirstName != null
                    ? new AttributeValue { S = accountDto.FirstName }
                    : new AttributeValue { NULL = true }
            },
            {
                "LastName",
                accountDto.LastName != null
                    ? new AttributeValue { S = accountDto.LastName }
                    : new AttributeValue { NULL = true }
            },
            {
                "PhoneNumber",
                accountDto.PhoneNumber != null
                    ? new AttributeValue { S = accountDto.PhoneNumber }
                    : new AttributeValue { NULL = true }
            },
            {
                "Email",
                accountDto.Email != null
                    ? new AttributeValue { S = accountDto.Email }
                    : new AttributeValue { NULL = true }
            },
            {
                "Type",
                accountDto.Type != null
                    ? new AttributeValue { S = accountDto.Type }
                    : new AttributeValue { NULL = true }
            },
            {
                "Active",
                accountDto.Active != null
                    ? new AttributeValue { BOOL = accountDto.Active.Value }
                    : new AttributeValue { NULL = true }
            }
        };
        var request = new PutItemRequest { TableName = AccountsTableName, Item = accountItem };
        var dynamoDbClient = new AmazonDynamoDBClient(_credentials, _clientConfig);
        var putItemResponse = dynamoDbClient.PutItemAsync(request).Result;
        return putItemResponse.HttpStatusCode != HttpStatusCode.OK
            ? throw new Exception("Failed to add item.")
            : accountItem;
    }

    public Dictionary<string, AttributeValue> AddOrderAsync(OrderDto orderDto)
    {
        var orderItem = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = orderDto.Id } },
            { "Timestamp", new AttributeValue { S = orderDto.Timestamp.ToString() } },
            {
                "AccountId",
                orderDto.AccountId != null
                    ? new AttributeValue { S = orderDto.AccountId }
                    : new AttributeValue { NULL = true }
            },
            {
                "CartId",
                orderDto.CartId != null
                    ? new AttributeValue { S = orderDto.CartId }
                    : new AttributeValue { NULL = true }
            },
            {
                "Status",
                orderDto.Status != null
                    ? new AttributeValue { S = orderDto.Status }
                    : new AttributeValue { NULL = true }
            },
            {
                "Active",
                orderDto.Active != null
                    ? new AttributeValue { BOOL = orderDto.Active.Value }
                    : new AttributeValue { NULL = true }
            }
        };
        var request = new PutItemRequest { TableName = OrdersTableName, Item = orderItem };
        var dynamoDbClient = new AmazonDynamoDBClient(_credentials, _clientConfig);
        var putItemResponse = dynamoDbClient.PutItemAsync(request).Result;
        return putItemResponse.HttpStatusCode != HttpStatusCode.OK
            ? throw new Exception("Failed to add item.")
            : orderItem;
    }

    public List<Dictionary<string, AttributeValue>> GetEvents(string tableName, string id)
    {
        var dynamoDbClient = new AmazonDynamoDBClient(_credentials, _clientConfig);
        var request = new QueryRequest
        {
            TableName = tableName, KeyConditionExpression = "Id = :id",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                { { ":id", new AttributeValue { S = id } } }
        };
        var response = dynamoDbClient.QueryAsync(request).Result;
        return response.HttpStatusCode != HttpStatusCode.OK
            ? throw new Exception("Failed to retrieve events.")
            : response.Items;
    }

    public HashSet<string> GetAccountByEmail(string email)
    {
        var dynamoDbClient = new AmazonDynamoDBClient(_credentials, _clientConfig);
        var request = new ScanRequest
        {
            TableName = AccountsTableName,
            FilterExpression = "Email = :email",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                { { ":email", new AttributeValue { S = email } } }
        };
        var response = dynamoDbClient.ScanAsync(request).Result;
        var ids = new HashSet<string>();
        response.Items.ForEach(item => ids.Add(item["Id"].S));
        return ids;
    }

    public void DeleteItem(string tableName, string partitionKey, string sortKey)
    {
        var request = new DeleteItemRequest
        {
            TableName = tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = partitionKey }},
                { "Timestamp", new AttributeValue { S = sortKey }}
            }
        };

        var dynamoDbClient = new AmazonDynamoDBClient(_credentials, _clientConfig);
        var deleteResponse = dynamoDbClient.DeleteItemAsync(request).Result;
        if (deleteResponse.HttpStatusCode != HttpStatusCode.OK)
        {
            throw new Exception("Failed to delete item.");
        }
    }

    public List<Dictionary<string, AttributeValue>> GetKeys(string tableName, string action)
    {
        var dynamoDbClient = new AmazonDynamoDBClient(_credentials, _clientConfig);
        var request = new ScanRequest
        {
            TableName = tableName, 
            ProjectionExpression = "Id"
        };
        var response = dynamoDbClient.ScanAsync(request).Result;
        return response.Items;
    }

    public void DeleteAllItemsForPartitionKey(string tableName, string partitionKey)
    {
        var dynamoDbClient = new AmazonDynamoDBClient(_credentials, _clientConfig);
        var request = new ScanRequest
        {
            TableName = tableName, 
            FilterExpression = "Id = :id", 
            ExpressionAttributeValues = new Dictionary<string, AttributeValue> { { ":id", new AttributeValue { S = partitionKey } } }
        };
        var response = dynamoDbClient.ScanAsync(request).Result;
        response.Items.ForEach(item => DeleteItem(tableName, item["Id"].S, item["Timestamp"].S));
    }
}