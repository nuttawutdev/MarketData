using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMBrandSummary
    {
        [Key]
        public Guid ID { get; set; }
        public Guid Brand_ID { get; set; }
        public Guid Brand_ID_Include { get; set; }
    }
}
