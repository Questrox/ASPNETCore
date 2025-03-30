using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationService _resService;
        public ReservationController(ReservationService resService)
        {
            _resService = resService;
        }
        // GET: api/<ReservationController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservations()
        {
            var reservs = await _resService.GetReservationsAsync();
            return Ok(reservs);
        }

        // GET api/<ReservationController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservation(int id)
        {
            var res = await _resService.GetReservationByIdAsync(id);
            if (res == null) return NotFound();
            return Ok(res);
        }

        // POST api/<ReservationController>
        [HttpPost]
        public async Task<ActionResult<ReservationDTO>> CreateUser(CreateReservationDTO createReservationDTO)
        {
            var resDTO = await _resService.AddReservationAsync(createReservationDTO);
            return CreatedAtAction(nameof(GetReservation), new { id = resDTO.ID }, resDTO);
        }

        // PUT api/<ReservationController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ReservationDTO>> UpdateUser(int id, ReservationDTO resDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != resDTO.ID) return BadRequest();

            var updatedRes = await _resService.UpdateReservationAsync(resDTO);

            if (updatedRes == null)
                return NotFound();

            return Ok(updatedRes);
        }

        // DELETE api/<ReservationController>/5
        //[Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                await _resService.DeleteReservationAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
