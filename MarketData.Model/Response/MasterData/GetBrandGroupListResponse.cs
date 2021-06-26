using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.MasterData
{
    public class GetBrandGroupListResponse : BaseGetDataListResponse
    {
        public List<BrandGroupData> data { get; set; }
    }
}
