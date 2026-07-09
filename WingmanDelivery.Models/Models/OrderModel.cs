using System;
using System.Collections.Generic;
using System.Text;
using WingmanDelivery.Models.Enums;

namespace WingmanDelivery.Models
{
    public class OrderModel : BaseModel
    {
        public Guid f_customer_uid { get; set; }
        public Guid f_restaurant_uid { get; set; }
        public OrderStatus f_status { get; set; }
        public decimal f_total_amount { get; set; }
    }
}
