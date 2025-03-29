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
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelDb _db;
        public RoomRepository(HotelDb db)
        {
            _db = db;
        }

        public async Task<Room> GetRoomByIdAsync(int id)
        {
            return await _db.Rooms.Include(r => r.Reservation).Include(r => r.RoomType).FirstOrDefaultAsync(r => r.ID == id);
        }
        public async Task<IEnumerable<Room>> GetRoomsAsync()
        {
            return await _db.Rooms.Include(u => u.Reservation).Include(r => r.RoomType).ToListAsync();
        }
        public async Task AddRoomAsync(Room room)
        {
            _db.Rooms.Add(room);
            await _db.SaveChangesAsync();
        }
        public async Task<Room> UpdateRoomAsync(Room room)
        {
            _db.Entry(room).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return await _db.Rooms.FindAsync(room.ID); // Загружаем обновленный объект из БД
        }

        public async Task DeleteRoomAsync(int id)
        {
            var room = await _db.Rooms.Include(u => u.Reservation).FirstOrDefaultAsync(u => u.ID == id);
            if (room != null)
            {
                if (room.Reservation.Any())
                    throw new InvalidOperationException("Room cannot be deleted because it has related reservations.");
                _db.Rooms.Remove(room);
                await _db.SaveChangesAsync();
            }
        }
    }
}