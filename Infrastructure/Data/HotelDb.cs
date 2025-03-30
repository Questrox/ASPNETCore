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
        public virtual DbSet<AdditionalService> AdditionalServices { get; set; }
        public virtual DbSet<ReservationStatus> ReservationStatuses { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomCategory> RoomCategories { get; set; }
        public virtual DbSet<RoomType> RoomTypes { get; set; }
        public virtual DbSet<ServiceStatus> ServiceStatuses { get; set; }
        public virtual DbSet<ServiceString> ServiceStrings { get; set; }

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

            // Настройка связи Room -> RoomType
            modelBuilder.Entity<Room>()
                .HasOne(r => r.RoomType)
                .WithMany(rt => rt.Room)
                .HasForeignKey(r => r.RoomTypeID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Room>()
                .Property(r => r.RoomTypeID)
                .HasDefaultValue(1);

            // Настройка связи RoomType -> RoomCategory
            modelBuilder.Entity<RoomType>()
                .HasOne(rt => rt.RoomCategory)
                .WithMany(rc => rc.RoomType)
                .HasForeignKey(rt => rt.RoomCategoryID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoomType>()
                .Property(r => r.RoomCategoryID)
                .HasDefaultValue(1);

            // Настройка связи Reservation -> Room
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Room)
                .WithMany(rm => rm.Reservation)
                .HasForeignKey(r => r.RoomID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Reservation>()
                .Property(r => r.RoomID)
                .HasDefaultValue(1);

            // Настройка связи Reservation -> ReservationStatus
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.ReservationStatus)
                .WithMany(rs => rs.Reservation)
                .HasForeignKey(r => r.ReservationStatusID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Reservation>()
                .Property(r => r.ReservationStatusID)
                .HasDefaultValue(1);

            // Настройка связи ServiceString -> AdditionalService
            modelBuilder.Entity<ServiceString>()
                .HasOne(ss => ss.AdditionalService)
                .WithMany(asv => asv.ServiceString)
                .HasForeignKey(ss => ss.AdditionalServiceID)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка связи ServiceString -> Reservation
            modelBuilder.Entity<ServiceString>()
                .HasOne(ss => ss.Reservation)
                .WithMany()
                .HasForeignKey(ss => ss.ReservationID)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка связи ServiceString -> ServiceStatus
            modelBuilder.Entity<ServiceString>()
                .HasOne(ss => ss.ServiceStatus)
                .WithMany(sss => sss.ServiceString)
                .HasForeignKey(ss => ss.ServiceStatusID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
