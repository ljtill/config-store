using ConfigStore.Exceptions;

namespace ConfigStore.Clients;

public static class DatabaseClient
{
    private static string? _accountName;
    private static string? _accountKey;

    public static async Task<CosmosClient> CreateAsync(string? accountName, string? accountKey)
    {
        ValidateOptions(accountName, accountKey);

        var options = new CosmosClientOptions()
        {
            SerializerOptions = new CosmosSerializationOptions()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        };
        var client = new CosmosClient($"https://{_accountName}.documents.azure.com:443/", _accountKey, options);
        
        var database = client.GetDatabase("global");
        try
        {
            await database.CreateContainerIfNotExistsAsync("regions", "/location");
        }
        catch (CosmosException ce)
        {
            if (ce.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AuthenticationException();
            }
        }
        
        return client;
    }
    
    private static void ValidateOptions(string? accountName, string? accountKey)
    {
        // Account Name
        if (accountName is null)
        {
            var envAccountName = Environment.GetEnvironmentVariable("COSMOS_ACCOUNT_NAME");
            if (envAccountName is not null)
            {
                _accountName = envAccountName;
            }
            else
            {
                throw new Exception("COSMOS_ACCOUNT_NAME environment variable is not set.");
            }
        }
        else
        {
            _accountName = accountName;    
        }
        
        // Account Key
        if (accountKey is null)
        {
            var envAccountKey = Environment.GetEnvironmentVariable("COSMOS_PRIMARY_KEY");
            if (envAccountKey is not null)
            {
                _accountKey = envAccountKey;
            }
            else
            {
                throw new Exception("COSMOS_PRIMARY_KEY environment variable is not set.");
            }
        }
        else
        {
            _accountKey = accountKey;    
        }
    }
}