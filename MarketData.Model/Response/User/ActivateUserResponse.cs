using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.User
{
    public class ActivateUserResponse : BaseResponse
    {
        public bool success { get; set; }
        public bool activated { get; set; }
        public bool urlExpire { get; set; }
    }
}
