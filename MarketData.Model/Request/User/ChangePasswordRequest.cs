using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.User
{
    public class ChangePasswordRequest
    {
        public Guid userID { get; set; }
        public string password { get; set; }
        public Guid urlID { get; set; }
    }
}
