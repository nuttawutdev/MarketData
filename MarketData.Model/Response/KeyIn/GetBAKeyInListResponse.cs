using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.KeyIn
{
    public class GetBAKeyInListResponse : BaseGetDataListResponse
    {
        public List<BAKeyInData> data { get; set; }
    }

    public class BAKeyInData
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
