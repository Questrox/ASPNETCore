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

        public async Task AddUserAsync(UserDTO user)
        {
            var newUser = new User
            {
                FullName = user.FullName,
                Passport = user.Passport,
                Discount = user.Discount,
            };
            await _userRepository.AddUserAsync(newUser);
        }

        public async Task UpdateUserAsync(UserDTO user)
        {
            var updUser = await _userRepository.GetUserByIdAsync(user.Id);
            if (updUser != null)
            {
                updUser.FullName = user.FullName;
                updUser.Passport = user.Passport;
                updUser.Discount = user.Discount;
                await _userRepository.UpdateUserAsync(updUser);
            }
        }
        public async Task DeleteUserAsync(string id)
        {
            await _userRepository.DeleteUserAsync(id);
        }
    }
}
