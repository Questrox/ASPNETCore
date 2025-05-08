using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы с бронированиями
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationService _resService;
        private readonly ILogger<ReservationController> _logger;
        public ReservationController(ReservationService resService, ILogger<ReservationController> logger)
        {
            _resService = resService;
            _logger = logger;
        }
        
        /// <summary>
        /// Рассчитывает стоимость бронирования с учетом доп.услуг
        /// </summary>
        /// <param name="req">Содержит даты заезда, выезда, тип номера и список доп.услуг</param>
        /// <returns>Цену или ошибку с сообщением</returns>
        [HttpPost("calculatePrice")]
        public async Task<ActionResult<decimal>> CalculatePrice([FromBody] CalculatePriceModel req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userId} рассчитывает промежуточную цену своего бронирования (метод CalculatePrice)");
            try
            {
                decimal price = await _resService.CalculatePriceAsync(
                req.ArrivalDate,
                req.DepartureDate,
                req.RoomTypeID,
                req.Services);
                _logger.LogInformation($"Расчет цены для пользователя {userId} прошел успешно, текущая цена: {price}");

                return Ok(price);
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
        /// Метод для администратора, подтверждает оплату доп.услуг бронирования (уже после оплаты проживания)
        /// </summary>
        /// <param name="resDTO">Бронирование, для которого происходит подтверждение</param>
        /// <returns>Обновленное бронирование</returns>
        [Authorize(Roles = "admin")]
        [HttpPut("confirmPayment")]
        public async Task<ActionResult<ReservationDTO>> ConfirmPayment(ReservationDTO resDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userId} подтверждает оплату доп.услуг бронирования с идентификатором {resDTO.ID}");
            try
            {
                return await _resService.ConfirmPayment(resDTO);
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
        /// Позволяет получить бронирования пользователя по паспорту
        /// </summary>
        /// <param name="passport">Паспортные данные</param>
        /// <returns>Список бронирований пользователя</returns>
        [Authorize(Roles = "admin")]
        [HttpGet("passportReservations")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservationsByPassport(string passport)
        {
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userId} получает все бронирования пользователя с паспортом {passport}");
            var reservs = await _resService.GetReservationsByPassportAsync(passport);
            return Ok(reservs);
        }
        /// <summary>
        /// Позволяет получить все бронирования
        /// </summary>
        /// <returns>Список всех бронирований</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservations()
        {
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userId} получает список всех бронирований");
            var reservs = await _resService.GetReservationsAsync();
            return Ok(reservs);
        }
        /// <summary>
        /// Позволяет получить бронирования пользователя, который вошел в систему
        /// </summary>
        /// <returns>Список бронирований текущего пользователя</returns>
        [Authorize]
        [HttpGet("userReservations")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservationsForUser()
        {
            var userName = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает список всех своих бронирований");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var reservs = await _resService.GetReservationsForUserAsync(userId);
            return Ok(reservs);
        }
        /// <summary>
        /// Получает бронирование по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Бронирование или же NotFound</returns>
        // GET api/<ReservationController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservation(int id)
        {
            var userName = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает бронирование по идентификатору {id}");
            var res = await _resService.GetReservationByIdAsync(id);
            if (res == null) return NotFound();
            return Ok(res);
        }

        /// <summary>
        /// Создает бронирование для текущего пользователя
        /// </summary>
        /// <param name="createReservationDTO">Создаваемое бронирование</param>
        /// <returns>Созданное бронирование или ошибку с сообщением</returns>
        // POST api/<ReservationController>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReservationDTO>> CreateReservation([FromBody] CreateReservationDTO createReservationDTO)
        {
            var userName = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создает бронирование");
            try
            {
                createReservationDTO.UserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var resDTO = await _resService.AddReservationAsync(createReservationDTO);
                return CreatedAtAction(nameof(GetReservation), new { id = resDTO.ID }, resDTO);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Произошла внутренняя ошибка сервера." + e.Message});
            }
        }
        /// <summary>
        /// Обновляет бронирование
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <param name="resDTO">Само бронирование</param>
        /// <returns>Обновленное бронирование или ошибку</returns>
        // PUT api/<ReservationController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ReservationDTO>> UpdateReservation(int id, ReservationDTO resDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != resDTO.ID) return BadRequest();

            var userName = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет бронирование с идентификатором {id}");

            var updatedRes = await _resService.UpdateReservationAsync(resDTO);

            if (updatedRes == null)
                return NotFound();

            return Ok(updatedRes);
        }

        /// <summary>
        /// Удаляет бронирование
        /// </summary>
        /// <param name="id">Идентификатор удаляемого бронирования</param>
        /// <returns>NoContent или ошибку</returns>
        // DELETE api/<ReservationController>/5
        //[Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var userName = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет бронирование с идентификатором {id}");
            try
            {
                await _resService.DeleteReservationAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Ошибка при удалении бронирования с идентификатором {id}: {ex.Message}");
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
