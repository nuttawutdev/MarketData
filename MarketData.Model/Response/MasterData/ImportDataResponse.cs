using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.MasterData
{
    public class ImportDataResponse : BaseResponse
    {
        public bool isSuccess { get; set; }
        public bool wrongFormatFile { get; set; }
    }
}
