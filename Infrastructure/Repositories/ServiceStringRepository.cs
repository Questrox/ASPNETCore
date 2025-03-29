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
    public class ServiceStringRepository : IServiceStringRepository
    {
        private readonly HotelDb _db;
        public ServiceStringRepository(HotelDb db)
        {
            _db = db;
        }

        public async Task<ServiceString> GetServiceStringByIdAsync(int id)
        {
            return await _db.ServiceStrings.Include(s => s.AdditionalService).Include(s => s.Reservation)
                .FirstOrDefaultAsync(s => s.ID == id);
        }
        public async Task<IEnumerable<ServiceString>> GetServiceStringsAsync()
        {
            return await _db.ServiceStrings.Include(s => s.AdditionalService).Include(s => s.Reservation).ToListAsync();
        }
        public async Task AddServiceStringAsync(ServiceString servStr)
        {
            _db.ServiceStrings.Add(servStr);
            await _db.SaveChangesAsync();
        }
        public async Task<ServiceString> UpdateServiceStringAsync(ServiceString servStr)
        {
            _db.Entry(servStr).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return await _db.ServiceStrings.FindAsync(servStr.ID); // Загружаем обновленный объект из БД
        }

        public async Task DeleteServiceStringAsync(int id)
        {
            var servStr = await _db.ServiceStrings.Include(u => u.Reservation).FirstOrDefaultAsync(u => u.ID == id);
            if (servStr != null)
            {
                _db.ServiceStrings.Remove(servStr);
                await _db.SaveChangesAsync();
            }
        }
    }
}
