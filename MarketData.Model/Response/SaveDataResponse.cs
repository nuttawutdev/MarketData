using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response
{
    public class SaveDataResponse : BaseResponse
    {
        public bool isSuccess { get; set; }
        public bool? isDuplicated { get; set; }
        public bool? notExistEmail { get; set; }
    }
}
