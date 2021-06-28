using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMRegion
    {
        [Key]
        public Guid Region_ID { get; set; }
        public string Region_Name { get; set; }
    }
}
