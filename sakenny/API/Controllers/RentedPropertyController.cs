using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentingController : ControllerBase
    {
        private readonly IRentedPropertyService _rentingService;

        public RentingController(IRentedPropertyService rentingService)
        {
            _rentingService = rentingService;
        }

        [HttpGet("/RentedProperties/{userId}")]
        public async Task<ActionResult<IEnumerable<RentedPropertyDTO>>> GetRentedPropertiesByUser(string userId)
        {
            var rentedProperties = await _rentingService.GetRentedPropertiesByUserAsync(userId);
            return Ok(rentedProperties);
        }
    }
}
