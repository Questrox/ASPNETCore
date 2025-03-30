using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public User()
        {
            Reservation = new HashSet<Reservation>();
        }

        public string FullName { get; set; }

        [StringLength(20)]
        public string Passport { get; set; }

        public int Discount { get; set; }
        [JsonIgnore]
        public virtual ICollection<Reservation> Reservation { get; set; }

    }
}
