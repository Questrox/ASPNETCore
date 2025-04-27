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
    public class AdditionalServiceRepository : IAdditionalServiceRepository
    {
        private readonly HotelDb _db;

        public AdditionalServiceRepository(HotelDb db)
        {
            _db = db;
        }

        public async Task<AdditionalService> GetAdditionalServiceByIdAsync(int id)
        {
            return await _db.AdditionalServices.FirstOrDefaultAsync(s => s.ID == id);
        }

        public async Task<IEnumerable<AdditionalService>> GetAdditionalServicesAsync()
        {
            return await _db.AdditionalServices.ToListAsync();
        }

        public async Task AddAdditionalServiceAsync(AdditionalService service)
        {
            _db.AdditionalServices.Add(service);
            await _db.SaveChangesAsync();
        }

        public async Task<AdditionalService> UpdateAdditionalServiceAsync(AdditionalService service)
        {
            _db.Entry(service).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return await _db.AdditionalServices.FirstOrDefaultAsync(s => s.ID == service.ID);
        }

        public async Task DeleteAdditionalServiceAsync(int id)
        {
            var service = await _db.AdditionalServices.FirstOrDefaultAsync(s => s.ID == id);
            if (service != null)
            {
                _db.AdditionalServices.Remove(service);
                await _db.SaveChangesAsync();
            }
        }
    }
}
