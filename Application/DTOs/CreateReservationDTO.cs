using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateReservationDTO
    {
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public decimal FullPrice { get; set; }
        public decimal ServicesPrice { get; set; }
        public int RoomID { get; set; }
        public string UserID { get; set; }
        public int ReservationStatusID { get; set; }
    }
}
