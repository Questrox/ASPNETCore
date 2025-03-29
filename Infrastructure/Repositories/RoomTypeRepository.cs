using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly HotelDb _db;
        public RoomTypeRepository(HotelDb db)
        {
            _db = db;
        }

        public async Task<RoomType> GetRoomTypeByIdAsync(int id)
        {
            return await _db.RoomTypes.Include(r => r.Room).Include(r => r.RoomCategory).FirstOrDefaultAsync(r => r.ID == id);
        }
        public async Task<IEnumerable<RoomType>> GetRoomTypesAsync()
        {
            return await _db.RoomTypes.Include(r => r.Room).Include(r => r.RoomCategory).ToListAsync();
        }
        public async Task AddRoomTypeAsync(RoomType rt)
        {
            _db.RoomTypes.Add(rt);
            await _db.SaveChangesAsync();
        }
        public async Task<RoomType> UpdateRoomTypeAsync(RoomType rt)
        {
            _db.Entry(rt).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return await _db.RoomTypes.FindAsync(rt.ID); // Загружаем обновленный объект из БД
        }

        public async Task DeleteRoomTypeAsync(int id)
        {
            var rt = await _db.RoomTypes.Include(r => r.Room).FirstOrDefaultAsync(r => r.ID == id);
            if (rt != null)
            {
                if (rt.Room.Any())
                    throw new InvalidOperationException("Room type cannot be deleted because it has related rooms.");
                _db.RoomTypes.Remove(rt);
                await _db.SaveChangesAsync();
            }
        }
    }
}
