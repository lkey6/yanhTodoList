using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
        // 从环境变量读取连接字符串
        _connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")
            ?? throw new Exception("環境変数 AZURE_STORAGE_CONNECTION_STRING を設定してください");

        _containerName = configuration["AzureStorage:ContainerName"];
        _folderName = configuration["AzureStorage:FolderName"];
    }

    public async Task<List<string>> GetImageUrlsAsync()
    {
        var blobUrls = new List<string>();
        BlobContainerClient containerClient = new BlobContainerClient(_connectionString, _containerName);

        // 遍历 mm 文件夹下的所有 Blob
        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: _folderName + "/"))
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
            blobUrls.Add(blobClient.Uri.ToString());
        }

        return blobUrls;
    }
}
