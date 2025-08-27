using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using AzurePJ.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

public class BlobService
{
    private readonly string _connectionString;
    private readonly BlobContainerClient containerClient;

    public BlobService(string? connectionString = null)
    {
        _connectionString = connectionString ?? Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")
                            ?? throw new ArgumentNullException("Azure Storage connection string is not set in environment variable AZURE_STORAGE_CONNECTION_STRING");
    }

    private BlobContainerClient GetContainerClient(string containerName)
    => new BlobContainerClient(_connectionString, containerName);

    public async Task<List<string>> GetFoldersAsync()
    {
        var container = GetContainerClient("thumbnails");
        var folders = new HashSet<string>();

        await foreach (var blob in container.GetBlobsAsync())
        {
            var folder = blob.Name.Contains("/") ? blob.Name.Split('/')[0] : "root";
            folders.Add(folder);
        }

        return folders.ToList();
    }

    public async Task<List<(string ThumbnailUrl, string OriginalUrl)>> GetImagesInFolderAsync(string folderName, int hoursValid = 1)
    {
        var thumbnailsContainer = GetContainerClient("thumbnails");
        var imagesContainer = GetContainerClient("mengmeng");

        var result = new List<(string, string)>();

        await foreach (var blobItem in thumbnailsContainer.GetBlobsAsync())
        {
            if (!blobItem.Name.StartsWith(folderName + "/")) continue;

            var thumbClient = thumbnailsContainer.GetBlobClient(blobItem.Name);
            var origClient = imagesContainer.GetBlobClient(blobItem.Name);

            string thumbUrl = GenerateSasUrl(thumbClient, hoursValid);
            string origUrl = GenerateSasUrl(origClient, hoursValid);

            result.Add((thumbUrl, origUrl));
        }

        return result;
    }

    private string GenerateSasUrl(BlobClient blobClient, int hoursValid)
    {
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(hoursValid)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }

    public string GenerateSasUrl(string containerName, string blobPath, int hoursValid = 1)
    {
        var containerClient = GetContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobPath);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobPath,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(hoursValid)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }

}
