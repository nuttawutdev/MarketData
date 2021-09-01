using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.User
{
    public class VerifyUrlResetPasswordResponse : BaseResponse
    {
        public Guid urlID { get; set; }
        public Guid userID { get; set; }
        public bool isSuccess { get; set; }
        public bool urlExpire { get; set; }
        public bool urlNotFound { get; set; }
        public bool unActive { get; set; }
    }
}
