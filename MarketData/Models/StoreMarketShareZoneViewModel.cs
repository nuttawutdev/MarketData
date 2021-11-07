using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class StoreMarketShareZoneViewModel
    {
        public List<BrandTypeViewModel> brandTypeList { get; set; }
        public List<DepartmentStoreViewModel> departmentStoreList { get; set; }
        public List<string> yearList { get; set; } = new List<string>();
    }
}
