using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class ApproveKeyInViewModel
    {
        public string htmlText { get; set; }
        public List<ChannelKeyInViewModel> channelList { get; set; } = new List<ChannelKeyInViewModel>();
        public List<RetailerGroupKeyInViewModel> retailerGroupList { get; set; } = new List<RetailerGroupKeyInViewModel>();
        public List<DepartmentStoreKeyInViewModel> departmentStoreList { get; set; } = new List<DepartmentStoreKeyInViewModel>();
        public List<BrandKeyInViewModel> brandList { get; set; } = new List<BrandKeyInViewModel>();
        public List<string> yearList { get; set; } = new List<string>();
        public List<StatusKeyInViewModel> statusList { get; set; } = new List<StatusKeyInViewModel>();
        public List<ApproveKeyInDataViewModel> data { get; set; } = new List<ApproveKeyInDataViewModel>();
    }

    public class ApproveKeyInDataViewModel
    {
        public Guid approveKeyInID { get; set; }
        public Guid baKeyInID { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
        public string departmentStoreName { get; set; }
        public string brandName { get; set; }
        public Guid retailerGroupID { get; set; }
        public Guid departmentStoreID { get; set; }
        public Guid brandID { get; set; }
        public Guid distributionChannelID { get; set; }
        public string distributionChannelName { get; set; }
        public Guid statusID { get; set; }
        public string statusName { get; set; }
        public string approver { get; set; }
        public string approveDate { get; set; }
        public string universe { get; set; }
    }
}
