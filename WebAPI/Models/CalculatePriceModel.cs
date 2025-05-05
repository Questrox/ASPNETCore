using Domain.Models;

namespace WebAPI.Models
{
    /// <summary>
    /// Модель для расчета цены бронирования с учетом доп.услуг
    /// </summary>
    public class CalculatePriceModel
    {
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int RoomTypeID { get; set; }
        /// <summary>
        /// Данные о дополнительных услугах (цена, стоимость и сама услуга)
        /// </summary>
        public List<SelectedServiceItem> Services { get; set; }
    }
}
