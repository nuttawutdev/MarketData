using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.Approve
{
    public class RejectKeyInDataRequest
    {
        public Guid approveKeyInID { get; set; }
        public string remark { get; set; }
        public Guid userID { get; set; }
        public string baRemark { get; set; }
    }
}
