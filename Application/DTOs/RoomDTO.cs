using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RoomDTO
    {
        public RoomDTO() { }
        public RoomDTO(Room r)
        {
            ID = r.ID;
            Number = r.Number;
            RoomTypeID = r.RoomTypeID;
            Reservation = r.Reservation?.Select(r => new ReservationDTO(r)).ToList();
            RoomType = r.RoomType;
        }
        public RoomDTO(RoomDTO r)
        {
            ID = r.ID;
            Number = r.Number;
            RoomTypeID = r.RoomTypeID;
            Reservation = r.Reservation;
            RoomType = r.RoomType;
        }

        public int ID { get; set; }

        public int Number { get; set; }

        public int RoomTypeID { get; set; }

        public ICollection<ReservationDTO>? Reservation { get; set; }

        public virtual RoomType RoomType { get; set; }
    }
}
