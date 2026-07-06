using System;
using System.Collections.Generic;
using System.Text;
using WingmanDelivery.Models.Enums;

namespace WingmanDelivery.Models;

public class DeliveryOrderModel : BaseModel
{
    public required string f_pickup_address { get; set; }
    public OrderStatus f_status { get; set; }
    public decimal f_total_cost { get; set; }
}

