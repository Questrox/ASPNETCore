using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

            #region Пользователи
            // Проверяем наличие ролей
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            // Проверяем наличие пользователей
            if (!userManager.Users.Any())
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    FullName = "Морозов Алексей Алексеевич",
                    Passport = "1234567890",
                    Discount = 10
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "admin");

                var user = new User
                {
                    UserName = "user",
                    Email = "user@example.com",
                    FullName = "Иванов Иван Иванович",
                    Passport = "0987654321",
                    Discount = 5
                };
                await userManager.CreateAsync(user, "User@123");
                await userManager.AddToRoleAsync(user, "user");
            }
            #endregion

            #region Категории номеров
            if (!context.RoomCategories.Any())
            {
                await using var transaction = await context.Database.BeginTransactionAsync(); //Для вставки данных с ID
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT RoomCategories ON");
                var categories = new List<RoomCategory>
                {
                    new RoomCategory
                    {
                        ID = 1,
                        Category = "Стандарт"
                    },
                    new RoomCategory
                    {
                        ID = 2,
                        Category = "Комфорт"
                    },
                    new RoomCategory
                    {
                        ID = 3,
                        Category = "Полулюкс"
                    },
                    new RoomCategory
                    {
                        ID = 4,
                        Category = "Люкс"
                    }
                };
                await context.RoomCategories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT RoomCategories OFF");
                await transaction.CommitAsync();
            }
            #endregion

            #region Типы номеров
            if (!context.RoomTypes.Any())
            {
                await using var transaction = await context.Database.BeginTransactionAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT RoomTypes ON");
                var types = new List<RoomType>
                {
                    new RoomType
                    {
                        ID = 1,
                        GuestCapacity = 1,
                        RoomCategoryID = 1,
                        Price = 2000,
                        Description = "Уютный номер для одного человека, обеспечивающий комфорт и практичность для отдыха или работы."
                    },
                    new RoomType
                    {
                        ID = 2,
                        GuestCapacity = 1,
                        RoomCategoryID = 2,
                        Price = 2500,
                        Description = "Просторный номер с улучшенным дизайном, идеально подходит для деловых поездок и отдыха."
                    },
                    new RoomType
                    {
                        ID = 3,
                        GuestCapacity = 2,
                        RoomCategoryID = 2,
                        Price = 3200,
                        Description = "Комфортный номер для пар или друзей, предлагающий современный интерьер и уютную атмосферу."
                    },
                    new RoomType
                    {
                        ID = 4,
                        GuestCapacity = 1,
                        RoomCategoryID = 3,
                        Price = 3500,
                        Description = "Элегантный номер с дополнительным пространством для отдыха, обеспечивает высокий уровень комфорта."
                    },
                };
                await context.RoomTypes.AddRangeAsync(types);
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT RoomTypes OFF");
                await transaction.CommitAsync();
            }
            #endregion

            #region Номера
            if (!context.Rooms.Any())
            {
                await using var transaction = await context.Database.BeginTransactionAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Rooms ON");
                var rooms = new List<Room>
                {
                    new Room
                    {
                        ID = 1,
                        Number = 101,
                        RoomTypeID = 1
                    },
                    new Room
                    {
                        ID = 2,
                        Number = 102,
                        RoomTypeID = 1
                    },
                    new Room
                    {
                        ID = 3,
                        Number = 103,
                        RoomTypeID = 2
                    },
                    new Room
                    {
                        ID = 4,
                        Number = 104,
                        RoomTypeID = 2
                    },
                    new Room
                    {
                        ID = 5,
                        Number = 201,
                        RoomTypeID = 3
                    }
                };
                await context.Rooms.AddRangeAsync(rooms);
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Rooms OFF");
                await transaction.CommitAsync();
            }
            #endregion

            #region Статусы бронирования
            if (!context.ReservationStatuses.Any())
            {
                await using var transaction = await context.Database.BeginTransactionAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT ReservationStatuses ON");
                var statuses = new List<ReservationStatus>
                {
                    new ReservationStatus
                    {
                        ID = 1,
                        Status = "Ожидание"
                    },
                    new ReservationStatus
                    {
                        ID = 2,
                        Status = "Оплачено проживание"
                    },
                    new ReservationStatus
                    {
                        ID = 3,
                        Status = "Оплачено полностью"
                    },
                    new ReservationStatus
                    {
                        ID = 4,
                        Status = "Отменено"
                    }
                };
                await context.ReservationStatuses.AddRangeAsync(statuses);
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT ReservationStatuses OFF");
                await transaction.CommitAsync();
            }
            #endregion

            #region Статусы доп. услуг
            if (!context.ServiceStatuses.Any())
            {
                await using var transaction = await context.Database.BeginTransactionAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT ServiceStatuses ON");
                var servStatuses = new List<ServiceStatus>
                {
                    new ServiceStatus
                    {
                        ID = 1,
                        Status = "Ожидание"
                    },
                    new ServiceStatus
                    {
                        ID = 2,
                        Status = "Оказана"
                    },
                    new ServiceStatus
                    {
                        ID = 3,
                        Status = "Отменена"
                    }
                };
                await context.ServiceStatuses.AddRangeAsync(servStatuses);
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT ServiceStatuses OFF");
                await transaction.CommitAsync();
            }

            #endregion

            #region Доп. услуги
            if (!context.AdditionalServices.Any())
            {
                await using var transaction = await context.Database.BeginTransactionAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT AdditionalServices ON");
                var services = new List<AdditionalService>
                {
                    new AdditionalService
                    {
                        ID = 1,
                        Name = "Одноразовое питание",
                        Price = 800
                    },
                    new AdditionalService
                    {
                        ID = 2,
                        Name = "Двухразовое питание",
                        Price = 1400
                    },
                    new AdditionalService
                    {
                        ID = 3,
                        Name = "Трехразовое питание",
                        Price = 1900
                    },
                    new AdditionalService
                    {
                        ID = 4,
                        Name = "Посещение бассейна",
                        Price = 600
                    },
                    new AdditionalService
                    {
                        ID = 5,
                        Name = "Посещение сауны",
                        Price = 400
                    }
                };
                await context.AdditionalServices.AddRangeAsync(services);
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT AdditionalServices OFF");
                await transaction.CommitAsync();
            }
            #endregion

            #region Бронирования
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
                        LivingPrice = 50000,
                        ServicesPrice = 0,
                        RoomID = 1,
                        UserID = adminUser.Id,
                        ReservationStatusID = 4
                    },
                    new Reservation
                    {
                        ArrivalDate = DateTime.Now.AddDays(10),
                        DepartureDate = DateTime.Now.AddDays(15),
                        FullPrice = 25000,
                        LivingPrice = 25000,
                        ServicesPrice = 0,
                        RoomID = 1,
                        UserID = normalUser.Id,
                        ReservationStatusID = 4
                    }
                };

                await context.Reservations.AddRangeAsync(reservations);
                await context.SaveChangesAsync();
            }
            #endregion
        }
    }
}