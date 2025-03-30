using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RoomType
    {
        public RoomType()
        {
            Room = new HashSet<Room>();
        }
        [Key]
        public int ID { get; set; }

        public int GuestCapacity { get; set; }

        public int RoomCategoryID { get; set; }

        public decimal Price { get; set; }

        [StringLength(400)]
        public string Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<Room> Room { get; set; }

        public virtual RoomCategory RoomCategory { get; set; }
    }
}
