using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.MasterData
{
    public class GetRetailerGroupListResponse : BaseResponse
    {
        public List<RetailerGroupData> data { get; set; }
    }
}
