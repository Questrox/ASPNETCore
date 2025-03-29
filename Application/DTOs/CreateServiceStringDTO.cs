using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateServiceStringDTO
    {
        public int Count { get; set; }

        public int AdditionalServiceID { get; set; }

        public int ReservationID { get; set; }

        public decimal Price { get; set; }
        public int ServiceStatusID { get; set; }
    }
}
