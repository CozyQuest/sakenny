using Microsoft.AspNetCore.Mvc;
using sakenny.Application.DTO;
using sakenny.Application.Services;


namespace sakenny.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IPropertyServicesService _service;

        public ServicesController(IPropertyServicesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _service.GetAllAsync();
            return Ok(services);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddServiceDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _service.AddServiceAsync(dto);
            return Ok("Service added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateServiceDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _service.UpdateServiceAsync(dto);
                return Ok("Service updated successfully.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteServiceAsync(id);
                return Ok("Service deleted successfully.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}

