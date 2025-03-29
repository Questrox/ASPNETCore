using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RoomTypeDTO
    {
        public RoomTypeDTO() { }
        public RoomTypeDTO(RoomType r) 
        {
            ID = r.ID;
            GuestCapacity = r.GuestCapacity;
            RoomCategoryID = r.RoomCategoryID;
            Price = r.Price;
            Description = r.Description;
            Room = r.Room.Select(r => new RoomDTO(r)).ToList();
            RoomCategory = r.RoomCategory;
        }
        public RoomTypeDTO(RoomTypeDTO r)
        {
            ID = r.ID;
            GuestCapacity = r.GuestCapacity;
            RoomCategoryID = r.RoomCategoryID;
            Price = r.Price;
            Description = r.Description;
            Room = r.Room;
            RoomCategory = r.RoomCategory;
        }
        public int ID { get; set; }

        public int GuestCapacity { get; set; }

        public int RoomCategoryID { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public virtual ICollection<RoomDTO>? Room { get; set; }

        public virtual RoomCategory RoomCategory { get; set; }
    }
}
