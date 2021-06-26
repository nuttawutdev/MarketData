using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.MasterData
{
    public class GetBrandListResponse : BaseGetDataListResponse
    {
        public List<BrandData> data { get; set; }
    }
}
