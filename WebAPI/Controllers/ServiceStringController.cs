using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceStringController : ControllerBase
    {
        private readonly ServiceStringService _serviceStringService;
        public ServiceStringController(ServiceStringService serviceStringService)
        {
            _serviceStringService = serviceStringService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceStringDTO>>> GetServiceStrings()
        {
            var serviceStrings = await _serviceStringService.GetServiceStringsAsync();
            return Ok(serviceStrings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceStringDTO>> GetServiceString(int id)
        {
            var serviceString = await _serviceStringService.GetServiceStringByIdAsync(id);
            if (serviceString == null) return NotFound();
            return Ok(serviceString);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceStringDTO>> CreateServiceString(CreateServiceStringDTO createServiceStringDTO)
        {
            var serviceStringDTO = await _serviceStringService.AddServiceStringAsync(createServiceStringDTO);
            return CreatedAtAction(nameof(GetServiceString), new { id = serviceStringDTO.ID }, serviceStringDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceStringDTO>> UpdateServiceString(int id, ServiceStringDTO serviceStringDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != serviceStringDTO.ID) return BadRequest();

            var updatedServiceString = await _serviceStringService.UpdateServiceStringAsync(serviceStringDTO);

            if (updatedServiceString == null)
                return NotFound();

            return Ok(updatedServiceString);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceString(int id)
        {
            try
            {
                await _serviceStringService.DeleteServiceStringAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
