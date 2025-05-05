using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Services
{
    /// <summary>
    /// Сервис для получения данных справочников
    /// </summary>
    public class DictionariesService
    {
        private readonly IDictionariesRepository _dicRepository;
        public DictionariesService(IDictionariesRepository dicRepository) => _dicRepository = dicRepository;
        /// <summary>
        /// Получает все категории комнат
        /// </summary>
        /// <returns>Список категорий комнат</returns>
        public async Task<IEnumerable<RoomCategoryDTO>> GetRoomCategoriesAsync()
        {
            var entities = await _dicRepository.GetRoomCategoriesAsync();
            return entities.Select(c => new RoomCategoryDTO(c));
        }
        /// <summary>
        /// Получает все статусы бронирований
        /// </summary>
        /// <returns>Список статусов бронирований</returns>
        public async Task<IEnumerable<ReservationStatusDTO>> GetReservationStatusesAsync()
        {
            var entities = await _dicRepository.GetReservationStatusesAsync();
            return entities.Select(s => new ReservationStatusDTO(s));
        }
        /// <summary>
        /// Получает все статусы доп.услуг
        /// </summary>
        /// <returns>Список статусов услуг</returns>
        public async Task<IEnumerable<ServiceStatusDTO>> GetServiceStatusesAsync()
        {
            var entities = await _dicRepository.GetServiceStatusesAsync();
            return entities.Select(s => new ServiceStatusDTO(s));
        }
        /// <summary>
        /// Получает все RoomTypeImageDTO (пути к изображениям типа номера) для определенного типа
        /// </summary>
        /// <param name="rtID">Идентификатор типа номера</param>
        /// <returns>Список RoomTypeImageDTO (путей к изображениям типа номера)</returns>
        public async Task<IEnumerable<RoomTypeImageDTO>> GetRoomTypeImagesAsync(int rtID)
        {
            var entities = await _dicRepository.GetRoomTypeImagesAsync(rtID);
            return entities.Select(i =>  new RoomTypeImageDTO(i));
        }
    }
}
