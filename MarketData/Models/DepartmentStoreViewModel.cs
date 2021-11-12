using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class DepartmentStoreViewModel
    {
        public Guid departmentStoreID { get; set; }
        public string departmentStoreName { get; set; }
        public Guid retailerGroupID { get; set; }
        public string retailerGroupName { get; set; }
        public Guid distributionChannelID { get; set; }
        public string distributionChannelName { get; set; }
        public Guid regionID { get; set; }
        public string region { get; set; }
        public int? rank { get; set; }
        public bool active { get; set; }
        public int? topNumber { get; set; }
        public List<RetailerGroupViewModel> retailerGroupList { get; set; }
        public List<DistributionChannelViewModel> channelList { get; set; }
        public List<RegionViewModel> regionList { get; set; }
    }
}
