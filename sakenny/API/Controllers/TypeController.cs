using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeController : ControllerBase
    {
        private readonly IPropertyTypeService _propertyTypeService;

        public TypeController(IPropertyTypeService propertyTypeService)
        {
            _propertyTypeService = propertyTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _propertyTypeService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddTypeDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _propertyTypeService.AddTypeAsync(dto);
            return Ok(new { message = "Property type added successfully" });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateTypeDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _propertyTypeService.UpdateTypeAsync(dto);
                return Ok(new { message = "Property type updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _propertyTypeService.DeleteTypeAsync(id);
                return Ok(new { message = "Property type deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }

}
