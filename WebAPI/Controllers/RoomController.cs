using Application.DTOs;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы с номерами/комнатами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly RoomService _roomService;
        private readonly ILogger<RoomController> _logger;
        public RoomController(RoomService roomService, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }
        /// <summary>
        /// Метод для получения всех комнат
        /// </summary>
        /// <returns>Список всех комнат</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetRooms()
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает список всех комнат.");
            var rooms = await _roomService.GetRoomsAsync();
            return Ok(rooms);
        }
        /// <summary>
        /// Метод для получения комнат с пагинацией
        /// </summary>
        /// <param name="page">Номер страницы</param>
        /// <param name="pageSize">Количество комнат на странице</param>
        /// <returns>Список комнат, который будет отображен на странице, а также общее число комнат</returns>
        [HttpGet("pagination")]
        public async Task<ActionResult<PagedResult<RoomDTO>>> GetPaginatedRooms([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает комнаты с пагинацией. Страница: {page}, размер страницы: {pageSize}.");
            var rooms = await _roomService.GetPaginatedRoomsAsync(page, pageSize);
            return Ok(rooms);
        }
        /// <summary>
        /// Метод для получения доступных комнат в зависимости от параметров
        /// </summary>
        /// <param name="arrivalDate">Дата приезда</param>
        /// <param name="departureDate">Дата отъезда</param>
        /// <param name="roomTypeID">Идентификатор типа комнаты</param>
        /// <returns>Список комнат данного типа, свободных в данный период</returns>
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAvailableRooms(DateTime arrivalDate, DateTime departureDate, int roomTypeID)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} запрашивает доступные комнаты типа {roomTypeID} " +
                $"на период с {arrivalDate:dd-MM-yyyy} по {departureDate:dd-MM-yyyy}.");
            var rooms = await _roomService.GetAvailableRoomsAsync(arrivalDate, departureDate, roomTypeID);
            return Ok(rooms);
        }
        /// <summary>
        /// Метод для получения комнаты по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор комнаты</param>
        /// <returns>Комнату или NotFound</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDTO>> GetRoom(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает комнату с идентификатором {id}.");
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound();
            return Ok(room);
        }
        /// <summary>
        /// Метод для создания комнаты
        /// </summary>
        /// <param name="createRoomDTO">Создаваемая комната</param>
        /// <returns>Созданную комнату</returns>
        [HttpPost]
        public async Task<ActionResult<RoomDTO>> CreateRoom(CreateRoomDTO createRoomDTO)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создает новую комнату с номером {createRoomDTO.Number}");
            var roomDTO = await _roomService.AddRoomAsync(createRoomDTO);
            return CreatedAtAction(nameof(GetRoom), new { id = roomDTO.ID }, roomDTO);
        }
        /// <summary>
        /// Метод для обновления комнаты
        /// </summary>
        /// <param name="id">Идентификатор комнаты</param>
        /// <param name="roomDTO">Сама комната</param>
        /// <returns>Обновленная комната</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<RoomDTO>> UpdateRoom(int id, RoomDTO roomDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != roomDTO.ID) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет комнату с идентификатором {id}.");

            var updatedRoom = await _roomService.UpdateRoomAsync(roomDTO);

            if (updatedRoom == null)
                return NotFound();

            return Ok(updatedRoom);
        }
        /// <summary>
        /// Метод для удаления комнаты
        /// </summary>
        /// <param name="id">Идентификатор удаляемой комнаты</param>
        /// <returns>NoContent или ошибку с сообщением</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет комнату с идентификатором {id}.");
            try
            {
                await _roomService.DeleteRoomAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Ошибка при удалении комнаты с идентификатором {id}: {ex.Message}");
                return Conflict(new { message = ex.Message });
            }
        }
    }
}