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
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        
        /// <summary>
        /// Метод для получения списка всех пользователей
        /// </summary>
        /// <returns>Список пользователей</returns>
        // GET: api/<UserController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
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
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
