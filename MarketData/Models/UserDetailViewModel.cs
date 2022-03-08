using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class UserDetailViewModel
    {
        public Guid userID { get; set; }
        public string email { get; set; }
        public string displayName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool active { get; set; }
        public bool validateEmail { get; set; }
        public bool viewMaster { get; set; }
        public bool editMaster { get; set; }
        public bool editUser { get; set; }
        public bool viewData { get; set; }
        public bool keyInData { get; set; }
        public bool approveData { get; set; }
        public bool viewReport { get; set; }
        public bool officeUser { get; set; }
        public Guid? brandOfficeID { get; set; }
        public List<UserCounterViewModel> userCounter { get; set; } = new List<UserCounterViewModel>();
        public List<ChannelKeyInViewModel> channelList { get; set; } = new List<ChannelKeyInViewModel>();
        public List<DepartmentStoreKeyInViewModel> departmentStoreList { get; set; } = new List<DepartmentStoreKeyInViewModel>();
        public List<BrandKeyInViewModel> brandList { get; set; } = new List<BrandKeyInViewModel>();
    }


    public class UserCounterViewModel
    {
        public Guid userCounterID { get; set; }
        public string departmentStoreName { get; set; }
        public Guid departmentStoreID { get; set; }
        public string brandName { get; set; }
        public Guid brandID { get; set; }
        public string channelName { get; set; }
        public Guid channelID { get; set; }
    }
}
