using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL.Models;
using System.Security.Claims;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }


        [HttpPost]
        [Route("/PostReview/{propertyId}")]
        public async Task<IActionResult> Post(int propertyId, [FromBody] PostReviewDTO review)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Unauthorized("You must log in first to post a review.");

                if (review == null)
                    return BadRequest("Review cannot be null.");

                await _reviewService.AddReviewAsync(propertyId, review, userId);

                return Ok("Review submitted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}. Inner: {ex.InnerException?.Message}");
            }
        }


    }
}

