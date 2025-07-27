using sakenny.DAL;

namespace sakenny.Services
{
    public class ImageService : IImageService
    {
        private readonly BlobService _blobService;

        public ImageService(BlobService blobService)
        {
            _blobService = blobService;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            return await _blobService.UploadImageAsync(file);
        }

        public async Task<List<string>> UploadImagesAsync(IEnumerable<IFormFile> files)
        {
            var urls = new List<string>();
            foreach (var file in files)
            {
                var url = await _blobService.UploadImageAsync(file);
                urls.Add(url);
            }
            return urls;
        }

        public async Task<(Stream? Content, string? ContentType)> GetImageStreamAsync(string url)
        {
            return await _blobService.GetImageStreamAsync(url);
        }
    }
}
