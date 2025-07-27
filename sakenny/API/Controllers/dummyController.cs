using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sakenny.DAL;
using sakenny.Models;
using System.Security.Claims;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class dummyController : ControllerBase
    {
        ApplicationDBContext db;

        public dummyController(ApplicationDBContext db)
        {
            this.db = db;
        }
        [HttpPost]
        public IActionResult Post([FromBody] string _description)
        {
            
            var dum = new DummyTable
            {
                description = _description,
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
             };
            db.Add(dum);
            db.SaveChanges();
            if (string.IsNullOrEmpty(_description))
            {
                return BadRequest("Description cannot be null or empty.");
            }
            // Simulate saving the description
            // SaveDescriptionToDatabase(description);
            return Ok("Description submitted successfully.");
        }
    }
}
