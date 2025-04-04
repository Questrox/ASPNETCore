﻿using Domain.Entities;
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
    public class ReservationRepository : IReservationRepository
    {
        private readonly HotelDb _db;
        public ReservationRepository(HotelDb db)
        {
            _db = db;
        }

        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            return await _db.Reservations.Include(r => r.User).Include(r => r.Room).Include(r => r.ReservationStatus)
                .FirstOrDefaultAsync(r => r.ID == id);
        }
        public async Task<IEnumerable<Reservation>> GetReservationsAsync()
        {
            return await _db.Reservations.Include(r => r.User).Include(r => r.Room).Include(r => r.ReservationStatus)
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

            return await _db.Reservations.FindAsync(res.ID); // Загружаем обновленный объект из БД
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
