using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.KeyIn
{
    public class GetAdminKeyInResponse : BaseResponse
    {
        public List<AdminKeyInDetailData> data { get; set; }
        public string totalAmountPreviosYear { get; set; }
    }
}
