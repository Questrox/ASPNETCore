using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDictionariesRepository
    {
        Task<IEnumerable<RoomCategory>> GetRoomCategoriesAsync();
        Task<IEnumerable<ReservationStatus>> GetReservationStatusesAsync();
        Task<IEnumerable<ServiceStatus>> GetServiceStatusesAsync();
    }
}
