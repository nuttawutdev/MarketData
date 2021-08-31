using MarketData.Helper;
using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Request.User;
using MarketData.Model.Response;
using MarketData.Model.Response.User;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            catch (Exception ex)
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

                foreach (var itemUser in allUser)
                {
                    UserData userData = new UserData
                    {
                        userID = itemUser.ID,
                        email = itemUser.Email,
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
                        var departmetList = userCounterData.Select(c => c.DepartmentStore_ID).GroupBy(e => e).Select(g => g.Key);
                        var brandList = userCounterData.Select(e => e.Brand_ID);
                        userData.departmentStoreID = departmetList.ToList();
                        userData.brandID = brandList.ToList();
                    }

                    userDataList.Add(userData);
                }

                response.data = userDataList;
            }
            catch (Exception ex)
            {
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public GetUserDetailResponse GetUserDetail(Guid userID)
        {
            GetUserDetailResponse response = new GetUserDetailResponse();

            try
            {
                var userData = repository.user.FindUserBy(c => c.ID == userID);
                var userCounterData = repository.user.GetUserCounterBy(e => e.User_ID == userID);
                var getDepartmentStoreResponse = repository.masterData.GetDepartmentStoreListBy(c => c.Active_Flag);
                var getBrandResponse = repository.masterData.GetBrandListBy(c => c.Active_Flag);
                var channelResponse = repository.masterData.GetDistributionChannelListBy(c => c.Active_Flag && c.Delete_Flag != true);

                response.userID = userData.ID;
                response.email = userData.Email;
                response.active = userData.ActiveFlag;
                response.displayName = userData.DisplayName;
                response.firstName = userData.FirstName;
                response.lastName = userData.LastName;
                response.validateEmail = userData.ValidateEmailFlag;
                response.viewMaster = userData.ViewMasterPermission;
                response.editMaster = userData.EditMasterPermission;
                response.editUser = userData.EditUserPermission;
                response.viewData = userData.ViewDataPermission;
                response.keyInData = userData.KeyInDataPermission;
                response.approveData = userData.ApprovePermission;
                response.viewReport = userData.ViewReportPermission;
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
                response.channel = channelResponse.Select(c => new DistributionChannelData
                {
                    distributionChannelID = c.Distribution_Channel_ID,
                    distributionChannelName = c.Distribution_Channel_Name
                }).OrderBy(r => r.distributionChannelName).ToList();

                if (userCounterData != null && userCounterData.Any())
                {
                    response.userCounter = userCounterData.Select(c => new UserCounterData
                    {
                        userCounterID = c.ID,
                        departmentStoreID = c.DepartmentStore_ID,
                        departmentStoreName = getDepartmentStoreResponse.FirstOrDefault(e=>e.Department_Store_ID == c.DepartmentStore_ID).Department_Store_Name,
                        brandID = c.Brand_ID,
                        brandName = getBrandResponse.FirstOrDefault(b=>b.Brand_ID == c.Brand_ID).Brand_Name,
                        channelID = c.DistributionChannel_ID,
                        channelName = channelResponse.FirstOrDefault(r=>r.Distribution_Channel_ID == c.DistributionChannel_ID).Distribution_Channel_Name
                    }).ToList();
                }
            }
            catch(Exception ex)
            {
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> SaveUserData(SaveUserDataRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                if(request.userID != null && request.userID != Guid.Empty)
                {
                    var createUserResponse = await repository.user.CreateNewUser(request);

                    if(createUserResponse != null)
                    {
                        await repository.user.RemoveAllUserCounterByID(createUserResponse.ID);

                    }
                }
                else
                {

                }
            }
            catch(Exception ex)
            {

            }

            return response;
        }

        //private async Task<bool> CreateUserCounterData(Guid userID,List<UserCounterData> userCounterData)
        //{
        //    var dateNow = Utility
        //    List<TMUserCounter> userCounterList = userCounterData.Select(e => new TMUserCounter
        //    {
        //        ID = 
        //    }).ToList();
        //}
    }
}
