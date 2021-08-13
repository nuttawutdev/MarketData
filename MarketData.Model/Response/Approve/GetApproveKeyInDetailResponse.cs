using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.Approve
{
    public class GetApproveKeyInDetailResponse : BaseResponse
    {
        public Guid approveKeyInID { get; set; }
        public string departmentStore { get; set; }
        public string retailerGroup { get; set; }
        public string channel { get; set; }
        public string brand { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
        public string status { get; set; }
        public string universe { get; set; }
        public List<BAKeyInDetailData> data { get; set; }
    }
}
