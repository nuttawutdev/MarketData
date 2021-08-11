using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.Approve
{
    public class GetApproveKeyInListResponse : BaseResponse
    {
        public List<ApproveKeyInData> data { get; set; }
    }

    public class ApproveKeyInData
    {
        public Guid keyInID { get; set; }
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
