using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы с данными о пользователях
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(UserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        
        /// <summary>
        /// Метод для получения списка всех пользователей
        /// </summary>
        /// <returns>Список пользователей</returns>
        // GET: api/<UserController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает список всех пользователей");
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }
        /// <summary>
        /// Метод для получения пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Пользователя или NotFound, если пользователь не найден</returns>
        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(string id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает пользователя по идентификатору {id}");
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
        /// <summary>
        /// Метод для создания пользователя
        /// </summary>
        /// <param name="userCreateDTO">Создаваемый пользователь</param>
        /// <returns>Созданный пользователь</returns>
        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser(CreateUserDTO userCreateDTO)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создает пользователя");
            var userDTO = await _userService.AddUserAsync(userCreateDTO);
            return CreatedAtAction(nameof(GetUser), new { id = userDTO.Id }, userDTO);
        }

        /// <summary>
        /// Метод для обновления пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="userDTO">Сам пользователь</param>
        /// <returns>Обновленный пользователь или NotFound, если не обновится</returns>
        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDTO>> UpdateUser(string id, UserDTO userDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != userDTO.Id) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет пользователя с идентификатором {id}");

            var updatedUser = await _userService.UpdateUserAsync(userDTO);

            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }

        /// <summary>
        /// Метод для удаления пользователя
        /// </summary>
        /// <param name="id">Идентификатор удаляемого пользователя</param>
        /// <returns>NoContent или ошибка с сообщением</returns>
        // DELETE api/<UserController>/5
        //[Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет пользователя с идентификатором {id}");
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Ошибка при удалении пользователя с идентификатором {id}: {ex.Message}");
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
