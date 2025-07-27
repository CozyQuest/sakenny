using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sakenny.DAL.Interfaces;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServicesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _unitOfWork.Services.GetAllAsync();
            return Ok(services);
        }

        [HttpPost]
        public async Task<IActionResult> Add()
        {

        }

        [HttpPost]
        public async Task<IActionResult> Update()
        {

        }

        [HttpPost]
        public async Task<IActionResult> Delete()
        {

        }
    }
}
