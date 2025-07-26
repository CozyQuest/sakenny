using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sakenny.DAL.Models;

namespace sakenny.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] Review review)
        {
            // Here you would typically save the review to a database or perform some action with it.
            // For this example, we will just return a success message.
            if (review == null)
            {
                return BadRequest("Review cannot be null.");
            }
            // Simulate saving the review
            // SaveReviewToDatabase(review);
            return Ok("Review submitted successfully.");
        }
    }
}
