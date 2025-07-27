namespace sakenny.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<List<string>> UploadImagesAsync(IEnumerable<IFormFile> files);
        Task<(Stream? Content, string? ContentType)> GetImageStreamAsync(string url);
    }
}
