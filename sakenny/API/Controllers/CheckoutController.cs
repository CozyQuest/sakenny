using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sakenny.Application.Interfaces;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;
        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropertyById(int id)
        {
            try
            {
                var property = await _checkoutService.getPropertyByIdAsync(id);
                return Ok(property);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
