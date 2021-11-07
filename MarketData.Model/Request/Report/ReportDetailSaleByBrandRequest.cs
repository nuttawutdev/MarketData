using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.Report
{
    public class ReportDetailSaleByBrandRequest
    {
        public string startWeek { get; set; }
        public string endWeek { get; set; }
        public string startMonth { get; set; }
        public string endMonth { get; set; }
        public string startYear { get; set; }
        public string endYear { get; set; }
        public List<Guid> departmentStoreList { get; set; }
        public Guid brandID { get; set; }
        public string brandName { get; set; }
    }
}
