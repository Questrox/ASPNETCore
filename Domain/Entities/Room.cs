using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Room
    {
        public Room()
        {
            Reservation = new HashSet<Reservation>();
        }
        [Key]
        public int ID { get; set; }

        public int Number { get; set; }

        public int RoomTypeID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<Reservation> Reservation { get; set; }

        public virtual RoomType RoomType { get; set; }
    }
}
