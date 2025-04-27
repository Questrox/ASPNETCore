using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AdditionalServiceDTO
    {
        public AdditionalServiceDTO() { }

        public AdditionalServiceDTO(AdditionalService service)
        {
            ID = service.ID;
            Name = service.Name;
            Price = service.Price;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
