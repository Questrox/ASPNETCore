using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Сервис для работы с типами номеров
    /// </summary>
    public class RoomTypeService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly ILogger<RoomTypeService> _logger;

        public RoomTypeService(IRoomTypeRepository roomTypeRepository, ILogger<RoomTypeService> logger)
        {
            _roomTypeRepository = roomTypeRepository;
            _logger = logger;
        }
        /// <summary>
        /// Получает список всех типов номеров
        /// </summary>
        /// <returns>Список всех типов номеров</returns>
        public async Task<IEnumerable<RoomTypeDTO>> GetRoomTypesAsync()
        {
            var rooms = await _roomTypeRepository.GetRoomTypesAsync();
            return rooms.Select(x => new RoomTypeDTO(x));
        }
        /// <summary>
        /// Получает тип номера по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор типа номера</param>
        /// <returns>Тип номера или null, если не найден</returns>
        public async Task<RoomTypeDTO> GetRoomTypeByIdAsync(int id)
        {
            var rt = await _roomTypeRepository.GetRoomTypeByIdAsync(id);
            if (rt == null)
            {
                _logger.LogError($"Не удалось найти тип комнаты с идентификатором {id}");
                return null;
            }
            return new RoomTypeDTO(rt);
        }
        /// <summary>
        /// Добавляет новый тип номера
        /// </summary>
        /// <param name="createRoomTypeDTO">Добавляемый тип номера</param>
        /// <returns>Созданный тип</returns>
        public async Task<RoomTypeDTO> AddRoomTypeAsync(CreateRoomTypeDTO createRoomTypeDTO)
        {
            var newRoomType = new RoomType
            {
                GuestCapacity = createRoomTypeDTO.GuestCapacity,
                RoomCategoryID = createRoomTypeDTO.RoomCategoryID,
                Price = createRoomTypeDTO.Price,
                Description = createRoomTypeDTO.Description
            };

            await _roomTypeRepository.AddRoomTypeAsync(newRoomType);

            return new RoomTypeDTO(newRoomType);
        }
        /// <summary>
        /// Обновляет существующий тип номера
        /// </summary>
        /// <param name="roomTypeDTO">Обновляемый тип</param>
        /// <returns>Обновленный тип или null, если не найден</returns>
        public async Task<RoomTypeDTO?> UpdateRoomTypeAsync(RoomTypeDTO roomTypeDTO)
        {
            var existingRoomType = await _roomTypeRepository.GetRoomTypeByIdAsync(roomTypeDTO.ID);
            if (existingRoomType == null)
            {
                _logger.LogError($"Тип комнаты с id {roomTypeDTO.ID} не найден, обновление не выполнено");
                return null;
            }

            existingRoomType.GuestCapacity = roomTypeDTO.GuestCapacity;
            existingRoomType.RoomCategoryID = roomTypeDTO.RoomCategoryID;
            existingRoomType.Price = roomTypeDTO.Price;
            existingRoomType.Description = roomTypeDTO.Description;

            await _roomTypeRepository.UpdateRoomTypeAsync(existingRoomType);

            return new RoomTypeDTO(existingRoomType); // Возвращаем обновленный объект
        }
        /// <summary>
        /// Удаляет тип номера по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор типа номера</param>
        public async Task DeleteRoomTypeAsync(int id)
        {
            await _roomTypeRepository.DeleteRoomTypeAsync(id);
        }
    }
}
