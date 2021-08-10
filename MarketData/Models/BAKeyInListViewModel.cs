using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class BAKeyInListViewModel
    {
        public List<BAKeyInDataViewModel> data { get; set; } = new List<BAKeyInDataViewModel>();
        public List<ChannelKeyInViewModel> channelList { get; set; } = new List<ChannelKeyInViewModel>();
        public List<RetailerGroupKeyInViewModel> retailerGroupList { get; set; } = new List<RetailerGroupKeyInViewModel>();
        public List<DepartmentStoreKeyInViewModel> departmentStoreList { get; set; } = new List<DepartmentStoreKeyInViewModel>();
        public List<BrandKeyInViewModel> brandList { get; set; } = new List<BrandKeyInViewModel>();
        public List<string> yearList { get; set; } = new List<string>();
        public List<StatusKeyInViewModel> statusList { get; set; } = new List<StatusKeyInViewModel>();
        public Guid userID { get; set; }
    }

    public class BAKeyInDataViewModel
    {
        public Guid keyInID { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
        public string counter { get; set; }
        public Guid retailerGroupID { get; set; }
        public Guid departmentStoreID { get; set; }
        public Guid brandID { get; set; }
        public Guid distributionChannelID { get; set; }
        public string distributionChannelName { get; set; }
        public Guid statusID { get; set; }
        public string statusName { get; set; }
        public string lastEdit { get; set; }
        public string approver { get; set; }
        public string submitDate { get; set; }
        public string approveDate { get; set; }
        public string remark { get; set; }
    }
}
