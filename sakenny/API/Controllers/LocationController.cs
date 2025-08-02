using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sakenny.Application.Interfaces;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _locationService.GetLocationsAsync();
            return Ok(result);
        }
    }
}
