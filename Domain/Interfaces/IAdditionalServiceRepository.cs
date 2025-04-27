using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAdditionalServiceRepository
    {
        Task<AdditionalService> GetAdditionalServiceByIdAsync(int id);
        Task<IEnumerable<AdditionalService>> GetAdditionalServicesAsync();
        Task AddAdditionalServiceAsync(AdditionalService service);
        Task<AdditionalService> UpdateAdditionalServiceAsync(AdditionalService service);
        Task DeleteAdditionalServiceAsync(int id);
    }
}
