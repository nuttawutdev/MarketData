using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class CounterViewModel
    {
        public Guid counterID { get; set; }
        public Guid distributionChannelID { get; set; }
        public string distributionChannelName { get; set; }
        public Guid retailerGroupID { get; set; }
        public string retailerGroupName { get; set; }
        public Guid departmentStoreID { get; set; }
        public string departmentStoreName { get; set; }
        public Guid brandID { get; set; }
        public string brandName { get; set; }
        public bool active { get; set; }
        public List<DepartmentStoreViewModel> departmentStoreList { get; set; }
        public List<BrandViewModel> brandList { get; set; }
        public List<DistributionChannelViewModel> channelList { get; set; }
    }
}
