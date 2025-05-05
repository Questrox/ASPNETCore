using Application.DTOs;
using Application.Services;
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
        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }
        /// <summary>
        /// Метод для получения всех комнат
        /// </summary>
        /// <returns>Список всех комнат</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetRooms()
        {
            var rooms = await _roomService.GetRoomsAsync();
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
            try
            {
                await _roomService.DeleteRoomAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}