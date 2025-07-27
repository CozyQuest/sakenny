using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using sakenny;
using sakenny.DAL.DTO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using sakenny.DAL;


[ApiController]
[Route("api/[controller]")]
[Consumes("multipart/form-data")]
public class ImageController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadImageRequestDTO file)
    {
        if (file == null || file.File.Length == 0)
            return BadRequest("No file uploaded.");

        try
        {
            var imageUrl = await _imageService.UploadImageAsync(file.File);
            return Ok(new { imageUrl });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("upload-multiple")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadMultiple([FromForm] MultiUploadImageRequestDTO model)
    {
        if (model.Files == null || !model.Files.Any())
            return BadRequest("No files uploaded.");

        try
        {
            var imageUrls = await _imageService.UploadImagesAsync(model.Files);
            return Ok(new { imageUrls });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest($"One or more files are invalid: {ex.Message}");
        }
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetImage([FromQuery] string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return BadRequest("Image URL is required.");

        var (stream, contentType) = await _imageService.GetImageStreamAsync(url);
        if (stream == null)
            return NotFound("Image not found.");

        return File(stream, contentType ?? "application/octet-stream");
    }

}
