using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class ApproveKeyInDetailViewModel
    {
        public Guid approveKeyInID { get; set; }
        public List<BAKeyInDetailData> BAKeyInDetailList { get; set; }
        public string departmentStore { get; set; }
        public string retailerGroup { get; set; }
        public string channel { get; set; }
        public string brand { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
        public string status { get; set; }
        public string universe { get; set; }
    }
}
