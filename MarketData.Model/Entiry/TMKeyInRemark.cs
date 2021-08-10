using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMKeyInRemark
    {
        [Key]
        public Guid ID { get; set; }
        public string Remark { get; set; }
        public bool Active_Flag { get; set; }
    }
}
