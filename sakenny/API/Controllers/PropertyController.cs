using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sakenny.Application.DTO;
using sakenny.Application.DTO.sakenny.DAL.DTOs;
using sakenny.Application.Interfaces;
using System.Security.Claims;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Host,User")]

    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;
        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }
        [HttpPost]
        [Authorize(Roles = "Host")]
        [Route("/AddProperty")]
        public async Task<IActionResult> AddProperty([FromForm] AddPropertyDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            if (ModelState.IsValid)
            {
                try
                {
                    var Property = await _propertyService.AddPropertyAsync(model, userId);
                    return Ok(Property);
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
            }
            return BadRequest("Can't add property");
        }



        [HttpPut]
        [Authorize(Roles = "Host")]
        [Route("/UpdateProperty/{id}")]
        public async Task<IActionResult> UpdateProperty(int id, [FromForm] UpdatePropertyDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedProperty = await _propertyService.UpdatePropertyAsync(id, model, userId);
                return Ok(updatedProperty);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"DB update error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{id}")]
        public async Task<IActionResult> ViewProperty(int id)
        {
            try
            {
                var property = await _propertyService.GetPropertyDetailsAsync(id);
                return Ok(property);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpGet("/owned-properties/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOwnedProperties(string userId)
        {
            var properties = await _propertyService.GetUserOwnedPropertiesAsync(userId);

            if (!properties.Any())
            {
                return Ok(new List<OwnedPropertyDTO>());
            }

            return Ok(properties);
        }
        [HttpGet("/owned-properties")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> GetOwnedProperties(int PageNumber = 1, int PageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var properties = await _propertyService.GetUserOwnedPropertiesPagedAsync(userId, PageNumber, PageSize);

            if (!properties.Any())
            {
                return Ok(new { message = "No properties added yet." });
            }

            return Ok(properties);
        }

        [HttpGet("top-rated")]
        [AllowAnonymous]
        public async Task<ActionResult<List<HomeTopRatedPropertyDTO>>> GetTopRatedProperties()
        {
            var properties = await _propertyService.GetTopRatedPropertiesAsync();
            return Ok(properties);
        }

        [HttpPost("filter")]
        [AllowAnonymous]
        public async Task<IActionResult> Filter([FromBody] PropertyFilterDTO filterDto)
        {
           
            var result = await _propertyService.GetFilteredPropertiesAsync(filterDto);
            return Ok(result);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProperties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(properties);
        }

    }
}
