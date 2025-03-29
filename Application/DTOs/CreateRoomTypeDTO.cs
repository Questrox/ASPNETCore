using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateRoomTypeDTO
    {
        public int GuestCapacity { get; set; }

        public int RoomCategoryID { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }
    }
}
