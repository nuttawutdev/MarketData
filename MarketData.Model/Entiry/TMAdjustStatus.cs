using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMAdjustStatus
    {
        [Key]
        public Guid ID { get; set; }
        public string Status_Name { get; set; }
        public bool Active_Flag { get; set; }
    }
}
