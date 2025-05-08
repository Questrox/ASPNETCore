using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для управления пользователями в системе. Содержит методы регистрации, входа и т.п.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, 
            IConfiguration configuration, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }
        /// <summary>
        /// Метод регистрации. Пытается создать пользователя на основе модели
        /// </summary>
        /// <param name="model">Содержит ФИО, паспортные данные, логин и пароль </param>
        /// <returns>Статус 200 ОК или 400 Bad Request в случае ошибки</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = new User { UserName = model.UserName, FullName = model.FullName, Passport = model.Passport};
            _logger.LogInformation($"Попытка регистрации пользователя {model.UserName}");
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                _logger.LogInformation($"Пользователь {model.UserName} успешно зарегистрирован");
                return Ok(new { Message = "User registered successfully" });
            }
            foreach (var error in result.Errors)
                _logger.LogError($"Ошибка регистрации: {error.Code} {error.Description}");
            return BadRequest(result.Errors);
        }
        /// <summary>
        /// Метод входа. Пытается войти в систему на основе данных модели
        /// </summary>
        /// <param name="model">Содержит логин и пароль</param>
        /// <returns>Статус ОК со сгенерированным JWT-токеном, именем пользователя и ролью или же Unauthorized в случае неверных данных</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation($"Попытка входа пользователя {model.UserName}");
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var token = GenerateJwtToken(user);
                IList<string> roles = await _userManager.GetRolesAsync(user);
                string userRole = roles.FirstOrDefault();
                _logger.LogInformation($"Пользователь {model.UserName} успешно вошел в систему");
                return Ok(new { token, userName = user.UserName, userRole });
            }
            _logger.LogError($"Пользователю {model.UserName} не удалось выполнить вход");
            return Unauthorized();
        }
        /// <summary>
        /// Выполняет выход из системы
        /// </summary>
        /// <returns>Статус 200 ОК и сообщение</returns>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userId} выполняет выход из системы");
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "User logged out successfully" });
        }
        /// <summary>
        /// Проверяет токен аутентификации пользователя
        /// </summary>
        /// <returns>Unauthorized, если пользователь не аутентифицирован, и OK с именем и ролью, если аутентифицирован</returns>
        [HttpGet("validate")]
        public async Task<IActionResult> ValidateToken()
        {
            _logger.LogInformation("Происходит валидация токена");
            User usr = await _userManager.GetUserAsync(HttpContext.User);
            if (usr == null)
            {
                _logger.LogWarning("Пользователь не аутентифицирован");
                return Unauthorized(new { message = "Вы Гость. Пожалуйста, выполните вход" });
            }
            _logger.LogInformation($"Сессия активна для пользователя {usr.UserName}");
            IList<string> roles = await _userManager.GetRolesAsync(usr);
            string userRole = roles.FirstOrDefault();
            return Ok(new { message = "Сессия активна", userName = usr.UserName, userRole });

        }
        /// <summary>
        /// Генерирует JWT-токен для пользователя
        /// </summary>
        /// <param name="user">Пользователь, для которого генерируется токен</param>
        /// <returns>Сгенерированный JWT-токен</returns>
        private string GenerateJwtToken(User user)
        {
            _logger.LogInformation($"Генерируется токен для пользователя {user.UserName}");
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = _userManager.GetRolesAsync(user).Result;
            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
