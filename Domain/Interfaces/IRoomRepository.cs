using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room> GetRoomByIdAsync(int id);
        Task<IEnumerable<Room>> GetRoomsAsync();
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime arrivalDate, DateTime departureDate, int roomTypeID);
        Task AddRoomAsync(Room room);
        Task<Room> UpdateRoomAsync(Room room);
        Task DeleteRoomAsync(int id);
    }
}
