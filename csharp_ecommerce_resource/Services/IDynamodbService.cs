using System.Net;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using csharp_ecommerce_resource.Accounts;

namespace csharp_ecommerce_resource.Services;

public interface IDynamodbService
{
    void AddAccountAsync(AccountDto accountDto);
    List<Dictionary<string, AttributeValue>> GetAccount(string id);
}

public class DynamoDbService : IDynamodbService
{
    private const string AccountsTableName = "accounts";

    private readonly AmazonDynamoDBConfig _clientConfig = new()
        { RegionEndpoint = RegionEndpoint.USWest2, ServiceURL = "http://localhost:8000" };

    private readonly BasicAWSCredentials _credentials = new("DUMMYIDEXAMPLE", "DUMMYEXAMPLEKEY");

    public void AddAccountAsync(AccountDto accountDto)
    {
        var dynamoDbClient = new AmazonDynamoDBClient(_credentials, _clientConfig);
        var accountItem = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = accountDto.Id } },
            { "Timestamp", new AttributeValue { S = accountDto.Timestamp.ToString() } },
            { "AccountType", new AttributeValue { S = accountDto.AccountType } },
            { "CompanyName", new AttributeValue { S = accountDto.CompanyName } },
            { "FirstName", new AttributeValue { S = accountDto.FirstName } },
            { "LastName", new AttributeValue { S = accountDto.LastName } },
            { "PhoneNumber", new AttributeValue { S = accountDto.PhoneNumber } },
            { "Email", new AttributeValue { S = accountDto.Email } },
            { "Active", new AttributeValue { BOOL = accountDto.Active?.Equals("true") } }
        };

        var request = new PutItemRequest
        {
            TableName = AccountsTableName,
            Item = accountItem
        };

        var putItemResponse = dynamoDbClient.PutItemAsync(request).Result;
        if (putItemResponse.HttpStatusCode != HttpStatusCode.OK) throw new Exception("Failed to add item.");
    }

    public List<Dictionary<string, AttributeValue>> GetAccount(string id)
    {
        var dynamoDbClient = new AmazonDynamoDBClient(_credentials, _clientConfig);
        var tableName = AccountsTableName;
        var partitionKeyValue = id;

        var request = new QueryRequest
        {
            TableName = tableName,
            KeyConditionExpression = "Id = :id",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":id", new AttributeValue { S = partitionKeyValue } }
            }
        };

        var response = dynamoDbClient.QueryAsync(request).Result;
        if (response.HttpStatusCode != HttpStatusCode.OK) throw new Exception("Failed to retrieve account.");
        return response.Items;
    }
}