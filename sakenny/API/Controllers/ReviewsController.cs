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
        [Route("/PostRating/{propertyId}")]
        public async Task<IActionResult> PostRating(int propertyId, [FromBody] PostRatingDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Unauthorized("You must log in first to post a rating.");

                if (dto == null)
                    return BadRequest("Rating data is required.");

                await _reviewService.AddOrUpdateRatingAsync(propertyId, dto, userId);

                return Ok("Rating submitted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("/PostReview/{propertyId}")]
        public async Task<IActionResult> PostReview(int propertyId, [FromBody] PostReviewDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Unauthorized("You must log in first to post a review.");

                if (dto == null)
                    return BadRequest("Review data is required.");

                await _reviewService.AddReviewAsync(propertyId, dto, userId);

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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpGet("{propertyId}")]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviewsForProperty(int propertyId)
        {
            try
            {
                var result = await _reviewService.GetReviewsForPropertyAsync(propertyId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while fetching reviews.");
            }
        }


    }
}

