namespace Microsoft.ConfigStore.Items;

public static class ExportItems
{
    private static string _databaseName = "global";
    private static string _containerName = "regions";

    public static async Task<List<Region>> InvokeAsync(CosmosClient client)
    {
        var items = new List<Region>();

        try
        {
            var container = client.GetContainer(_databaseName, _containerName);
            using (var feedIterator = container.GetItemQueryIterator<Region>("SELECT * FROM c"))
            {
                while (feedIterator.HasMoreResults)
                {
                    foreach (var item in await feedIterator.ReadNextAsync())
                    {
                        items.Add(item);
                    }
                }
            }
        }
        catch (CosmosException ce)
        {
            Console.WriteLine($"Cosmos error encountered: {ce.Message}");
        }

        return items;
    }
}