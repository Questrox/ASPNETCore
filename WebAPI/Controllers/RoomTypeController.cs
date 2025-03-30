using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly RoomTypeService _roomTypeService;
        public RoomTypeController(RoomTypeService roomTypeService)
        {
            _roomTypeService = roomTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTypeDTO>>> GetRoomTypes()
        {
            var roomTypes = await _roomTypeService.GetRoomTypesAsync();
            return Ok(roomTypes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomTypeDTO>> GetRoomType(int id)
        {
            var roomType = await _roomTypeService.GetRoomTypeByIdAsync(id);
            if (roomType == null) return NotFound();
            return Ok(roomType);
        }

        [HttpPost]
        public async Task<ActionResult<RoomTypeDTO>> CreateRoomType(CreateRoomTypeDTO createRoomTypeDTO)
        {
            var roomTypeDTO = await _roomTypeService.AddRoomTypeAsync(createRoomTypeDTO);
            return CreatedAtAction(nameof(GetRoomType), new { id = roomTypeDTO.ID }, roomTypeDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RoomTypeDTO>> UpdateRoomType(int id, RoomTypeDTO roomTypeDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != roomTypeDTO.ID) return BadRequest();

            var updatedRoomType = await _roomTypeService.UpdateRoomTypeAsync(roomTypeDTO);

            if (updatedRoomType == null)
                return NotFound();

            return Ok(updatedRoomType);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomType(int id)
        {
            try
            {
                await _roomTypeService.DeleteRoomTypeAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
