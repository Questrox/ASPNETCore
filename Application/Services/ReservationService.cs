using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Сервис для бронирований
    /// </summary>
    public class ReservationService
    {
        private readonly IReservationRepository _resRepository;
        private readonly IRoomTypeRepository _rtRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IServiceStringRepository _serviceStringRepository;
        private readonly IAdditionalServiceRepository _additionalServiceRepository;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(IReservationRepository resRepository, IRoomTypeRepository rtRepository, 
            IRoomRepository roomRepository, IServiceStringRepository serviceStringRepository,
            IAdditionalServiceRepository additionalServiceRepository, ILogger<ReservationService> logger)
        {
            _resRepository = resRepository;
            _rtRepository = rtRepository;
            _roomRepository = roomRepository;
            _serviceStringRepository = serviceStringRepository;
            _additionalServiceRepository = additionalServiceRepository;
            _logger = logger;
        }
        /// <summary>
        /// Подтверждает оплату доп.услуг бронирования (меняет статус бронирования). Также изменяет статусы строк услуг, связанных с этим бронированием
        /// </summary>
        /// <param name="resDTO">Подтверждаемое бронирование</param>
        /// <returns>Обновленное бронирование</returns>
        /// <exception cref="ArgumentException">Исключения на случай неверного статуса бронирования</exception>
        public async Task<ReservationDTO> ConfirmPayment(ReservationDTO resDTO)
        {
            //Проверки
            if (resDTO.ReservationStatusID == 1)
            {
                _logger.LogError($"Ошибка при подтверждении оплаты доп.услуг у бронирования с id {resDTO.ID}:" +
                    $" у бронирования еще не оплачено проживание");
                throw new ArgumentException("Попытка подтвердить оплату услуг у бронирования, у которого еще не оплачено проживание");
            }
            if (resDTO.ReservationStatusID == 3)
            {
                _logger.LogError($"Ошибка при подтверждении оплаты доп.услуг у бронирования с id {resDTO.ID}:" +
                    $" бронирование уже оплачено");
                throw new ArgumentException("Попытка подтвердить оплату услуг у уже оплаченного бронирования");
            }
            if (resDTO.ReservationStatusID == 4)
            {
                _logger.LogError($"Ошибка при подтверждении оплаты доп.услуг у бронирования с id {resDTO.ID}:" +
                    $" бронирование отменено");
                throw new ArgumentException("Попытка подтвердить оплату услуг у отмененного бронирования");
            }
            
            //Меняем статусы услуг
            var res = await _resRepository.GetReservationByIdAsync(resDTO.ID);
            var services = await _serviceStringRepository.GetServiceStringsForReservationAsync(resDTO.ID);

            foreach (var service in services.Where(s => s.ServiceStatusID == 1))
            {
                service.ServiceStatusID = 2;
                await _serviceStringRepository.UpdateServiceStringAsync(service);
            }

            //Обновляем статус бронирования
            res.ReservationStatusID = 3;
            await _resRepository.UpdateReservationAsync(res);

            return new ReservationDTO(res);
        }
        /// <summary>
        /// Получает все бронирования пользователя по его паспорту
        /// </summary>
        /// <param name="passport">Паспортные данные пользователя</param>
        /// <returns>Список бронирований пользователя с данным паспортом</returns>
        public async Task<IEnumerable<ReservationDTO>> GetReservationsByPassportAsync(string passport)
        {
            var reservs = await _resRepository.GetReservationsByPassportAsync(passport);
            return reservs.Select(x => new ReservationDTO(x));
        }
        /// <summary>
        /// Получает все бронирования пользователя по его идентификатору
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список бронирований пользователя</returns>
        public async Task<IEnumerable<ReservationDTO>> GetReservationsForUserAsync(string userId)
        {
            var reservs = await _resRepository.GetReservationsForUserAsync(userId);
            return reservs.Select(x => new ReservationDTO(x));
        }
        /// <summary>
        /// Получает список всех бронирований
        /// </summary>
        /// <returns>Список всех бронирований</returns>
        public async Task<IEnumerable<ReservationDTO>> GetReservationsAsync()
        {
            var reservs = await _resRepository.GetReservationsAsync();
            return reservs.Select(x => new ReservationDTO(x));
        }
        /// <summary>
        /// Получает бронирование по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>Найденное бронирование или null</returns>
        public async Task<ReservationDTO> GetReservationByIdAsync(int id)
        {
            var res = await _resRepository.GetReservationByIdAsync(id);
            if (res == null)
            {
                _logger.LogError($"Не удалось найти бронирование с идентификатором {id}");
                return null;
            }
            return new ReservationDTO(res);
        }
        /// <summary>
        /// Создает бронирование, заново рассчитывая цены, а также добавляет в БД строки доп.услуг, связанных с этим бронированием
        /// </summary>
        /// <param name="createReservationDTO">Создаваемое бронирование</param>
        /// <returns>Созданное бронирование</returns>
        /// <exception cref="ArgumentException">Исключения на случай некорректных данных</exception>
        public async Task<ReservationDTO> AddReservationAsync(CreateReservationDTO createReservationDTO)
        {
            //Проверки
            if (createReservationDTO.ArrivalDate >= createReservationDTO.DepartureDate)
            {
                _logger.LogError($"Ошибка при создании бронирования: дата выезда ({createReservationDTO.ArrivalDate:dd-MM-yyyy})" +
                    $" должна быть позже даты заезда ({createReservationDTO.DepartureDate:dd-MM-yyyy})");
                throw new ArgumentException("Дата выезда должна быть позже даты заезда.");
            }
            int totalDays = (int)(createReservationDTO.DepartureDate - createReservationDTO.ArrivalDate).TotalDays;
            Room r = await _roomRepository.GetRoomByIdAsync(createReservationDTO.RoomID);
            var availableRooms = await _roomRepository.GetAvailableRoomsAsync(createReservationDTO.ArrivalDate, createReservationDTO.DepartureDate, r.RoomTypeID);
            if (!availableRooms.Any(x => x.ID == r.ID))
            {
                _logger.LogError($"Ошибка при создании бронирования: комната {r.Number} не является свободной в период " +
                    $"с {createReservationDTO.ArrivalDate:dd-MM-yyyy} по {createReservationDTO.DepartureDate:dd-MM-yyyy}");
                throw new ArgumentException("Данная комната не является свободной в данный период");
            }
            RoomType rt = await _rtRepository.GetRoomTypeByIdAsync(r.RoomTypeID);

            //Расчет цен
            decimal livingPrice = rt.Price * totalDays;
            decimal servicesPrice = 0;
            decimal fullPrice = livingPrice + servicesPrice;

            //Добавление бронирования в БД
            var newRes = new Reservation
            {
                ArrivalDate = createReservationDTO.ArrivalDate,
                DepartureDate = createReservationDTO.DepartureDate,
                FullPrice = fullPrice,
                ServicesPrice = servicesPrice,
                LivingPrice = livingPrice,
                RoomID = createReservationDTO.RoomID,
                UserID = createReservationDTO.UserID,
                ReservationStatusID = createReservationDTO.ReservationStatusID
            };

            await _resRepository.AddReservationAsync(newRes);
            var services = createReservationDTO.Services ?? new List<SelectedServiceItem>();
            //Добавление строк доп.услуг в БД
            for (int i = 0; i < services.Count; i++)
            {
                ServiceString servStr = new ServiceString
                {
                    Count = services[i].Count,
                    DeliveredCount = 0,
                    AdditionalServiceID = services[i].AdditionalServiceID,
                    Price = services[i].Price,
                    ServiceStatusID = 1,
                    ReservationID = newRes.ID
                };
                await _serviceStringRepository.AddServiceStringAsync(servStr);
            }
            newRes = await _resRepository.GetReservationByIdAsync(newRes.ID);
            return new ReservationDTO(newRes);
        }
        /// <summary>
        /// Обновляет бронирование
        /// </summary>
        /// <param name="resDTO">Обновляемое бронирование</param>
        /// <returns>Обновленное бронирование</returns>
        public async Task<ReservationDTO?> UpdateReservationAsync(ReservationDTO resDTO)
        {
            var existingRes = await _resRepository.GetReservationByIdAsync(resDTO.ID);
            if (existingRes == null)
            {
                _logger.LogError($"Бронирования с id {resDTO.ID} не найдено, обновление не выполнено");
                return null;
            }

            existingRes.ArrivalDate = resDTO.ArrivalDate;
            existingRes.DepartureDate = resDTO.DepartureDate;
            existingRes.FullPrice = resDTO.FullPrice;
            existingRes.ServicesPrice = resDTO.ServicesPrice;
            existingRes.RoomID = resDTO.RoomID;
            existingRes.UserID = resDTO.UserID;
            existingRes.ReservationStatusID = resDTO.ReservationStatusID;

            await _resRepository.UpdateReservationAsync(existingRes);

            return new ReservationDTO(existingRes); // Возвращаем обновленный объект
        }
        /// <summary>
        /// Удаляет бронирование
        /// </summary>
        /// <param name="id">Идентификатор удаляемого бронирования</param>
        /// <returns></returns>
        public async Task DeleteReservationAsync(int id)
        {
            await _resRepository.DeleteReservationAsync(id);
        }
        /// <summary>
        /// Рассчитывает цену для формируемого бронирования с учетом доп.услуг
        /// </summary>
        /// <param name="arrival">Дата приезда</param>
        /// <param name="departure">Дата отъезда</param>
        /// <param name="roomTypeID">Идентификатор типа комнаты</param>
        /// <param name="services">Выбранные доп.услуги</param>
        /// <returns>Цену</returns>
        /// <exception cref="ArgumentException">Неверный идентификатор типа комнаты или доп.услуга не найдена</exception>
        public async Task<decimal> CalculatePriceAsync(DateTime arrival, DateTime departure, int roomTypeID, List<SelectedServiceItem> services)
        {
            decimal result = 0;
            int totalDays = (int)(departure - arrival).TotalDays;
            RoomType roomType = await _rtRepository.GetRoomTypeByIdAsync(roomTypeID);
            if (roomType == null)
            {
                _logger.LogError($"Расчет цены не выполнен: не удалось найти тип комнаты с идентификатором {roomTypeID}");
                throw new ArgumentException("Неверный ID типа комнаты.");
            }
            result += roomType.Price * totalDays;
            for (int i = 0; i < services.Count; i++)
            {
                var service = await _additionalServiceRepository.GetAdditionalServiceByIdAsync(services[i].AdditionalServiceID);
                if (service == null)
                {
                    _logger.LogError($"Расчет цены не выполнен: не удалось найти доп.услугу " +
                        $"с идентификатором {services[i].AdditionalServiceID}");
                    throw new ArgumentException($"Услуга с ID {services[i].AdditionalServiceID} не найдена.");
                }
                result += service.Price * services[i].Count;
            }
            return result;
        }
    }
}