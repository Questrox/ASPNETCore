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
    public class DictionariesRepository : IDictionariesRepository
    {
        private readonly HotelDb _db;
        public DictionariesRepository(HotelDb db)
        {
            _db = db;
        }
        public async Task<IEnumerable<RoomCategory>> GetRoomCategoriesAsync()
        {
            return await _db.RoomCategories.ToListAsync();
        }
        public async Task<IEnumerable<ReservationStatus>> GetReservationStatusesAsync()
        {
            return await _db.ReservationStatuses.ToListAsync();
        }
        public async Task<IEnumerable<ServiceStatus>> GetServiceStatusesAsync()
        {
            return await _db.ServiceStatuses.ToListAsync();
        }
    }
}
