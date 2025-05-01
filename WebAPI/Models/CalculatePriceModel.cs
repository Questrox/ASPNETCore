using Domain.Models;

namespace WebAPI.Models
{
    public class CalculatePriceModel
    {
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int RoomTypeID { get; set; }
        public List<SelectedServiceItem> Services { get; set; }
    }
}
