using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly RoomService _roomService;
        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetRooms()
        {
            var rooms = await _roomService.GetRoomsAsync();
            return Ok(rooms);
        }
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAvailableRooms(DateTime arrivalDate, DateTime departureDate, int roomTypeID)
        {
            var rooms = await _roomService.GetAvailableRoomsAsync(arrivalDate, departureDate, roomTypeID);
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDTO>> GetRoom(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [HttpPost]
        public async Task<ActionResult<RoomDTO>> CreateRoom(CreateRoomDTO createRoomDTO)
        {
            var roomDTO = await _roomService.AddRoomAsync(createRoomDTO);
            return CreatedAtAction(nameof(GetRoom), new { id = roomDTO.ID }, roomDTO);
        }

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