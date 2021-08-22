using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.Adjust
{
    public class GetAdjustDetailRequest
    {
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
        public Guid departmentStoreID { get; set; }
        public Guid distributionChannelID { get; set; }
        public string universe { get; set; }
    }
}
