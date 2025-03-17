using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(HotelDb context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Проверяем наличие ролей
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            // Проверяем наличие пользователей
            if (!userManager.Users.Any())
            {
                await userManager.CreateAsync(new User
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    FullName = "Морозов Алексей Алексеевич",
                    Passport = "1234567890",
                    Discount = 10
                }, "Admin@123");

                await userManager.CreateAsync(new User
                {
                    UserName = "user",
                    Email = "user@example.com",
                    FullName = "Иванов Иван Иванович",
                    Passport = "0987654321",
                    Discount = 5
                }, "User@123");
            }

            // Загружаем пользователей после создания
            var adminUser = await userManager.FindByNameAsync("admin");
            var normalUser = await userManager.FindByNameAsync("user");

            // Проверяем наличие бронирований
            if (!context.Reservations.Any())
            {
                var reservations = new List<Reservation>
                {
                    new Reservation
                    {
                        ArrivalDate = DateTime.Now,
                        DepartureDate = DateTime.Now.AddDays(7),
                        FullPrice = 50000,
                        RoomID = 1,
                        UserID = adminUser.Id
                    },
                    new Reservation
                    {
                        ArrivalDate = DateTime.Now.AddDays(10),
                        DepartureDate = DateTime.Now.AddDays(15),
                        FullPrice = 25000,
                        RoomID = 2,
                        UserID = normalUser.Id
                    }
                };

                await context.Reservations.AddRangeAsync(reservations);
                await context.SaveChangesAsync();
            }
        }

    }
}