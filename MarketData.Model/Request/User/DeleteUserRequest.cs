using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.User
{
    public class DeleteUserRequest
    {
        public Guid userID { get; set; }
        public Guid actionBy { get; set; }
    }
}
