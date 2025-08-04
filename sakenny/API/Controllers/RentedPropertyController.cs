using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using System.Security.Claims;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedInUserId == null)
                return Unauthorized();

            if (loggedInUserId != userId)
                return Forbid();

            var rentedProperties = await _rentingService.GetRentedPropertiesByUserAsync(userId);
            return Ok(rentedProperties);
        }
    }
}
