using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class HotelDb : IdentityDbContext<User>
    {
        public HotelDb(DbContextOptions<HotelDb> options)
            : base(options)
        { }

        public virtual DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            // Настройка сущности User
            modelBuilder.Entity<User>()
                .Property(e => e.FullName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Passport)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Reservation)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
