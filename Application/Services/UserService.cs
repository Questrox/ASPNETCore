using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            return users.Select(x => new UserDTO(x));
        }

        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return null;
            return new UserDTO(user);
        }

        public async Task<UserDTO> AddUserAsync(CreateUserDTO userCreateDTO)
        {
            var newUser = new User
            {
                FullName = userCreateDTO.FullName,
                Passport = userCreateDTO.Passport,
                Discount = userCreateDTO.Discount
            };

            await _userRepository.AddUserAsync(newUser);

            return new UserDTO(newUser);
        }
        public async Task<UserDTO?> UpdateUserAsync(UserDTO userDTO)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(userDTO.Id);
            if (existingUser == null) return null;

            existingUser.FullName = userDTO.FullName;
            existingUser.Passport = userDTO.Passport;
            existingUser.Discount = userDTO.Discount;

            await _userRepository.UpdateUserAsync(existingUser);

            return new UserDTO(existingUser); // Возвращаем обновленный объект
        }

        public async Task DeleteUserAsync(string id)
        {
            await _userRepository.DeleteUserAsync(id);
        }
    }
}
