using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;


namespace WingmanDelivery.Models
{
    public class InvokeDataModel
    {
        public Guid userUid { get; set; }
        public required string schema { get; set; }
        public int? commandTimeout { get; set; }

        [JsonIgnore]
        public IConfiguration? configuration { get; set; }
    }
}
