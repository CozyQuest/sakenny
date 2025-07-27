using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace sakenny.Application.Services
{
    public class BlobService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobService(IConfiguration config)
        {
            var connectionString = config["AzureBlobStorage:ConnectionString"];
            var containerName = config["AzureBlobStorage:ContainerName"];

            var serviceClient = new BlobServiceClient(connectionString);
            _containerClient = serviceClient.GetBlobContainerClient(containerName);

            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            // Validate content type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                throw new InvalidOperationException("Only image files are allowed.");

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var blobClient = _containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString();
        }

        public async Task<(Stream? Content, string? ContentType)> GetImageStreamAsync(string url)
        {
            try
            {
                //Console.WriteLine($"[DEBUG] Original URL: {url}");

                var uri = new Uri(url);
                //Console.WriteLine($"[DEBUG] URI AbsolutePath: {uri.AbsolutePath}");

                // Split and examine all path segments
                var pathSegments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                //Console.WriteLine($"[DEBUG] Path segments count: {pathSegments.Length}");
                //Console.WriteLine($"[DEBUG] Path segments: [{string.Join(", ", pathSegments)}]");

                if (pathSegments.Length < 2)
                {
                    //Console.WriteLine("[DEBUG] Not enough path segments");
                    return (null, null);
                }

                // Try different approaches to construct the blob path
                var blobPath = string.Join('/', pathSegments.Skip(1));
                //Console.WriteLine($"[DEBUG] Constructed blob path: {blobPath}");

                // Alternative: If your container is already set to the right container,
                // you might need to skip more or fewer segments
                var alternativePath = string.Join('/', pathSegments.Skip(2));
                //Console.WriteLine($"[DEBUG] Alternative blob path: {alternativePath}");

                var blobClient = _containerClient.GetBlobClient(blobPath);
                //Console.WriteLine($"[DEBUG] Checking if blob exists: {blobClient.Name}");
                //Console.WriteLine($"[DEBUG] Container name: {_containerClient.Name}");

                // Check if blob exists
                var existsResponse = await blobClient.ExistsAsync();
                //Console.WriteLine($"[DEBUG] Blob exists: {existsResponse.Value}");

                if (existsResponse.Value)
                {
                    var download = await blobClient.DownloadAsync();
                    var properties = await blobClient.GetPropertiesAsync();
                    //Console.WriteLine($"[DEBUG] Successfully retrieved blob with content type: {properties.Value.ContentType}");
                    return (download.Value.Content, properties.Value.ContentType);
                }

                // If first attempt fails, try the alternative path
                if (!string.IsNullOrEmpty(alternativePath) && alternativePath != blobPath)
                {
                    //Console.WriteLine($"[DEBUG] Trying alternative path: {alternativePath}");
                    var altBlobClient = _containerClient.GetBlobClient(alternativePath);
                    var altExistsResponse = await altBlobClient.ExistsAsync();
                    //Console.WriteLine($"[DEBUG] Alternative blob exists: {altExistsResponse.Value}");

                    if (altExistsResponse.Value)
                    {
                        var download = await altBlobClient.DownloadAsync();
                        var properties = await altBlobClient.GetPropertiesAsync();
                        return (download.Value.Content, properties.Value.ContentType);
                    }
                }

                // List blobs to see what's actually in the container
               // await ListBlobsForDebugging();

                return (null, null);
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"[ERROR] GetImageStreamAsync failed: {ex.Message}");
                //Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                return (null, null);
            }
        }


    }
}