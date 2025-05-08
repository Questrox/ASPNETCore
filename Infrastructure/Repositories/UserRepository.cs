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
    public class UserRepository : IUserRepository
    {
        private readonly HotelDb _db;
        public UserRepository(HotelDb db)
        {
            _db = db;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _db.Users.Include(u => u.Reservation).FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _db.Users.ToListAsync();
        }
        public async Task AddUserAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
        public async Task<User> UpdateUserAsync(User user)
        {
            _db.Entry(user).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return await _db.Users.FindAsync(user.Id); // Загружаем обновленный объект из БД
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _db.Users.Include(u => u.Reservation).FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                if (user.Reservation.Any())
                    throw new InvalidOperationException("User cannot be deleted because it has related reservations.");
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
            }
        }
    }
}