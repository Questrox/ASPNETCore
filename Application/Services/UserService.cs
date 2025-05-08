using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Сервис для управления пользователями
    /// </summary>
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        /// <summary>
        /// Получает список всех пользователей
        /// </summary>
        /// <returns>Список всех пользователей</returns>
        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            return users.Select(x => new UserDTO(x));
        }
        /// <summary>
        /// Получает пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Пользователь или null, если не найден</returns>
        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogError($"Не удалось найти пользователя с идентификатором {id}");
                return null;
            }
            return new UserDTO(user);
        }
        /// <summary>
        /// Добавляет нового пользователя
        /// </summary>
        /// <param name="userCreateDTO">Новый пользователь</param>
        /// <returns>Созданный пользователь</returns>
        public async Task<UserDTO> AddUserAsync(CreateUserDTO userCreateDTO)
        {
            var newUser = new User
            {
                FullName = userCreateDTO.FullName,
                Passport = userCreateDTO.Passport,
            };

            await _userRepository.AddUserAsync(newUser);

            return new UserDTO(newUser);
        }
        /// <summary>
        /// Обновляет данные пользователя
        /// </summary>
        /// <param name="userDTO">Обновляемый пользователь</param>
        /// <returns>Обновленный пользователь или null, если не найден</returns>
        public async Task<UserDTO?> UpdateUserAsync(UserDTO userDTO)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(userDTO.Id);
            if (existingUser == null)
            {
                _logger.LogError($"Пользователь с id {userDTO.Id} не найден, обновление не выполнено");
                return null;
            }

            existingUser.FullName = userDTO.FullName;
            existingUser.Passport = userDTO.Passport;

            await _userRepository.UpdateUserAsync(existingUser);

            return new UserDTO(existingUser); // Возвращаем обновленный объект
        }
        /// <summary>
        /// Удаляет пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        public async Task DeleteUserAsync(string id)
        {
            await _userRepository.DeleteUserAsync(id);
        }
    }
}
