using System.IO;

namespace AzureBlobStorage.Models
{
    public class BlobInfo
    {
        public BlobInfo(Stream content, string contentType)
        {
            Content = content;
            ContentType = contentType;
        }
        
        public Stream Content { get; }

        public string ContentType { get; }
    }
}