using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.User
{
    public class LoginResponse
    {
        public bool isSuccess { get; set; }
        public bool wrongPassword { get; set; }
        public bool emailNotExist { get; set; }
        public bool userLocked { get; set; }
        public bool userNotValidate { get; set; }
        public bool userOnline { get; set; }
        public string tokenID { get; set; }
        public GetUserDetailResponse userDetail { get; set; }
    }
}
