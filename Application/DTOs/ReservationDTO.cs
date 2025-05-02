using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ReservationDTO
    {
        public ReservationDTO() { }

        public ReservationDTO(Domain.Entities.Reservation reservation)
        {
            ID = reservation.ID;
            ArrivalDate = reservation.ArrivalDate;
            DepartureDate = reservation.DepartureDate;
            FullPrice = reservation.FullPrice;
            ServicesPrice = reservation.ServicesPrice;
            LivingPrice = reservation.LivingPrice;
            RoomID = reservation.RoomID;
            UserID = reservation.UserID;
            ReservationStatusID = reservation.ReservationStatusID;
            Room = reservation.Room;
            User = reservation.User;
            ReservationStatus = reservation.ReservationStatus;
            ServiceStrings = reservation.ServiceStrings.Select(s => new ServiceStringDTO(s)).ToList();
        }

        public int ID { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public decimal FullPrice { get; set; }
        public decimal ServicesPrice { get; set; }
        public decimal LivingPrice { get; set; }
        public int RoomID { get; set; }
        public string UserID { get; set; }
        public int ReservationStatusID { get; set; }
        public virtual Room Room { get; set; }
        public virtual User User { get; set; }
        public virtual ReservationStatus ReservationStatus { get; set; }
        public virtual ICollection<ServiceStringDTO> ServiceStrings { get; set; }
    }
}
