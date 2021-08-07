using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.User
{
    public class LoginResponse
    {
        public Guid? userID { get; set; }
        public string role { get; set; }
    }
}
