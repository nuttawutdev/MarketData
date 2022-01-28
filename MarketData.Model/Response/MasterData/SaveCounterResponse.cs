using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.MasterData
{
    public class SaveCounterResponse : BaseResponse
    {
        public bool isSuccess { get; set; }
        public bool isDuplicated { get; set; }
        public int countSuccess { get; set; }
        public int countFailed { get; set; }
        public List<string> result { get; set; }
    }
}
