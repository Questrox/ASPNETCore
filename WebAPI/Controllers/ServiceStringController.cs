using Application.DTOs;
using Application.Services;
using Infrastructure.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var serviceStringDTO = await _serviceStringService.AddServiceStringAsync(createServiceStringDTO);
                var fullDTO = await _serviceStringService.GetServiceStringByIdAsync(serviceStringDTO.ID); //Чтобы вернулся со всеми связанными объектами
                if (fullDTO == null)
                    return NotFound();
                return CreatedAtAction(nameof(GetServiceString), new { id = fullDTO.ID }, fullDTO);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Произошла внутренняя ошибка сервера." });
            }
        }
        [Authorize(Roles = "admin")]
        [HttpPut("deliver")]
        public async Task<ActionResult<ServiceStringDTO>> DeliverService(int serviceStringID, int amount)
        {
            try
            {
                return await _serviceStringService.DeliverServiceAsync(serviceStringID, amount);
            } 
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Произошла внутренняя ошибка сервера." });
            }
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
