using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.User
{
    public class GetUserDetailResponse : BaseResponse
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
        public List<UserCounter> userCounter { get; set; }
        public List<DepartmentStoreData> departmentStore { get; set; }
        public List<BrandData> brand { get; set; }
        public List<DistributionChannelData> channel { get; set; }
    }

    public class UserCounter
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
