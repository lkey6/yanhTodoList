using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;

public class BlobService
{
    private readonly string _connectionString;
    private readonly string _containerName;
    private readonly string _accountKey;
    private readonly string _accountName;

    public BlobService(IConfiguration configuration)
    {
        _accountName = configuration["AzureStorage:AccountName"];
        _accountKey = configuration["AzureStorage:AccountKey"];
        _containerName = configuration["AzureStorage:ContainerName"];

        _connectionString = $"DefaultEndpointsProtocol=https;AccountName={_accountName};AccountKey={_accountKey};EndpointSuffix=core.windows.net";
    }

    public async Task<List<string>> ListBlobsWithSasAsync(string folder = "")
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        var sharedKeyCredential = new StorageSharedKeyCredential(_accountName, _accountKey);

        var urls = new List<string>();
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: folder))
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName = blobItem.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);
            urls.Add(sasUri.ToString());
        }

        return urls;
    }
}
