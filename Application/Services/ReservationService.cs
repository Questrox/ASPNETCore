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
    public class ReservationService
    {
        private readonly IReservationRepository _resRepository;

        public ReservationService(IReservationRepository resRepository)
        {
            _resRepository = resRepository;
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
            var newRes = new Reservation
            {
                ArrivalDate = createReservationDTO.ArrivalDate,
                DepartureDate = createReservationDTO.DepartureDate,
                FullPrice = createReservationDTO.FullPrice,
                ServicesPrice = createReservationDTO.ServicesPrice,
                RoomID = createReservationDTO.RoomID,
                UserID = createReservationDTO.UserID,
                ReservationStatusID = createReservationDTO.ReservationStatusID
            };

            await _resRepository.AddReservationAsync(newRes);

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
    }
}

