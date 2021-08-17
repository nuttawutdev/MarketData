using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class AdjustListViewModel
    {
        public List<AdjustViewData> data { get; set; }
        public List<string> columnList { get; set; }
    }

    public class AdjustViewData
    {
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
        public string counter { get; set; }
        public Guid retailerGroupID { get; set; }
        public string retailerGroupName { get; set; }
        public Guid departmentStoreID { get; set; }
        public string departmentStoreName { get; set; }
        public Guid distributionChannelID { get; set; }
        public string distributionChannelName { get; set; }
        public Guid statusID { get; set; }
        public string statusName { get; set; }
        public Dictionary<string, string> brandStatus { get; set; }

    }
}
