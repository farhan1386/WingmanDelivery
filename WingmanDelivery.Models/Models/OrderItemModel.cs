using System;
using System.Collections.Generic;
using System.Text;

namespace WingmanDelivery.Models
{
    public class OrderItemModel : BaseModel
    {
        public Guid f_order_uid { get; set; }
        public Guid f_menu_item_uid { get; set; }
        public int f_quantity { get; set; }
    }
}