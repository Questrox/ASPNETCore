using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Контроллер, содержащий методы для получения справочников
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DictionariesController : ControllerBase
    {
        private readonly DictionariesService _dicService;
        private readonly ILogger<DictionariesController> _logger;
        public DictionariesController(DictionariesService dicService, ILogger<DictionariesController> logger)
        {
            _dicService = dicService;
            _logger = logger;
        }
        /// <summary>
        /// Метод для получения категорий комнат
        /// </summary>
        /// <returns>Все категории комнат</returns>
        [HttpGet("roomCategories")]
        public async Task<ActionResult<IEnumerable<ReservationStatusDTO>>> GetRoomCategories()
        {
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userId} получает список всех категорий комнат");
            var roomCategories = await _dicService.GetRoomCategoriesAsync();
            return Ok(roomCategories);
        }
        /// <summary>
        /// Метод для получения статусов услуг
        /// </summary>
        /// <returns>Все возможные статусы услуг</returns>
        [HttpGet("serviceStatuses")]
        public async Task<ActionResult<IEnumerable<ServiceStatusDTO>>> GetServiceStatuses()
        {
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userId} получает список всех статусов доп.услуг");
            var serviceStatuses = await _dicService.GetServiceStatusesAsync();
            return Ok(serviceStatuses);
        }
        /// <summary>
        /// Метод для получения статусов бронирований
        /// </summary>
        /// <returns>Все возможные статусы бронирований</returns>
        [HttpGet("reservationStatuses")]
        public async Task<ActionResult<IEnumerable<RoomCategoryDTO>>> GetReservationStatuses()
        {
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userId} получает список всех статусов бронирования");
            var resStatuses = await _dicService.GetReservationStatusesAsync();
            return Ok(resStatuses);
        }
        /// <summary>
        /// Метод для получения путей к изображениям для определенного типа комнаты
        /// </summary>
        /// <param name="roomTypeID">Идентификатор типа комнаты</param>
        /// <returns>Список сущностей RoomTypeImageDTO, в которых есть пути к изображениям для типа комнаты</returns>
        [HttpGet("images")]
        public async Task<ActionResult<IEnumerable<RoomTypeImageDTO>>> GetImages(int roomTypeID)
        {
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userId} получает список всех изображений для номера с идентификатором {roomTypeID}");
            var images = await _dicService.GetRoomTypeImagesAsync(roomTypeID);
            return Ok(images);
        }
    }
}
