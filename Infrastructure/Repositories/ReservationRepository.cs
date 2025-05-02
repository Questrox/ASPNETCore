using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly HotelDb _db;
        public ReservationRepository(HotelDb db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Reservation>> GetReservationsForUserAsync(string userID)
        {
            return await _db.Reservations.Include(r => r.Room).ThenInclude(room => room.RoomType).ThenInclude(rt => rt.RoomCategory)
                .Include(r => r.ReservationStatus).Include(r => r.User)
                .Include(r => r.ServiceStrings).ThenInclude(r => r.ServiceStatus)
                .Include(r => r.ServiceStrings).ThenInclude(r => r.AdditionalService)
                .Where(r => r.UserID == userID)
                .OrderByDescending(r => r.ArrivalDate).ToListAsync();
        }
        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            return await _db.Reservations.Include(r => r.Room).ThenInclude(room => room.RoomType).ThenInclude(rt => rt.RoomCategory)
                .Include(r => r.ReservationStatus).Include(r => r.User)
                .Include(r => r.ServiceStrings).ThenInclude(r => r.ServiceStatus)
                .Include(r => r.ServiceStrings).ThenInclude(r => r.AdditionalService)
                .FirstOrDefaultAsync(r => r.ID == id);
        }
        public async Task<IEnumerable<Reservation>> GetReservationsAsync()
        {
            return await _db.Reservations.Include(r => r.Room).ThenInclude(room => room.RoomType).ThenInclude(rt => rt.RoomCategory)
                .Include(r => r.ReservationStatus).Include(r => r.User)
                .Include(r => r.ServiceStrings).ThenInclude(r => r.ServiceStatus)
                .Include(r => r.ServiceStrings).ThenInclude(r => r.AdditionalService)
                .ToListAsync();
        }
        public async Task AddReservationAsync(Reservation res)
        {
            _db.Reservations.Add(res);
            await _db.SaveChangesAsync();
        }
        public async Task<Reservation> UpdateReservationAsync(Reservation res)
        {
            _db.Entry(res).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return await _db.Reservations.Include(r => r.Room).ThenInclude(room => room.RoomType).ThenInclude(rt => rt.RoomCategory)
                .Include(r => r.ReservationStatus).Include(r => r.User)
                .Include(r => r.ServiceStrings).ThenInclude(r => r.ServiceStatus)
                .Include(r => r.ServiceStrings).ThenInclude(r => r.AdditionalService)
                .FirstOrDefaultAsync(r => r.ID == res.ID); // Загружаем обновленный объект из БД
        }

        public async Task DeleteReservationAsync(int id)
        {
            var res = await _db.Reservations.FirstOrDefaultAsync(u => u.ID == id);
            if (res != null)
            {
                _db.Reservations.Remove(res);
                await _db.SaveChangesAsync();
            }
        }
    }
}
