using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RoomTypeImageDTO
    {
        public RoomTypeImageDTO() { }
        public RoomTypeImageDTO(Domain.Entities.RoomTypeImage rtImage)
        {
            ID = rtImage.ID;
            ImagePath = rtImage.ImagePath;
            RoomTypeID = rtImage.RoomTypeID;
        }
        public int ID { get; set; }
        public string ImagePath { get; set; }
        public int RoomTypeID { get; set; }
    }
}
