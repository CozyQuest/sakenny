using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
    }
}
