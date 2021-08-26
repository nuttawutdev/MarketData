using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.AdjustData
{
    public class GetAdjustDetailResponse : BaseResponse
    {
        public List<AdjustDetailData> data { get; set; }
        public Guid adjustDataID { get; set; }
        public string departmentStore { get; set; }
        public string retailerGroup { get; set; }
        public string channel { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
        public string status { get; set; }
        public string universe { get; set; }
        public List<string> brandDataColumn { get; set; }
        public Dictionary<string,decimal?> brandTotalAmount { get; set; }
    }

    public class AdjustDetailData
    {
        public Guid brandID { get; set; }
        public string brandName { get; set; }
        public decimal? amountPreviousYear { get; set; }
        public decimal? adminAmountSale { get; set; }
        public decimal? adjustAmountSale { get; set; }
        public decimal? adjustWholeSale { get; set; }
        public string brandColor { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
        public int rank { get; set; }
        public decimal? sk { get; set; }
        public decimal? mu { get; set; }
        public decimal? fg { get; set; }
        public decimal? ot { get; set; }
        public string remark { get; set; }
        public Dictionary<string,decimal?> brandKeyInAmount { get; set; }
        public Dictionary<string, string> brandKeyInRank { get; set; }

        // ((adjustAmountSale - amountPreviousYear) / amountPreviousYear) X 100
        public decimal? percentGrowth { get; set; }
    }
}
