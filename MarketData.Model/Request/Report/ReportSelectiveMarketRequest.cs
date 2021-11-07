using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.Report
{
    public class ReportSelectiveMarketRequest
    {
        public string startWeek { get; set; }
        public string endWeek { get; set; }
        public string startMonth { get; set; }
        public string endMonth { get; set; }
        public string startYear { get; set; }
        public string endYear { get; set; }
        public string compareYear { get; set; }
        public List<Guid> departmentStoreList { get; set; }
        public Guid? brandType { get; set; }
        public string universe { get; set; }
        public string saleType { get; set; }
        public bool lorealStore { get; set; }
        public bool preview { get; set; }
    }
}
