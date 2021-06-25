using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Data
{
    public class BrandSegmentData
    {
        public Guid brandSegmentID { get; set; }
        public string brandSegmentName { get; set; }
        public bool active { get; set; }
        public DateTime? createdDate { get; set; }
    }
}
