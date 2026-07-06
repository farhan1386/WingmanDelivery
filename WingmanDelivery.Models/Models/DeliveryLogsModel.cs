using System;
using System.Collections.Generic;
using System.Text;

namespace WingmanDelivery.Models
{
    public class DeliveryLogsModel : BaseModel
    {
        public Guid f_delivery_uid { get; set; }
        public int f_status_from { get; set; }
        public int f_status_to { get; set; }
    }
}
