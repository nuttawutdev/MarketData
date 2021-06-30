using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class BrandViewModel
    {
        public Guid brandID { get; set; }
        public string brandName { get; set; }
        public string brandShortName { get; set; }
        public Guid brandGroupID { get; set; }
        public string brandGroupName { get; set; }
        public Guid brandSegmentID { get; set; }
        public string brandSegmentName { get; set; }
        public Guid brandTypeID { get; set; }
        public string brandTypeName { get; set; }
        public string universe { get; set; }
        public string brandBirth { get; set; }
        public string color { get; set; }
        public bool isLorealBrand { get; set; }
        public int? lorealBrandRank { get; set; }
        public bool active { get; set; }
        public string userID { get; set; }
        public List<BrandTypeViewModel> brandTypeList { get; set; }
        public List<BrandGroupViewModel> brandGroupList { get; set; }
        public List<BrandSegmentViewModel> brandSegmentList { get; set; }
    }
}
