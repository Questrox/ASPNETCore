using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Application.DTOs;
using Microsoft.Extensions.Logging;

namespace Tests.ApplicationTests
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _resRepoMock = new();
        private readonly Mock<IRoomTypeRepository> _roomTypeRepoMock = new();
        private readonly Mock<IRoomRepository> _roomRepoMock = new();
        private readonly Mock<IServiceStringRepository> _serviceStringRepoMock = new();
        private readonly Mock<IAdditionalServiceRepository> _addServiceRepoMock = new();
        private readonly Mock<ILogger<ReservationService>> _loggerMock = new();

        private readonly ReservationService _service;

        public ReservationServiceTests()
        {
            _service = new ReservationService(
                _resRepoMock.Object,
                _roomTypeRepoMock.Object,
                _roomRepoMock.Object,
                _serviceStringRepoMock.Object,
                _addServiceRepoMock.Object,
                _loggerMock.Object
            );
        }

        #region Тесты расчета цены

        [Fact]
        public async Task CalculatePriceAsync_ValidInput_ReturnsCorrectTotal() //Просто правильный расчет
        {
            // Arrange
            var arrival = new DateTime(2025, 5, 10);
            var departure = new DateTime(2025, 5, 15); // 5 дней
            int roomTypeId = 1;
            decimal roomPrice = 100;

            _roomTypeRepoMock //1 тип комнаты с айди 1 и ценой 100
                .Setup(r => r.GetRoomTypeByIdAsync(roomTypeId))
                .ReturnsAsync(new RoomType { ID = roomTypeId, Price = roomPrice });

            var services = new List<SelectedServiceItem>
            {
                new SelectedServiceItem { AdditionalServiceID = 10, Count = 2, Price = 50 },
                new SelectedServiceItem { AdditionalServiceID = 11, Count = 1, Price = 200 } //Неправильная цена
            };

            _addServiceRepoMock
                .Setup(r => r.GetAdditionalServiceByIdAsync(10))
                .ReturnsAsync(new AdditionalService { ID = 10, Price = 50 });

            _addServiceRepoMock
                .Setup(r => r.GetAdditionalServiceByIdAsync(11))
                .ReturnsAsync(new AdditionalService { ID = 11, Price = 30 });

            // Act
            var total = await _service.CalculatePriceAsync(arrival, departure, roomTypeId, services);

            // Assert
            Assert.Equal(630, total);
        }

        [Fact]
        public async Task CalculatePriceAsync_ZeroServices_ReturnsCorrectTotal() //Правильный расчет без услуг
        {
            // Arrange
            var arrival = new DateTime(2025, 6, 1);
            var departure = new DateTime(2025, 6, 4); // 3 дня
            int roomTypeId = 2;

            _roomTypeRepoMock
                .Setup(r => r.GetRoomTypeByIdAsync(roomTypeId))
                .ReturnsAsync(new RoomType { ID = roomTypeId, Price = 150 });

            var emptyServices = new List<SelectedServiceItem>();

            // Act
            var total = await _service.CalculatePriceAsync(arrival, departure, roomTypeId, emptyServices);

            // Assert
            Assert.Equal(3 * 150, total);
        }

        [Fact]
        public async Task CalculatePriceAsync_InvalidRoomType_ThrowsArgumentException() //Исключение, если тип комнаты не найден
        {
            // Arrange
            _roomTypeRepoMock
                .Setup(r => r.GetRoomTypeByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((RoomType)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CalculatePriceAsync(DateTime.Now, DateTime.Now.AddDays(1), 99, new List<SelectedServiceItem>()));
        }

        [Fact]
        public async Task CalculatePriceAsync_ServiceNotFound_ThrowsArgumentException() //Исключение, если услуга не найдена
        {
            // Arrange
            int roomTypeId = 1;

            _roomTypeRepoMock
                .Setup(r => r.GetRoomTypeByIdAsync(roomTypeId))
                .ReturnsAsync(new RoomType { ID = roomTypeId, Price = 100 });

            var services = new List<SelectedServiceItem>
            {
                new SelectedServiceItem { AdditionalServiceID = 42, Count = 1 }
            };

            _addServiceRepoMock
                .Setup(r => r.GetAdditionalServiceByIdAsync(42))
                .ReturnsAsync((AdditionalService)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CalculatePriceAsync(DateTime.Now, DateTime.Now.AddDays(2), roomTypeId, services));
            Assert.Contains("не найдена", ex.Message);
        }

        #endregion

        #region Тесты подтверждения оплаты

        public async Task ConfirmPaymentAsync_ValidInput_ChangesStatuses() //Проверяет изменение статусов при правильных данных
        {
            // Arrange
            int resId = 1;
            var reservation = new Reservation
            {
                ID = resId,
                ReservationStatusID = 2 // Ожидание оплаты услуг
            };

            var services = new List<ServiceString>
            {
                new ServiceString { ID = 1, ReservationID = resId, ServiceStatusID = 1 }, // ожидает
                new ServiceString { ID = 2, ReservationID = resId, ServiceStatusID = 3 }, // отменена
                new ServiceString { ID = 3, ReservationID = resId, ServiceStatusID = 1 }  // ожидает
            };

            _resRepoMock
                .Setup(r => r.GetReservationByIdAsync(resId))
                .ReturnsAsync(reservation);

            _serviceStringRepoMock
                .Setup(s => s.GetServiceStringsForReservationAsync(resId))
                .ReturnsAsync(services);

            _resRepoMock
                .Setup(r => r.UpdateReservationAsync(It.IsAny<Reservation>()))
                .ReturnsAsync((Reservation r) => r); // возвращаем тот же объект, как будто он обновился


            _serviceStringRepoMock
                .Setup(s => s.UpdateServiceStringAsync(It.IsAny<ServiceString>()))
                .ReturnsAsync((ServiceString s) => s);


            var dto = new ReservationDTO(reservation);

            // Act
            var result = await _service.ConfirmPayment(dto);

            // Assert
            Assert.Equal(3, reservation.ReservationStatusID); //Статус бронирования изменился на "Оплачено полностью"
            Assert.All(services.Where(s => s.ID != 2), s => Assert.Equal(2, s.ServiceStatusID)); // Статусы услуг изменились на "Оплачена"
            Assert.Equal(3, services[1].ServiceStatusID); // Услуга осталась отмененной

            //Проверяет, что методы были вызваны только определенное количество раз
            _resRepoMock.Verify(r => r.UpdateReservationAsync(reservation), Times.Once);
            _serviceStringRepoMock.Verify(s => s.UpdateServiceStringAsync(It.Is<ServiceString>(ss => ss.ServiceStatusID == 2)), Times.Exactly(2));
            _serviceStringRepoMock.Verify(s => s.UpdateServiceStringAsync(It.Is<ServiceString>(ss => ss.ServiceStatusID == 3)), Times.Never);
        }

        [Theory]
        [InlineData(1, "Попытка подтвердить оплату услуг у бронирования, у которого еще не оплачено проживание")]
        [InlineData(3, "Попытка подтвердить оплату услуг у уже оплаченного бронирования")]
        [InlineData(4, "Попытка подтвердить оплату услуг у отмененного бронирования")]
        public async Task ConfirmPayment_InvalidStatus_ThrowsArgumentException(int statusId, string expectedMessage) //Проверяет выброс исключений
        {
            // Arrange
            var dto = new ReservationDTO { ID = 10, ReservationStatusID = statusId };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.ConfirmPayment(dto));
            Assert.Equal(expectedMessage, ex.Message);
        }

        #endregion

        #region Тесты добавления бронирования

        [Fact]
        public async Task AddReservationAsync_ValidData_AddsReservationAndServices() //Проверяет добавление бронирования и сервисов
        {
            // Arrange
            var room = new Room { ID = 1, RoomTypeID = 1 };
            var roomType = new RoomType { ID = 1, Price = 100 };
            var availableRooms = new List<Room> { room };

            var createDto = new CreateReservationDTO
            {
                ArrivalDate = DateTime.Today,
                DepartureDate = DateTime.Today.AddDays(3),
                RoomID = room.ID,
                UserID = "user123",
                ReservationStatusID = 1,
                Services = new List<SelectedServiceItem>
                {
                    new SelectedServiceItem() { AdditionalServiceID = 1, Count = 2, Price = 50 },
                    new SelectedServiceItem() { AdditionalServiceID = 2, Count = 1, Price = 200 }
                }
            };

            _roomRepoMock.Setup(r => r.GetRoomByIdAsync(room.ID)).ReturnsAsync(room);
            _roomRepoMock.Setup(r => r.GetAvailableRoomsAsync(createDto.ArrivalDate, createDto.DepartureDate, room.RoomTypeID)).ReturnsAsync(availableRooms);
            _roomTypeRepoMock.Setup(rt => rt.GetRoomTypeByIdAsync(room.RoomTypeID)).ReturnsAsync(roomType);

            //Принимает любой объект Reservation и при вызове присваивает ему id 10 (имитация сохранения в БД)
            _resRepoMock.Setup(r => r.AddReservationAsync(It.IsAny<Reservation>()))
                .Callback<Reservation>(r => r.ID = 10)
                .Returns(Task.CompletedTask);

            _resRepoMock.Setup(r => r.GetReservationByIdAsync(10))
                .ReturnsAsync(new Reservation { ID = 10, UserID = "", Room = new Room { },
                    ServiceStrings = new List<ServiceString>(), ReservationStatus = new ReservationStatus { }, User = new User { } });

            _serviceStringRepoMock.Setup(s => s.AddServiceStringAsync(It.IsAny<ServiceString>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.AddReservationAsync(createDto);

            // Assert
            _resRepoMock.Verify(r => r.AddReservationAsync(It.IsAny<Reservation>()), Times.Once);
            //Проверяем, что для каждой доп.услуги метод добавления услуги был вызван ровно 1 раз
            _serviceStringRepoMock.Verify(s => s.AddServiceStringAsync(It.Is<ServiceString>(
                ss => ss.AdditionalServiceID == 1 && ss.Count == 2 && ss.Price == 50
            )), Times.Once);
            _serviceStringRepoMock.Verify(s => s.AddServiceStringAsync(It.Is<ServiceString>(
                ss => ss.AdditionalServiceID == 2 && ss.Count == 1 && ss.Price == 200
            )), Times.Once);
            Assert.Equal(10, result.ID);
        }

        [Theory]
        [InlineData("2025-06-01", "2025-05-31", "Дата выезда должна быть позже даты заезда.")]
        [InlineData("2025-05-01", "2025-05-05", "Данная комната не является свободной в данный период")]
        public async Task AddReservationAsync_InvalidData_ThrowsArgumentException(string arrivalStr, string departureStr, string expectedMessage)
        {
            // Arrange
            var arrival = DateTime.Parse(arrivalStr);
            var departure = DateTime.Parse(departureStr);
            var room = new Room { ID = 1, RoomTypeID = 1 };

            var dto = new CreateReservationDTO
            {
                ArrivalDate = arrival,
                DepartureDate = departure,
                RoomID = 1,
                UserID = "user123",
                ReservationStatusID = 1,
                Services = new()
            };

            _roomRepoMock.Setup(r => r.GetRoomByIdAsync(1)).ReturnsAsync(room);
            _roomRepoMock.Setup(r => r.GetAvailableRoomsAsync(arrival, departure, 1)).ReturnsAsync(new List<Room>());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AddReservationAsync(dto));
            Assert.Equal(expectedMessage, ex.Message);
        }

        #endregion
    }
}