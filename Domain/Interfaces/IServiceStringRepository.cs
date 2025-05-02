using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IServiceStringRepository
    {
        Task<ServiceString> GetServiceStringByIdAsync(int id);
        Task<IEnumerable<ServiceString>> GetServiceStringsAsync();
        Task<IEnumerable<ServiceString>> GetServiceStringsForReservationAsync(int resID);
        Task AddServiceStringAsync(ServiceString serviceString);
        Task<ServiceString> UpdateServiceStringAsync(ServiceString serviceString);
        Task DeleteServiceStringAsync(int id);
    }
}
