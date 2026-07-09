namespace WingmanDelivery.Models
{
    public class MenuItemModel : BaseModel
    {
        public Guid f_restaurant_uid { get; set; }
        public required string f_name { get; set; }
        public decimal f_price { get; set; }
    }
}
