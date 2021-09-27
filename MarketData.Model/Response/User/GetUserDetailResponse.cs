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
        public List<UserCounterData> userCounter { get; set; }
        public List<DepartmentStoreData> departmentStore { get; set; }
        public List<BrandData> brand { get; set; }
        public List<DistributionChannelData> channel { get; set; }
    }
}
