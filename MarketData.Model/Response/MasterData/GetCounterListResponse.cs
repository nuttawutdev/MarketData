using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.MasterData
{
    public class GetCounterListResponse : BaseResponse
    {
        public List<CounterData> data { get; set; }
    }
}
