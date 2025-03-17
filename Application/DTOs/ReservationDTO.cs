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
            RoomID = reservation.RoomID;
            UserID = reservation.UserID;
        }

        public int ID { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public double FullPrice { get; set; }
        public int RoomID { get; set; }
        public string UserID { get; set; }
    }
}
