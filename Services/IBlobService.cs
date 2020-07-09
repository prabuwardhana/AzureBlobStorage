using System.Collections.Generic;
using System.Threading.Tasks;
using AzureBlobStorage.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AzureBlobStorage.Services
{
    public interface IBlobService
    {
        Task<BlobInfo> GetBlobAsync(string name);

        Task<IEnumerable<FileData>> ListBlobsAsync();

        Task UploadFileBlobAsync(string filePath, string fileName);
        
        Task UploadContentBlobAsync(string content, string fileName);

        Task UploadContentBlobAsync(IFormFile file, ModelStateDictionary modelState);

        Task DeleteBlobAsync(string blobName);
    }
}