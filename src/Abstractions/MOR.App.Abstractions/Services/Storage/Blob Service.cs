namespace MOR.App.Services.Storage
{
    public interface IBlobServiceProvider
    {
        string Instance { get; }

        ValueTask<IBlobService> GetBlobServiceAsync(string container);
    }

    public interface IBlobService
    {
        string Instance { get; }
        string Container { get; }

        Task<bool> ExistsAsync(string path);

        Task<BlobMetadataResult> GetBlobInfoAsync(string path);
        Task<BlobContentResult> DownloadBlobAsync(string path);
        Task<Stream> DownloadBlobStreamAsync(string path);

        Task UploadBlobAsync(BlobUploadRequest request);

        Task DeleteBlobAsync(string path, bool slient = true);

        Task<Dictionary<string, string>> GetBlobTagsAsync(string path);
    }


    public static class BlobConstants
    {
        public const string MetaKey_AdditionalData = "AdditionalData";
        public const string MetaKey_FileName = "OriginalFileName";
    }

    public class BlobMetadataResult
    {
        public required Uri Url { get; init; }
        public string? Filename { get; set; }
        public string? ContentType { get; set; }
        public long? Length { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
    }

    public class BlobContentResult : BlobMetadataResult
    {
        public required Stream Content { get; init; }
    }


    public class BlobUploadRequest
    {
        private readonly Dictionary<string, string> _Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


        private BlobUploadRequest(string targetPath, string mediaType)
        {
            TargetPath = targetPath;
            MediaType = mediaType;
        }

        public BlobUploadRequest(string targetPath, Stream contentStream, string mediaType = System.Net.Mime.MediaTypeNames.Application.Octet)
            : this(targetPath, mediaType)
        {
            Type = BlobContentType.Stream;
            ContentStream = contentStream;
        }

        public BlobUploadRequest(string targetPath, byte[] contentBytes, string mediaType = System.Net.Mime.MediaTypeNames.Application.Octet)
            : this(targetPath, mediaType)
        {
            Type = BlobContentType.Bytes;
            ContentBytes = contentBytes;
        }

        public BlobUploadRequest(string targetPath, string contentString, string mediaType = System.Net.Mime.MediaTypeNames.Application.Octet)
            : this(targetPath, mediaType)
        {
            Type = BlobContentType.String;
            ContentString = contentString;
        }


        public BlobContentType Type { get; set; }

        public required string TargetPath { get; set; }

        public Stream? ContentStream { get; set; }
        public byte[]? ContentBytes { get; set; }
        public string? ContentString { get; set; }

        public string MediaType { get; set; }

        public IReadOnlyDictionary<string, string> Metadata => _Metadata;


        public void AddMetadata_FileName(string filename)
        {
            AddMetadata(BlobConstants.MetaKey_FileName, filename);
        }

        public void AddMetadata_AdditionalData(string data)
        {
            AddMetadata(BlobConstants.MetaKey_AdditionalData, data);
        }

        public void AddMetadata(string key, string value)
        {
            _Metadata.TryAddOrUpdate(key, value);
        }


        public enum BlobContentType
        {
            Stream,
            Bytes,
            String,
        }
    }
}
