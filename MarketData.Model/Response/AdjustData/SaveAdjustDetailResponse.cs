using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.AdjustData
{
    public class SaveAdjustDetailResponse : BaseResponse
    {
        public bool isSuccess { get; set; }
        public Guid adjustDataID { get; set; }
    }
}
