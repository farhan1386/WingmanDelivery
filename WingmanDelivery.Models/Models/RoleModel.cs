using System;
using System.Collections.Generic;
using System.Text;

namespace WingmanDelivery.Models
{
    public class RoleModel : BaseModel
    {
        public required string f_role_name { get; set; }
        public required string f_description { get; set; }
        public bool f_active { get; set; }
    }
}