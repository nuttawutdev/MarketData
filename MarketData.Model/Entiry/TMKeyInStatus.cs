using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMKeyInStatus
    {
        [Key]
        public Guid ID { get; set; }
        public char Status_Name { get; set; }
        public bool Active_Flag { get; set; }
    }
}
