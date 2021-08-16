using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.AdjustData
{
    public class GetAdjustDataListModel : BaseResponse
    {
        public List<AdjustData> data { get; set; }
        public List<string> columnList { get; set; }
    }

    public class AdjustData
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
        public Dictionary<string,string> brandStatus { get; set; }

    }
}
