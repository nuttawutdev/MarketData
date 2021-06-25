using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class BrandTypeViewModel
    {
        public Guid brandTypeID { get; set; }
        public string brandTypeName { get; set; }
        public bool active { get; set; }
        public DateTime? createdDate { get; set; }
    }
}
