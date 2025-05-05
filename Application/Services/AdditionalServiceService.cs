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
    /// <summary>
    /// Сервис для доп.услуг
    /// </summary>
    public class AdditionalServiceService
    {
        private readonly IAdditionalServiceRepository _serviceRepository;

        public AdditionalServiceService(IAdditionalServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }
        /// <summary>
        /// Получает список дополнительных услуг
        /// </summary>
        /// <returns>Список из всех услуг</returns>
        public async Task<IEnumerable<AdditionalServiceDTO>> GetAdditionalServicesAsync()
        {
            var services = await _serviceRepository.GetAdditionalServicesAsync();
            return services.Select(s => new AdditionalServiceDTO(s));
        }
        /// <summary>
        /// Получает услугу по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Найденная услуга или null</returns>
        public async Task<AdditionalServiceDTO> GetAdditionalServiceByIdAsync(int id)
        {
            var service = await _serviceRepository.GetAdditionalServiceByIdAsync(id);
            if (service == null) return null;
            return new AdditionalServiceDTO(service);
        }
        /// <summary>
        /// Добавляет услугу
        /// </summary>
        /// <param name="createServiceDTO">Добавляемая услуга</param>
        /// <returns>Созданная услуга</returns>
        public async Task<AdditionalServiceDTO> AddAdditionalServiceAsync(CreateAdditionalServiceDTO createServiceDTO)
        {
            var newService = new AdditionalService
            {
                Name = createServiceDTO.Name,
                Price = createServiceDTO.Price
            };

            await _serviceRepository.AddAdditionalServiceAsync(newService);

            return new AdditionalServiceDTO(newService);
        }
        /// <summary>
        /// Обновляет услугу
        /// </summary>
        /// <param name="serviceDTO">Обновляемая услуга</param>
        /// <returns>Обновленная услуга</returns>
        public async Task<AdditionalServiceDTO> UpdateAdditionalServiceAsync(AdditionalServiceDTO serviceDTO)
        {
            var existingService = await _serviceRepository.GetAdditionalServiceByIdAsync(serviceDTO.ID);
            if (existingService == null) return null;

            existingService.Name = serviceDTO.Name;
            existingService.Price = serviceDTO.Price;

            await _serviceRepository.UpdateAdditionalServiceAsync(existingService);

            return new AdditionalServiceDTO(existingService);
        }
        /// <summary>
        /// Удаляет услугу
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAdditionalServiceAsync(int id)
        {
            await _serviceRepository.DeleteAdditionalServiceAsync(id);
        }
    }
}
