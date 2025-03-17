using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserDTO
    {
        public UserDTO() { }

        public UserDTO(User u)
        {
            Id = u.Id;
            FullName = u.FullName;
            Passport = u.Passport;
            Discount = u.Discount;
            Reservation = u.Reservation?.Select(r => new ReservationDTO(r)).ToList();
        }

        public UserDTO(UserDTO u)
        {
            Id = u.Id;
            FullName = u.FullName;
            Passport = u.Passport;
            Discount = u.Discount;
            u.Reservation = Reservation;
        }

        public string Id { get; set; }

        public string FullName { get; set; }

        public string Passport { get; set; }

        public int Discount { get; set; }
        public ICollection<ReservationDTO>? Reservation { get; set; }

    }
}
