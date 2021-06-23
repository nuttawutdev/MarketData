using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response
{
    public class GetBrandTypeListResponse : BaseGetDataListResponse
    {
        public List<BrandTypeData> data { get; set; }
    }

    public class BrandTypeData
    {
        public string brandTypeName { get; set; }
        public bool active { get; set; }
    }
}
