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
        private readonly ILogger<AdditionalServiceController> _logger;

        public AdditionalServiceController(AdditionalServiceService service, ILogger<AdditionalServiceController> logger)
        {
            _service = service;
            _logger = logger;
        }
        /// <summary>
        /// Метод получения всех дополнительных услуг
        /// </summary>
        /// <returns>Список всех услуг в базе данных</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdditionalServiceDTO>>> GetAdditionalServices()
        {
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userId} получает список всех дополнительных услуг");
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
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userId} получает дополнительную услугу с идентификатором {id}");
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
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            var serviceDTO = await _service.AddAdditionalServiceAsync(createServiceDTO);
            _logger.LogInformation($"Пользователь {userId} создал дополнительную услугу {serviceDTO.Name}" +
                $" с идентификатором {serviceDTO.ID}");
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

            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userId} обновляет дополнительную услугу с идентификатором {id}");
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
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userId} удаляет дополнительную услугу с идентификатором {id}");
            try
            {
                await _service.DeleteAdditionalServiceAsync(id);
                return NoContent(); ;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Ошибка при удалении дополнительной услуги с идентификатором {id}: {ex.Message}");
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
