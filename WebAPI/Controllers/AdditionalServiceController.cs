using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы с доп.услугами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AdditionalServiceController : ControllerBase
    {
        private readonly AdditionalServiceService _service;

        public AdditionalServiceController(AdditionalServiceService service)
        {
            _service = service;
        }
        /// <summary>
        /// Метод получения всех дополнительных услуг
        /// </summary>
        /// <returns>Список всех услуг в базе данных</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdditionalServiceDTO>>> GetAdditionalServices()
        {
            var services = await _service.GetAdditionalServicesAsync();
            return Ok(services);
        }
        /// <summary>
        /// Метод получения доп.услуги по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор услуги</param>
        /// <returns>Дополнительную услугу или же NotFound</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AdditionalServiceDTO>> GetAdditionalService(int id)
        {
            var service = await _service.GetAdditionalServiceByIdAsync(id);
            if (service == null) return NotFound();
            return Ok(service);
        }
        /// <summary>
        /// Метод создания доп.услуги
        /// </summary>
        /// <param name="createServiceDTO">Создаваемая услуга</param>
        /// <returns>Созданную доп.услугу</returns>
        [HttpPost]
        public async Task<ActionResult<AdditionalServiceDTO>> CreateAdditionalService(CreateAdditionalServiceDTO createServiceDTO)
        {
            var serviceDTO = await _service.AddAdditionalServiceAsync(createServiceDTO);
            return CreatedAtAction(nameof(GetAdditionalService), new { id = serviceDTO.ID }, serviceDTO);
        }
        /// <summary>
        /// Метод обновления доп.услуги
        /// </summary>
        /// <param name="id">Идентификатор обновляемой услуги</param>
        /// <param name="serviceDTO">Сама услуга</param>
        /// <returns>Обновленную услугу</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<AdditionalServiceDTO>> UpdateAdditionalService(int id, AdditionalServiceDTO serviceDTO)
        {
            if (id != serviceDTO.ID) return BadRequest();

            var updatedService = await _service.UpdateAdditionalServiceAsync(serviceDTO);
            if (updatedService == null)
                return NotFound();

            return Ok(updatedService);
        }
        /// <summary>
        /// Удаляет доп.услугу
        /// </summary>
        /// <param name="id">Идентификатор удаляемой услуги</param>
        /// <returns>NoContent</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdditionalService(int id)
        {
            await _service.DeleteAdditionalServiceAsync(id);
            return NoContent();
        }
    }
}
