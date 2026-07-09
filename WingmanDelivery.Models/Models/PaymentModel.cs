using System;
using System.Collections.Generic;
using System.Text;
using WingmanDelivery.Models.Enums;

namespace WingmanDelivery.Models
{
    public class PaymentModel : BaseModel
    {
        public Guid f_order_uid { get; set; }
        public decimal f_amount { get; set; }
        public PaymentMethod f_method { get; set; }
        public PaymentStatus f_status { get; set; }
    }
}
