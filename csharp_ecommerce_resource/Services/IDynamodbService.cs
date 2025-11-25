using System.Net;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using csharp_ecommerce_resource.Models;

namespace csharp_ecommerce_resource.Services;

public interface IDynamodbService
{
    void AddAccountAsync(Account account);
}

public class DynamoDbService : IDynamodbService
{
    private const string AccountsTableName = "accounts";

    private readonly AmazonDynamoDBConfig _clientConfig = new()
        { RegionEndpoint = RegionEndpoint.USWest2, ServiceURL = "http://localhost:8000" };

    private readonly BasicAWSCredentials _credentials = new("DUMMYIDEXAMPLE", "DUMMYEXAMPLEKEY");

    public void AddAccountAsync(Account account)
    {
        var dynamoDbClient = new AmazonDynamoDBClient(_credentials, _clientConfig);
        var accountItem = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = account.Id } },
            { "Timestamp", new AttributeValue { S = account.Timestamp.ToString() } },
            { "AccountType", new AttributeValue { S = account.AccountType } },
            { "CompanyName", new AttributeValue { S = account.CompanyName } },
            { "FirstName", new AttributeValue { S = account.FirstName } },
            { "LastName", new AttributeValue { S = account.LastName } },
            { "PhoneNumber", new AttributeValue { S = account.PhoneNumber } },
            { "Email", new AttributeValue { S = account.Email } }
        };

        var request = new PutItemRequest
        {
            TableName = AccountsTableName,
            Item = accountItem
        };

        var putItemResponse = dynamoDbClient.PutItemAsync(request).Result;
        if (putItemResponse.HttpStatusCode != HttpStatusCode.OK) throw new Exception("Failed to add item.");
    }
}