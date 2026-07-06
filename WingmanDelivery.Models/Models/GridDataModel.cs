using System;
using System.Collections.Generic;
using System.Text;

namespace WingmanDelivery.Models
{
    public class GridDataModel<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
