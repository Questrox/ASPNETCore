using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RoomTypeService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;

        public RoomTypeService(IRoomTypeRepository roomTypeRepository)
        {
            _roomTypeRepository = roomTypeRepository;
        }

        public async Task<IEnumerable<RoomTypeDTO>> GetRoomTypesAsync()
        {
            var rooms = await _roomTypeRepository.GetRoomTypesAsync();
            return rooms.Select(x => new RoomTypeDTO(x));
        }

        public async Task<RoomTypeDTO> GetRoomTypeByIdAsync(int id)
        {
            var rt = await _roomTypeRepository.GetRoomTypeByIdAsync(id);
            if (rt == null) return null;
            return new RoomTypeDTO(rt);
        }

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
        public async Task<RoomTypeDTO?> UpdateRoomTypeAsync(RoomTypeDTO roomTypeDTO)
        {
            var existingRoomType = await _roomTypeRepository.GetRoomTypeByIdAsync(roomTypeDTO.ID);
            if (existingRoomType == null) return null;

            existingRoomType.GuestCapacity = roomTypeDTO.GuestCapacity;
            existingRoomType.RoomCategoryID = roomTypeDTO.RoomCategoryID;
            existingRoomType.Price = roomTypeDTO.Price;
            existingRoomType.Description = roomTypeDTO.Description;

            await _roomTypeRepository.UpdateRoomTypeAsync(existingRoomType);

            return new RoomTypeDTO(existingRoomType); // Возвращаем обновленный объект
        }

        public async Task DeleteRoomTypeAsync(int id)
        {
            await _roomTypeRepository.DeleteRoomTypeAsync(id);
        }
    }
}
