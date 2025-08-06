using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using Stripe;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IConfiguration _configuration;
        public CheckoutController(ICheckoutService checkoutService, IConfiguration configuration)
        {
            _checkoutService = checkoutService;
            _configuration = configuration;
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
        // stripe endPoint to get payment session
        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] RentingCheckoutDTO rentingCheckoutDTO )
        {
            if (rentingCheckoutDTO == null)
            {
                return BadRequest("RentingCheckoutDTO cannot be null.");
            }
            // Get user ID from claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            // Add validation
            if (rentingCheckoutDTO.StartDate >= rentingCheckoutDTO.EndDate)
            {
                return BadRequest("End date must be after start date.");
            }

            if (rentingCheckoutDTO.StartDate < DateTime.Today)
            {
                return BadRequest("Start date cannot be in the past.");
            }

            try
            {
                var checkoutUrl = await _checkoutService.CreateCheckoutSessionAsync(rentingCheckoutDTO, userId);

                return Ok(new
                {
                    CheckoutUrl = checkoutUrl,
                    Message = "Checkout session created successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Internal server error: {ex.Message}");
            }
        }
        //webhook endpoint for stripe payment success
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                // Read the request body
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var stripeSignature = Request.Headers["Stripe-Signature"].ToString();
                var endpointSecret = _configuration["Stripe:WebhookEndpointSecret"];

                // Validate required data
                if (string.IsNullOrEmpty(json))
                {
                    return BadRequest("Empty payload");
                }

                if (string.IsNullOrEmpty(stripeSignature))
                {
                    return BadRequest("Missing signature");
                }

                if (string.IsNullOrEmpty(endpointSecret))
                {
                    return StatusCode(500, "Webhook not properly configured");
                }

                // Process the webhook through the service
                var success = await _checkoutService.ProcessWebhookAsync(json, stripeSignature, endpointSecret);

                if (success)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Failed to process webhook");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex.Message}");
            }
        }
    }
}
