using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.MasterData
{
    public class GetKeyInStatusResponse : BaseResponse
    {
        public List<KeyInStatusData> data { get; set; }
    }
}
