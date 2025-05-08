using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Application.Services;
using Domain.Entities;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace Tests.ApplicationTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _userService;
        
        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockUserRepository.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUsersAsync_ShouldReturnUsers() //Проверяем, что нам вернулись все 3 правильных пользователя 
        {
            // Arrange
            var users = new List<User>
            {
                new User { FullName = "User1", Passport = "1234567890" },
                new User { FullName = "User2", Passport = "0987654321" },
                new User { FullName = "User3", Passport = "1112223334"}
            };

            _mockUserRepository.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetUsersAsync();

            // Assert
            Assert.Equal(3, result.Count());
        }
    }
}
