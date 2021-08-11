using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class ApproveKeyInListViewModel
    {
        public List<ApproveKeyInDataViewModel> data { get; set; }
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
    }
}
