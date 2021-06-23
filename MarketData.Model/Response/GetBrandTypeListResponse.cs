using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response
{
    public class GetBrandTypeListResponse : BaseGetDataListResponse
    {
        public List<BrandTypeData> data { get; set; }
    }
}
