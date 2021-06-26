using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class GetBrandGroupListRequest : BaseGetDataListRequest
    {
        public string isLoreal { get; set; }
    }
}
