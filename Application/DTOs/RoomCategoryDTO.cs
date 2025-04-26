using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RoomCategoryDTO
    {
        public int ID { get; set; }
        public string Category { get; set; }

        public RoomCategoryDTO() { }

        public RoomCategoryDTO(Domain.Entities.RoomCategory entity)
        {
            ID = entity.ID;
            Category = entity.Category;
        }
    }
}
