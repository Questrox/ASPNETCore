using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Reservation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Reservation()
        {
        }

        [Key]
        public int ID { get; set; }

        [Column(TypeName = "date")]
        public DateTime ArrivalDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime DepartureDate { get; set; }

        public decimal FullPrice { get; set; }
        public decimal LivingPrice { get; set; }
        public decimal ServicesPrice { get; set; }

        public int RoomID { get; set; }

        public string UserID { get; set; }
        public int ReservationStatusID { get; set; }
        public virtual Room Room { get; set; }
        public virtual User User { get; set; }
        public virtual ReservationStatus ReservationStatus { get; set; }
    }
}
