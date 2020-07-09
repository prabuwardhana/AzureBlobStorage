namespace AzureBlobStorage.Options
{
    public class AzureBlobStorageOption
    {
        public const string AzureBlobOption = "AzureBlobOption";

        public string ConnectionString { get; set; }
        public string Container { get; set; }
        public long FileSizeLimit { get; set; }
    }
}