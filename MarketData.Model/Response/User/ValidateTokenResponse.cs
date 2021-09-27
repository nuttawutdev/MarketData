using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.User
{
    public class ValidateTokenResponse
    {
        public bool isValid { get; set; }
        public bool tokenExpire { get; set; }
        public bool notExistToken { get; set; }
    }
}
