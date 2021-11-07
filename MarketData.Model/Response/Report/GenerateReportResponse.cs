using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.Report
{
    public class GenerateReportResponse : BaseResponse
    {
        public bool success { get; set; }
        public byte[] fileContent { get; set; }
        public string filePreview { get; set; }
        public string fileName { get; set; }
    }
}
