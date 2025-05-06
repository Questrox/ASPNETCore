using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.InfrastructureTests
{
    public class HotelDbTests
    {
        private readonly DbContextOptions<HotelDb> _options;
        public HotelDbTests() 
        {
            _options = new DbContextOptionsBuilder<HotelDb>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [Fact]
        public async Task AddProjectAsync_ShouldSaveUser() //Проверка на то, что User сохраняется в БД
        {
            using (var context = new HotelDb(_options))
            {
                var user = new User { FullName = "TestName", Passport = "1234567890" };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            using (var context = new HotelDb(_options))
            {
                var user = await context.Users.FirstOrDefaultAsync(p => p.FullName == "TestName");
                Assert.NotNull(user);
                Assert.Equal("1234567890", user.Passport);
            }
        }
    }
}
