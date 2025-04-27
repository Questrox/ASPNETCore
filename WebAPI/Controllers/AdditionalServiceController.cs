using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdditionalServiceController : ControllerBase
    {
        private readonly AdditionalServiceService _service;

        public AdditionalServiceController(AdditionalServiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdditionalServiceDTO>>> GetAdditionalServices()
        {
            var services = await _service.GetAdditionalServicesAsync();
            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdditionalServiceDTO>> GetAdditionalService(int id)
        {
            var service = await _service.GetAdditionalServiceByIdAsync(id);
            if (service == null) return NotFound();
            return Ok(service);
        }

        [HttpPost]
        public async Task<ActionResult<AdditionalServiceDTO>> CreateAdditionalService(CreateAdditionalServiceDTO createServiceDTO)
        {
            var serviceDTO = await _service.AddAdditionalServiceAsync(createServiceDTO);
            return CreatedAtAction(nameof(GetAdditionalService), new { id = serviceDTO.ID }, serviceDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AdditionalServiceDTO>> UpdateAdditionalService(int id, AdditionalServiceDTO serviceDTO)
        {
            if (id != serviceDTO.ID) return BadRequest();

            var updatedService = await _service.UpdateAdditionalServiceAsync(serviceDTO);
            if (updatedService == null)
                return NotFound();

            return Ok(updatedService);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdditionalService(int id)
        {
            await _service.DeleteAdditionalServiceAsync(id);
            return NoContent();
        }
    }
}
