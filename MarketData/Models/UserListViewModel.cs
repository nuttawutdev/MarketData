using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class UserListViewModel
    {
        public List<UserListData> data { get; set; }
    }

    public class UserListData
    {
        public Guid userID { get; set; }
        public string email { get; set; }
        public string displayName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool validateEmail { get; set; }
        public bool active { get; set; }
        public string lastLogin { get; set; }
        public string departmentStoreID { get; set; }
        public string brandID { get; set; }
    }
}
