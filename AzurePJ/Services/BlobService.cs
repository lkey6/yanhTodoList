using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using AzurePJ.Models;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

public class BlobService
{
    private readonly string _connectionString;

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

    public async Task CreateBlobFolderAsync(string albumName)
    {
        if (string.IsNullOrWhiteSpace(albumName))
            throw new ArgumentException("Album name cannot be null or empty.", nameof(albumName));

        var container = GetContainerClient("mengmeng");

        var folderBlobClient = container.GetBlobClient($"{albumName}/");

        using var emptyStream = new MemoryStream(Array.Empty<byte>());
        await folderBlobClient.UploadAsync(emptyStream, overwrite: true);
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

    /// <summary>
    /// 安全上传文件到指定容器
    /// </summary>
    public async Task<string> UploadAsync(string containerName, string blobName, Stream fileStream, string contentType)
    {
        var containerClient = GetContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(); // 不设置 PublicAccessType

        var blobClient = containerClient.GetBlobClient(blobName);

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(fileStream, options);
        return blobClient.Uri.ToString();
    }

    /// <summary>
    /// 上传并生成缩略图
    /// </summary>
    public async Task<string> UploadThumbnailAsync(string containerName, string blobName, Stream originalStream, int width = 200)
    {
        using var image = await Image.LoadAsync(originalStream);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(width, 0),
            Mode = ResizeMode.Max
        }));

        using var ms = new MemoryStream();
        await image.SaveAsJpegAsync(ms);
        ms.Position = 0;

        return await UploadAsync(containerName, blobName, ms, "image/jpeg");
    }

}
