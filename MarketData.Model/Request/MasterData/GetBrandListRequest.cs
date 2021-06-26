using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class GetBrandListRequest : BaseGetDataListRequest
    {
        public string searchBy { get; set; }
    }
}
