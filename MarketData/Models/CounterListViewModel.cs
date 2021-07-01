using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class CounterListViewModel
    {
        public List<CounterViewModel> data { get; set; }
        public List<DepartmentStoreViewModel> departmentStoreList { get; set; }
        public List<BrandViewModel> brandList { get; set; }
        public List<DistributionChannelViewModel> channelList { get; set; }
    }
}
