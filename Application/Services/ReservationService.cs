using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ReservationService
    {
        private readonly IReservationRepository _resRepository;
        private readonly IRoomTypeRepository _rtRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IServiceStringRepository _serviceStringRepository;

        public ReservationService(IReservationRepository resRepository, IRoomTypeRepository rtRepository, 
            IRoomRepository roomRepository, IServiceStringRepository serviceStringRepository)
        {
            _resRepository = resRepository;
            _rtRepository = rtRepository;
            _roomRepository = roomRepository;
            _serviceStringRepository = serviceStringRepository;
        }

        public async Task<IEnumerable<ReservationDTO>> GetReservationsForUser(string userId)
        {
            var reservs = await _resRepository.GetReservationsForUserAsync(userId);
            return reservs.Select(x => new ReservationDTO(x));
        }
        public async Task<IEnumerable<ReservationDTO>> GetReservationsAsync()
        {
            var reservs = await _resRepository.GetReservationsAsync();
            return reservs.Select(x => new ReservationDTO(x));
        }

        public async Task<ReservationDTO> GetReservationByIdAsync(int id)
        {
            var res = await _resRepository.GetReservationByIdAsync(id);
            if (res == null) return null;
            return new ReservationDTO(res);
        }

        public async Task<ReservationDTO> AddReservationAsync(CreateReservationDTO createReservationDTO)
        {
            //Проверки
            if (createReservationDTO.ArrivalDate >= createReservationDTO.DepartureDate)
                throw new ArgumentException("Дата выезда должна быть позже даты заезда.");
            int totalDays = (int)(createReservationDTO.DepartureDate - createReservationDTO.ArrivalDate).TotalDays;
            Room r = await _roomRepository.GetRoomByIdAsync(createReservationDTO.RoomID);
            var availableRooms = await _roomRepository.GetAvailableRoomsAsync(createReservationDTO.ArrivalDate, createReservationDTO.DepartureDate, r.RoomTypeID);
            if (!availableRooms.Any(x => x.ID == r.ID))
                throw new ArgumentException("Данная комната не является свободной в данный период");
            RoomType rt = await _rtRepository.GetRoomTypeByIdAsync(r.RoomTypeID);

            //Расчет цен
            decimal livingPrice = rt.Price * totalDays;
            decimal servicesPrice = 0;
            //for (int i = 0; i < createReservationDTO.Services.Count; i++)
            //{
            //    servicesPrice += createReservationDTO.Services[i].Price * createReservationDTO.Services[i].Count;
            //}
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

            //Добавление строк доп.услуг в БД
            for (int i = 0; i < createReservationDTO.Services.Count; i++)
            {
                ServiceString servStr = new ServiceString
                {
                    Count = createReservationDTO.Services[i].Count,
                    DeliveredCount = 0,
                    AdditionalServiceID = createReservationDTO.Services[i].AdditionalServiceID,
                    Price = createReservationDTO.Services[i].Price,
                    ServiceStatusID = 1,
                    ReservationID = newRes.ID
                };
                await _serviceStringRepository.AddServiceStringAsync(servStr);
            }

            return new ReservationDTO(newRes);
        }
        public async Task<ReservationDTO?> UpdateReservationAsync(ReservationDTO resDTO)
        {
            var existingRes = await _resRepository.GetReservationByIdAsync(resDTO.ID);
            if (existingRes == null) return null;

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

        public async Task DeleteReservationAsync(int id)
        {
            await _resRepository.DeleteReservationAsync(id);
        }

        public async Task<decimal> CalculatePriceAsync(DateTime arrival, DateTime departure, int roomTypeID, List<SelectedServiceItem> services)
        {
            decimal result = 0;
            int totalDays = (int)(departure - arrival).TotalDays;
            RoomType roomType = await _rtRepository.GetRoomTypeByIdAsync(roomTypeID);
            if (roomType == null)
                throw new ArgumentException("Неверный ID типа комнаты.");
            result += roomType.Price * totalDays;
            for (int i = 0; i < services.Count; i++)
            {
                result += services[i].Price * services[i].Count;
            }
            return result;
        }
    }
}

