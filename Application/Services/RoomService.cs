using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services
{
    /// <summary>
    /// Сервис для комнат/номеров
    /// </summary>
    public class RoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }
        /// <summary>
        /// Получает список всех комнат
        /// </summary>
        /// <returns>Список всех комнат</returns>
        public async Task<IEnumerable<RoomDTO>> GetRoomsAsync()
        {
            var rooms = await _roomRepository.GetRoomsAsync();
            return rooms.Select(x => new RoomDTO(x));
        }
        /// <summary>
        /// Получает комнату по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор комнаты</param>
        /// <returns>Комнату или null, если ее нет</returns>
        public async Task<RoomDTO> GetRoomByIdAsync(int id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if (room == null) return null;
            return new RoomDTO(room);
        }
        /// <summary>
        /// Получает список свободных комнат в указанный период указанного типа
        /// </summary>
        /// <param name="arrivalDate">Дата приезда</param>
        /// <param name="departureDate">Дата отъезда</param>
        /// <param name="roomTypeID">Тип комнаты</param>
        /// <returns>Список свободных комнат</returns>
        public async Task<IEnumerable<RoomDTO>> GetAvailableRoomsAsync(DateTime arrivalDate, DateTime departureDate, int roomTypeID)
        {
            var rooms = await _roomRepository.GetAvailableRoomsAsync(arrivalDate, departureDate, roomTypeID);
            return rooms.Select(x => new RoomDTO(x));
        }
        /// <summary>
        /// Добавляет комнату
        /// </summary>
        /// <param name="createRoomDTO">Добавляемая комната</param>
        /// <returns>Созданную комнату</returns>
        public async Task<RoomDTO> AddRoomAsync(CreateRoomDTO createRoomDTO)
        {
            var newRoom = new Room
            {
                Number = createRoomDTO.Number,
                RoomTypeID = createRoomDTO.RoomTypeID
            };

            await _roomRepository.AddRoomAsync(newRoom);

            return new RoomDTO(newRoom);
        }
        /// <summary>
        /// Обновляет комнату
        /// </summary>
        /// <param name="roomDTO">Обновляемая комната</param>
        /// <returns>Обновленную комнату</returns>
        public async Task<RoomDTO?> UpdateRoomAsync(RoomDTO roomDTO)
        {
            var existingRoom = await _roomRepository.GetRoomByIdAsync(roomDTO.ID);
            if (existingRoom == null) return null;

            existingRoom.Number = roomDTO.Number;
            existingRoom.RoomTypeID = roomDTO.RoomTypeID;

            await _roomRepository.UpdateRoomAsync(existingRoom);

            return new RoomDTO(existingRoom); // Возвращаем обновленный объект
        }
        /// <summary>
        /// Удаляет комнату
        /// </summary>
        /// <param name="id">Идентификатор удаляемой комнаты</param>
        /// <returns></returns>
        public async Task DeleteRoomAsync(int id)
        {
            await _roomRepository.DeleteRoomAsync(id);
        }
    }
}
