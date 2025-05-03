using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RoomTypeImage
    {
        [Key]
        public int ID { get; set; }

        public string ImagePath { get; set; }
        public int RoomTypeID { get; set; }
        public virtual RoomType RoomType { get; set; }
    }
}
