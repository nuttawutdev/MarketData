using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.User
{
    public class LoginResponse
    {
        public Guid? userID { get; set; }
        public string role { get; set; }
        public bool isSuccess { get; set; }
        public bool wrongPassword { get; set; }
        public bool emailNotExist { get; set; }
        public bool userLocked { get; set; }
        public GetUserDetailResponse userDetail { get; set; }
    }
}
