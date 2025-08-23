using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BlobService
{
    private readonly string _connectionString;
    private readonly string _containerName;
    private readonly string _folderName;

    public BlobService(IConfiguration configuration)
    {
        _connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")
            ?? throw new Exception("環境変数 AZURE_STORAGE_CONNECTION_STRING を設定してください");

        _containerName = configuration["AzureStorage:ContainerName"];
        _folderName = configuration["AzureStorage:FolderName"];
    }

    public async Task<List<string>> GetImageUrlsAsync()
    {
        var blobUrls = new List<string>();
        BlobContainerClient containerClient = new BlobContainerClient(_connectionString, _containerName);

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: _folderName + "/"))
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                BlobName = blobItem.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddDays(30)  //30日
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
            blobUrls.Add(sasUri.ToString());
        }

        return blobUrls;
    }
}
