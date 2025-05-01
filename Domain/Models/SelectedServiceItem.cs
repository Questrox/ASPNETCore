using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SelectedServiceItem
    {
        public int Count { get; set; }
        public int AdditionalServiceID { get; set; }
        public decimal Price { get; set; }
    }
}
