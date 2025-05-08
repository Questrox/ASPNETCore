using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы с типами комнат
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly RoomTypeService _roomTypeService;
        private readonly ILogger<RoomTypeController> _logger;

        public RoomTypeController(RoomTypeService roomTypeService, ILogger<RoomTypeController> logger)
        {
            _roomTypeService = roomTypeService;
            _logger = logger;
        }
        /// <summary>
        /// Метод для получения списка всех типов комнат
        /// </summary>
        /// <returns>Список всех типов комнат</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTypeDTO>>> GetRoomTypes()
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает список всех типов комнат");
            var roomTypes = await _roomTypeService.GetRoomTypesAsync();
            return Ok(roomTypes);
        }
        /// <summary>
        /// Метод для получения типа комнаты по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор типа комнаты</param>
        /// <returns>Тип или NotFound</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomTypeDTO>> GetRoomType(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает тип комнаты с идентификатором {id}");
            var roomType = await _roomTypeService.GetRoomTypeByIdAsync(id);
            if (roomType == null) return NotFound();
            return Ok(roomType);
        }
        /// <summary>
        /// Метод для создания типа комнаты
        /// </summary>
        /// <param name="createRoomTypeDTO">Создаваемый тип</param>
        /// <returns>Созданный тип или NotFound, если не создастся</returns>
        [HttpPost]
        public async Task<ActionResult<RoomTypeDTO>> CreateRoomType(CreateRoomTypeDTO createRoomTypeDTO)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создает новый тип комнаты");
            var created = await _roomTypeService.AddRoomTypeAsync(createRoomTypeDTO);
            var fullDto = await _roomTypeService.GetRoomTypeByIdAsync(created.ID); //Чтобы вернулся вместе со связанными объектами
            if (fullDto == null)
                return NotFound();
            return CreatedAtAction(nameof(GetRoomType), new { id = fullDto.ID }, fullDto);
        }
        /// <summary>
        /// Метод для обновления типа комнаты
        /// </summary>
        /// <param name="id">Идентификатор типа</param>
        /// <param name="roomTypeDTO">Сам тип</param>
        /// <returns>Обновленный тип или NotFound, если не обновится</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<RoomTypeDTO>> UpdateRoomType(int id, RoomTypeDTO roomTypeDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != roomTypeDTO.ID) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет тип комнаты с идентификатором {id}");

            var updatedRoomType = await _roomTypeService.UpdateRoomTypeAsync(roomTypeDTO);

            if (updatedRoomType == null)
                return NotFound();

            return Ok(updatedRoomType);
        }
        /// <summary>
        /// Метод для удаления типа комнаты
        /// </summary>
        /// <param name="id">Идентификатор удаляемого типа</param>
        /// <returns>NoContent или ошибку с сообщением</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomType(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет тип комнаты с идентификатором {id}");
            try
            {
                await _roomTypeService.DeleteRoomTypeAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Ошибка при удалении типа комнаты с идентификатором {id}: {ex.Message}");
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
