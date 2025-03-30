using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ServiceStatus
    {
        public ServiceStatus() 
        {
            ServiceString = new HashSet<ServiceString>();
        }
        [Key]
        public int ID { get; set; }
        [StringLength(40)]
        public string Status { get; set; }
        [JsonIgnore]
        public virtual ICollection<ServiceString> ServiceString { get; set; }
    }
}
