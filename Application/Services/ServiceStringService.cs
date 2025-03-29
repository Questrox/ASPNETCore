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
    public class ServiceStringService
    {
        private readonly IServiceStringRepository _serviceStringRepository;

        public ServiceStringService(IServiceStringRepository serviceStringRepository)
        {
            _serviceStringRepository = serviceStringRepository;
        }

        public async Task<IEnumerable<ServiceStringDTO>> GetServiceStringsAsync()
        {
            var strings = await _serviceStringRepository.GetServiceStringsAsync();
            return strings.Select(x => new ServiceStringDTO(x));
        }

        public async Task<ServiceStringDTO> GetServiceStringByIdAsync(int id)
        {
            var s = await _serviceStringRepository.GetServiceStringByIdAsync(id);
            if (s == null) return null;
            return new ServiceStringDTO(s);
        }

        public async Task<ServiceStringDTO> AddServiceStringAsync(CreateServiceStringDTO createServiceStringDTO)
        {
            var s = new ServiceString
            {
                Count = createServiceStringDTO.Count,
                AdditionalServiceID = createServiceStringDTO.AdditionalServiceID,
                ReservationID = createServiceStringDTO.ReservationID,
                Price = createServiceStringDTO.Price,
                ServiceStatusID = createServiceStringDTO.ServiceStatusID
            };

            await _serviceStringRepository.AddServiceStringAsync(s);

            return new ServiceStringDTO(s);
        }
        public async Task<ServiceStringDTO?> UpdateServiceStringAsync(ServiceStringDTO s)
        {
            var existingString = await _serviceStringRepository.GetServiceStringByIdAsync(s.ID);
            if (existingString == null) return null;

            existingString.Count = s.Count;
            existingString.AdditionalServiceID = s.AdditionalServiceID;
            existingString.ReservationID = s.ReservationID;
            existingString.Price = s.Price;
            existingString.ServiceStatusID = s.ServiceStatusID;

            await _serviceStringRepository.UpdateServiceStringAsync(existingString);

            return new ServiceStringDTO(existingString); // Возвращаем обновленный объект
        }

        public async Task DeleteServiceStringAsync(int id)
        {
            await _serviceStringRepository.DeleteServiceStringAsync(id);
        }
    }
}