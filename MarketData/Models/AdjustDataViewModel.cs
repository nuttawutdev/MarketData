using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class AdjustDataViewModel
    {
        public List<ChannelKeyInViewModel> channelList { get; set; } = new List<ChannelKeyInViewModel>();
        public List<RetailerGroupKeyInViewModel> retailerGroupList { get; set; } = new List<RetailerGroupKeyInViewModel>();
        public List<DepartmentStoreKeyInViewModel> departmentStoreList { get; set; } = new List<DepartmentStoreKeyInViewModel>();
        public List<BrandKeyInViewModel> brandList { get; set; } = new List<BrandKeyInViewModel>();
        public List<string> yearList { get; set; } = new List<string>();
        public List<StatusKeyInViewModel> statusList { get; set; } = new List<StatusKeyInViewModel>();
    }
}
