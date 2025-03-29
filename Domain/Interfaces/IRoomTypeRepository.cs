using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRoomTypeRepository
    {
        Task<RoomType> GetRoomTypeByIdAsync(int id);
        Task<IEnumerable<RoomType>> GetRoomTypesAsync();
        Task AddRoomTypeAsync(RoomType roomType);
        Task<RoomType> UpdateRoomTypeAsync(RoomType roomType);
        Task DeleteRoomTypeAsync(int id);
    }
}
