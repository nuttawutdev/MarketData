using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.User
{
    public class GetUserListResponse : BaseResponse
    {
        public List<UserData> data { get; set; }
    }

    public class UserData
    {
        public Guid userID { get; set; }
        public string email { get; set; }
        public string displayName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool validateEmail { get; set; }
        public bool active { get; set; }
        public string lastLogin { get; set; }
        public List<Guid> departmentStoreID { get; set; }
        public List<Guid> brandID { get; set; }
    }
}
