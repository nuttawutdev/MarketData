using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Data
{
    public class BrandData
    {
        public Guid brandID { get; set; }
        public string brandName { get; set; }
        public string brandGroupName { get; set; }
        public string brandSegmentName { get; set; }
        public string brandTypeName { get; set; }
        public string universe { get; set; }
        public string brandBirth { get; set; }
        public string color { get; set; }
        public bool isLorealBrand { get; set; }
        public bool active { get; set; }
    }
}
