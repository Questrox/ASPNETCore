using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ServiceStringDTO
    {
        public ServiceStringDTO() { }
        public ServiceStringDTO(ServiceString s)
        {
            ID = s.ID;
            Count = s.Count;
            AdditionalServiceID = s.AdditionalServiceID;
            ReservationID = s.ReservationID;
            Price = s.Price;
            ServiceStatusID = s.ServiceStatusID;
            AdditionalService = s.AdditionalService;
            Reservation = s.Reservation;
            ServiceStatus = s.ServiceStatus;
        }
        public ServiceStringDTO(ServiceStringDTO s)
        {
            ID = s.ID;
            Count = s.Count;
            DeliveredCount = s.DeliveredCount;
            AdditionalServiceID = s.AdditionalServiceID;
            ReservationID = s.ReservationID;
            Price = s.Price;
            ServiceStatusID = s.ServiceStatusID;
            AdditionalService = s.AdditionalService;
            Reservation = s.Reservation;
            ServiceStatus = s.ServiceStatus;
        }
        public int ID { get; set; }

        public int Count { get; set; }

        public int DeliveredCount { get; set; }
        public int AdditionalServiceID { get; set; }

        public int ReservationID { get; set; }

        public decimal Price { get; set; }
        public int ServiceStatusID { get; set; }

        public virtual AdditionalService AdditionalService { get; set; }
        [JsonIgnore]
        public virtual Reservation? Reservation { get; set; }
        public virtual ServiceStatus ServiceStatus { get; set; }
    }
}
