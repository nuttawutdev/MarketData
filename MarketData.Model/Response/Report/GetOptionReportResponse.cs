using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.Report
{
    public class GetOptionReportResponse
    {
        public List<DepartmentStoreData> departmentStore { get; set; }
        public List<BrandTypeData> brandType { get; set; }
        public List<string> year { get; set; } = new List<string>();
        public List<BrandData> brandList { get; set; }
    }
}
