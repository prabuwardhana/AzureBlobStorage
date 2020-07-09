using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlobStorage.Extensions;
using AzureBlobStorage.Models;
using AzureBlobStorage.Options;
using AzureBlobStorage.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using BlobInfo = AzureBlobStorage.Models.BlobInfo;

namespace AzureBlobStorage.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;
        private readonly AzureBlobStorageOption _options;
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { ".png", ".jpg" };
        
        public BlobService(IOptions<AzureBlobStorageOption> options)
        {
            _options = options.Value;

            _fileSizeLimit = _options.FileSizeLimit;  
            _blobServiceClient = new BlobServiceClient(_options.ConnectionString);   
            _containerClient = _blobServiceClient.GetBlobContainerClient(_options.Container);
        }

        public async Task<BlobInfo> GetBlobAsync(string name)
        {
            var blobClient = _containerClient.GetBlobClient(name);
            var blobDownloadInfo = await blobClient.DownloadAsync();
            return new BlobInfo(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);
        }

        public async Task<IEnumerable<FileData>> ListBlobsAsync()
        {
            var fileList = new List<FileData>();

            await foreach (var blobItem in _containerClient.GetBlobsAsync())
            {
                var fileSize = blobItem.Properties.ContentLength ?? default;
                fileList.Add(new FileData()
                {
                    FileName = blobItem.Name,
                    FileSize = Math.Round((fileSize / 1024f), 2).ToString(),
                    ModifiedOn = DateTime.Parse(blobItem.Properties.LastModified.ToString()).ToLocalTime().ToString(),
                    Uri = _containerClient.Uri  + "/" + blobItem.Name
                });
            }

            return fileList;
        }

        public async Task UploadFileBlobAsync(string filePath, string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(filePath, new BlobHttpHeaders {ContentType = filePath.GetContentType()});
        }

        public async Task UploadContentBlobAsync(string content, string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            var bytes = Encoding.UTF8.GetBytes(content);
            
            await using var memoryStream = new MemoryStream(bytes);
            await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders {ContentType = fileName.GetContentType()});
        }

        public async Task UploadContentBlobAsync(IFormFile file, ModelStateDictionary modelState)
        {
            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            var blobClient = _containerClient.GetBlobClient(trustedFileNameForFileStorage);
            var formFileContent = await FileHelpers.ProcessFormFile(file, modelState, _permittedExtensions, _fileSizeLimit);

            await using var memoryStream = new MemoryStream(formFileContent);
            await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders {ContentType = file.ContentType});
        }

        public async Task DeleteBlobAsync(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}