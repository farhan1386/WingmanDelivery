using System;
using System.Collections.Generic;
using System.Text;

namespace WingmanDelivery.Models
{
    public class FilterModel
    {
        public string? FilterString { get; set; }
        public string? SearchValue { get; set; }
        public string? OrderField { get; set; }
        public string? Order { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
    }
}
