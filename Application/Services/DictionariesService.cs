using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Services
{
    public class DictionariesService
    {
        private readonly IDictionariesRepository _dicRepository;
        public DictionariesService(IDictionariesRepository dicRepository) => _dicRepository = dicRepository;
        public async Task<IEnumerable<RoomCategoryDTO>> GetRoomCategoriesAsync()
        {
            var entities = await _dicRepository.GetRoomCategoriesAsync();
            return entities.Select(c => new RoomCategoryDTO(c));
        }

        public async Task<IEnumerable<ReservationStatusDTO>> GetReservationStatusesAsync()
        {
            var entities = await _dicRepository.GetReservationStatusesAsync();
            return entities.Select(s => new ReservationStatusDTO(s));
        }

        public async Task<IEnumerable<ServiceStatusDTO>> GetServiceStatusesAsync()
        {
            var entities = await _dicRepository.GetServiceStatusesAsync();
            return entities.Select(s => new ServiceStatusDTO(s));
        }
        public async Task<IEnumerable<RoomTypeImageDTO>> GetRoomTypeImagesAsync(int rtID)
        {
            var entities = await _dicRepository.GetRoomTypeImagesAsync(rtID);
            return entities.Select(i =>  new RoomTypeImageDTO(i));
        }
    }
}
