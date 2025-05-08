using Application.Services;
using Castle.Core.Logging;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.ApplicationTests
{
    public class ServiceStringServiceTests
    {
        private readonly Mock<IServiceStringRepository> _serviceStringRepoMock = new();
        private readonly Mock<IAdditionalServiceRepository> _additionalServiceRepoMock = new();
        private readonly Mock<IReservationRepository> _reservationRepoMock = new();
        private readonly Mock<ILogger<ServiceStringService>> _loggerMock = new();
        private readonly ServiceStringService _service;

        public ServiceStringServiceTests()
        {
            _service = new ServiceStringService(
                _serviceStringRepoMock.Object,
                _additionalServiceRepoMock.Object,
                _reservationRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task DeliverServiceAsync_ValidData_UpdatesCorrectly()
        {
            // Arrange
            var servStr = new ServiceString
            {
                ID = 1,
                Count = 5,
                DeliveredCount = 1,
                ServiceStatusID = 1,
                ReservationID = 2,
                AdditionalServiceID = 3,
                AdditionalService = new AdditionalService { },
                ServiceStatus = new ServiceStatus { },
                Reservation = new Reservation { }
            };
            var res = new Reservation
            {
                ID = 2,
                ReservationStatusID = 2,
                ServicesPrice = 100,
                FullPrice = 300
            };
            var service = new AdditionalService
            {
                ID = 3,
                Price = 50
            };

            _serviceStringRepoMock.Setup(r => r.GetServiceStringByIdAsync(servStr.ID)).ReturnsAsync(servStr);
            _reservationRepoMock.Setup(r => r.GetReservationByIdAsync(res.ID)).ReturnsAsync(res);
            _additionalServiceRepoMock.Setup(r => r.GetAdditionalServiceByIdAsync(service.ID)).ReturnsAsync(service);
            _serviceStringRepoMock.Setup(r => r.UpdateServiceStringAsync(It.IsAny<ServiceString>())).ReturnsAsync(servStr);
            _reservationRepoMock.Setup(r => r.UpdateReservationAsync(It.IsAny<Reservation>())).ReturnsAsync(res);
            _serviceStringRepoMock.Setup(r => r.GetServiceStringByIdAsync(servStr.ID)).ReturnsAsync(servStr);

            // Act
            var result = await _service.DeliverServiceAsync(servStr.ID, 2);

            // Assert
            Assert.Equal(3, servStr.DeliveredCount);
            Assert.Equal(200, res.ServicesPrice);
            Assert.Equal(400, res.FullPrice);

            _serviceStringRepoMock.Verify(r => r.UpdateServiceStringAsync(servStr), Times.Once);
            _reservationRepoMock.Verify(r => r.UpdateReservationAsync(res), Times.Once);
        }

        [Theory]
        [InlineData(null, 1, 2, 1, 3, 1, "Не существует строки услуг с идентификатором")]
        [InlineData(1, 5, 4, 1, 3, 2, "Попытка оказать больше услуг, чем было забронировано")]
        [InlineData(1, 5, 0, 2, 3, 1, "Попытка оказать уже оказанную (оплаченную) услугу")]
        [InlineData(1, 5, 0, 3, 3, 1, "Попытка оказать отмененную услугу")]
        [InlineData(1, 5, 0, 1, 1, 1, "Попытка оказать услугу гостю, который еще не заехал")]
        [InlineData(1, 5, 0, 1, 3, 1, "Попытка оказать услугу гостю, который уже выехал")]
        [InlineData(1, 5, 0, 1, 4, 1, "Попытка оказать услугу в отмененном бронировании")]
        [InlineData(1, 5, 0, 1, 2, 0, "Попытка указать количество услуг <= 0: ")]
        
        public async Task DeliverServiceAsync_InvalidData_ThrowsArgumentException(
            int? servStrExists,
            int count,
            int delivered,
            int status,
            int reservationStatus,
            int amount,
            string expectedMessageStart)
        {
            // Arrange
            var id = 1;

            if (servStrExists != null)
            {
                var servStr = new ServiceString
                {
                    ID = id,
                    Count = count,
                    DeliveredCount = delivered,
                    ServiceStatusID = status,
                    ReservationID = 2,
                    AdditionalServiceID = 3,
                    AdditionalService = new AdditionalService { },
                    ServiceStatus = new ServiceStatus { },
                    Reservation = new Reservation { }
                };
                _serviceStringRepoMock.Setup(r => r.GetServiceStringByIdAsync(id)).ReturnsAsync(servStr);
            }
            else
                _serviceStringRepoMock.Setup(r => r.GetServiceStringByIdAsync(id)).ReturnsAsync((ServiceString)null);

            _reservationRepoMock.Setup(r => r.GetReservationByIdAsync(It.IsAny<int>())).ReturnsAsync(new Reservation
            {
                ID = 2,
                ReservationStatusID = reservationStatus,
                ServicesPrice = 0,
                FullPrice = 0
            });

            _additionalServiceRepoMock.Setup(r => r.GetAdditionalServiceByIdAsync(It.IsAny<int>())).ReturnsAsync(new AdditionalService
            {
                ID = 3,
                Price = 10
            });

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.DeliverServiceAsync(id, amount));
            Assert.StartsWith(expectedMessageStart, ex.Message);
        }
    }
}
