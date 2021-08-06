using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.KeyIn
{
    public class CreateBAKeyInDetailResponse : BaseResponse
    {
        public bool isSuccess { get; set; }
        public Guid baKeyInID { get; set; }
    }
}
