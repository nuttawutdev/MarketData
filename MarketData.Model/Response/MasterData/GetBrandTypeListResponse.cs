using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.MasterData
{
    public class GetBrandTypeListResponse : BaseResponse
    {
        public List<BrandTypeData> data { get; set; }
    }
}
