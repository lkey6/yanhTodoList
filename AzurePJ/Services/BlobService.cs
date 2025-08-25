using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class BlobService
{
    private readonly string _connectionString;
    private readonly BlobContainerClient containerClient;

    public BlobService()
    {
        _connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")
            ?? throw new Exception("環境変数 AZURE_STORAGE_CONNECTION_STRING を設定してください");

        containerClient = new BlobContainerClient(_connectionString, "mengmeng");
    }

    public async Task<Dictionary<string, List<string>>> GetImageUrlsAsync()
    {
        var blobUrls = new List<(string Folder, string Url)>();

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            // SAS URL生成（1時間有効）
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                BlobName = blobItem.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            var sasUri = blobClient.GenerateSasUri(sasBuilder);

            // フォルダ名を取得（"/"の前の部分）
            var folder = blobItem.Name.Contains("/") ? blobItem.Name.Split('/')[0] : "";

            blobUrls.Add((folder, sasUri.ToString()));

        }

        return blobUrls.GroupBy(b => b.Folder)
               .ToDictionary(g => g.Key, g => g.Select(x => x.Url).ToList());
    }

    /// <summary>
    /// 特定のフォルダ（例：mm）だけ取得
    /// </summary>
    public async Task<List<string>> GetBlobsInFolderAsync(string folderName)
    {
        var urls = new List<string>();

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            if (blobItem.Name.StartsWith(folderName + "/"))
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerClient.Name,
                    BlobName = blobItem.Name,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                urls.Add(blobClient.GenerateSasUri(sasBuilder).ToString());
            }
        }

        return urls;
    }

    // フォルダ一覧を取得
    public async Task<List<string>> GetFoldersAsync()
    {
        var folderNames = new HashSet<string>();

        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            var name = blobItem.Name;
            if (name.Contains("/"))
            {
                var folderName = name.Split("/")[0];
                folderNames.Add(folderName);
            }
        }

        return folderNames.ToList();
    }

    // 指定フォルダ内の画像URLを取得（SAS付き）
    public async Task<List<string>> GetImagesAsync(string folderName)
    {
        var imageUrls = new List<string>();

        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            if (blobItem.Name.StartsWith(folderName + "/"))
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);

                // SAS 生成
                var sasBuilder = new Azure.Storage.Sas.BlobSasBuilder
                {
                    BlobContainerName = containerClient.Name,
                    BlobName = blobItem.Name,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };
                sasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Read);

                var sasUri = blobClient.GenerateSasUri(sasBuilder);
                imageUrls.Add(sasUri.ToString());
            }
        }

        return imageUrls;
    }

    public async Task<Dictionary<string, List<(string ThumbnailUrl, string OriginalUrl)>>> GetThumbnailsAsync()
    {
        var result = new Dictionary<string, List<(string, string)>>();

        var thumbnailContainer = new BlobContainerClient(_connectionString, "thumbnails");
        var originalContainer = new BlobContainerClient(_connectionString, "images");

        await foreach (BlobItem blobItem in thumbnailContainer.GetBlobsAsync(BlobTraits.None, BlobStates.None, null))
        {
            string thumbnailUrl = $"{thumbnailContainer.Uri}/{blobItem.Name}";
            string originalUrl = $"{originalContainer.Uri}/{blobItem.Name}";

            string folder = System.IO.Path.GetDirectoryName(blobItem.Name) ?? "root";

            if (!result.ContainsKey(folder))
                result[folder] = new List<(string, string)>();

            result[folder].Add((thumbnailUrl, originalUrl));
        }

        return result;
    }


}
