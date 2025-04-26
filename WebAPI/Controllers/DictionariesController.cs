using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionariesController : ControllerBase
    {
        private readonly DictionariesService _dicService;
        public DictionariesController(DictionariesService dicService)
        {
            _dicService = dicService;
        }

        [HttpGet("roomCategories")]
        public async Task<ActionResult<IEnumerable<ReservationStatusDTO>>> GetRoomCategories()
        {
            var roomCategories = await _dicService.GetRoomCategoriesAsync();
            return Ok(roomCategories);
        }
        [HttpGet("serviceStatuses")]
        public async Task<ActionResult<IEnumerable<ServiceStatusDTO>>> GetServiceStatuses()
        {
            var serviceStatuses = await _dicService.GetServiceStatusesAsync();
            return Ok(serviceStatuses);
        }
        [HttpGet("reservationStatuses")]
        public async Task<ActionResult<IEnumerable<RoomCategoryDTO>>> GetReservationStatuses()
        {
            var resStatuses = await _dicService.GetReservationStatusesAsync();
            return Ok(resStatuses);
        }
    }
}
