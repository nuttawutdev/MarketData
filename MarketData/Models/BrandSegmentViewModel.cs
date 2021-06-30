using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class BrandSegmentViewModel
    {
        public Guid brandSegmentID { get; set; }
        public string brandSegmentName { get; set; }
        public bool active { get; set; }
    }
}
