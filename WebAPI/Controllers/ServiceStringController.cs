using Application.DTOs;
using Application.Services;
using Infrastructure.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы со строками услуг
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceStringController : ControllerBase
    {
        private readonly ServiceStringService _serviceStringService;
        public ServiceStringController(ServiceStringService serviceStringService)
        {
            _serviceStringService = serviceStringService;
        }
        /// <summary>
        /// Метод для получения всех строк услуг
        /// </summary>
        /// <returns>Список строк</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceStringDTO>>> GetServiceStrings()
        {
            var serviceStrings = await _serviceStringService.GetServiceStringsAsync();
            return Ok(serviceStrings);
        }
        /// <summary>
        /// Метод для получение строки по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор строки</param>
        /// <returns>Строку или NotFound, если такой нет</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceStringDTO>> GetServiceString(int id)
        {
            var serviceString = await _serviceStringService.GetServiceStringByIdAsync(id);
            if (serviceString == null) return NotFound();
            return Ok(serviceString);
        }
        /// <summary>
        /// Метод для создания строки услуг
        /// </summary>
        /// <param name="createServiceStringDTO">Создаваемая строка</param>
        /// <returns>Созданная строка или ошибка с сообщением</returns>
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
        /// <summary>
        /// Метод для оказания доп.услуги. Увеличивает оказанное количество на оказываемое количество
        /// </summary>
        /// <param name="serviceStringID">Оказываемая услуга</param>
        /// <param name="amount">Оказываемое количество</param>
        /// <returns>Оказанная услуга или ошибка с сообщением</returns>
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
        /// <summary>
        /// Метод для обновления строки услуги
        /// </summary>
        /// <param name="id">Идентификатор обновляемой строки</param>
        /// <param name="serviceStringDTO">Сама строка</param>
        /// <returns>Обновленная строка или NotFound, если не обновится</returns>
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
        /// <summary>
        /// Метод для удаления строки услуг
        /// </summary>
        /// <param name="id">Идентификатор удаляемой строки</param>
        /// <returns>NoContent или же ошибка с сообщением</returns>
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
