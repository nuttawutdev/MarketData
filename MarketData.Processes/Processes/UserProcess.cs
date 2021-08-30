using MarketData.Model.Data;
using MarketData.Model.Request.User;
using MarketData.Model.Response.User;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarketData.Processes.Processes
{
    public class UserProcess
    {
        private readonly Repository repository;

        public UserProcess(Repository repository)
        {
            this.repository = repository;
        }

        public LoginResponse Login(LoginRequest request)
        {
            LoginResponse response = new LoginResponse();

            try
            {
                var userData = repository.user.FindUserBy(c => c.Email.ToLower() == request.userName.ToLower());

                if (userData != null)
                {
                    response.userID = userData.ID;
                    response.role = request.userName.ToLower().Contains("admin") ? "Admin" : "BA";
                }

            }
            catch (Exception ex)
            {

            }

            return response;
        }

        public GetUserOptionResponse GetUserOption()
        {
            GetUserOptionResponse response = new GetUserOptionResponse();

            try
            {
                var getDepartmentStoreResponse = repository.masterData.GetDepartmentStoreListBy(c => c.Active_Flag);
                var getBrandResponse = repository.masterData.GetBrandListBy(c => c.Active_Flag);

                response.departmentStore = getDepartmentStoreResponse.Select(c => new DepartmentStoreData
                {
                    departmentStoreID = c.Department_Store_ID,
                    departmentStoreName = c.Department_Store_Name,
                    distributionChannelID = c.Distribution_Channel_ID,
                    retailerGroupID = c.Retailer_Group_ID
                }).OrderBy(r => r.departmentStoreName).ToList();
                response.brand = getBrandResponse.Select(c => new BrandData
                {
                    brandID = c.Brand_ID,
                    brandName = c.Brand_Name
                }).OrderBy(r => r.brandName).ToList();
            }
            catch(Exception ex)
            {
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public GetUserListResponse GetUserList()
        {
            GetUserListResponse response = new GetUserListResponse();

            try
            {
                var allUser = repository.user.GetUserBy(c => c.ID != Guid.Empty);
                var userIDList = allUser.Select(e => e.ID);
                var allUserCounter = repository.user.GetUserCounterBy(c => userIDList.Contains(c.User_ID));

                List<UserData> userDataList = new List<UserData>();

                foreach(var itemUser in allUser)
                {
                    UserData userData = new UserData
                    {
                        userID = itemUser.ID,
                        displayName = itemUser.DisplayName,
                        firstName = itemUser.FirstName,
                        lastName = itemUser.LastName,
                        lastLogin = itemUser.Last_Login.HasValue ? itemUser.Last_Login.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                        active = itemUser.ActiveFlag,
                        validateEmail = itemUser.ValidateEmailFlag
                    };

                    var userCounterData = allUserCounter.Where(r => r.User_ID == itemUser.ID);

                    if (userCounterData.Any())
                    {
                        var departmetList = userCounterData.Select(c => c.DepartmentStore_ID);
                        var brandList = userCounterData.Select(e => e.Brand_ID);

                        userData.departmentStoreID = departmetList.ToList();
                        userData.brandID = brandList.ToList();
                    }

                    userDataList.Add(userData);
                }

                response.data = userDataList;
            }
            catch(Exception ex)
            {
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }
    }
}
