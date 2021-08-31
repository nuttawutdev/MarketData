using MarketData.Helper;
using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Request.User;
using MarketData.Model.Response;
using MarketData.Model.Response.User;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MarketData.Processes.Processes
{
    public class UserProcess
    {
        private readonly Repository repository;
        private static Random random = new Random();

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
                        departmentStoreName = getDepartmentStoreResponse.FirstOrDefault(e => e.Department_Store_ID == c.DepartmentStore_ID).Department_Store_Name,
                        brandID = c.Brand_ID,
                        brandName = getBrandResponse.FirstOrDefault(b => b.Brand_ID == c.Brand_ID).Brand_Name,
                        channelID = c.DistributionChannel_ID,
                        channelName = channelResponse.FirstOrDefault(r => r.Distribution_Channel_ID == c.DistributionChannel_ID).Distribution_Channel_Name
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> SaveUserData(SaveUserDataRequest request,string hostUrl)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                request.email = request.email.Trim();
                request.displayName = request.displayName.Trim();

                var userByEmail = repository.user.FindUserBy(c =>
                c.Email.ToLower() == request.email.ToLower());

                var userByDisplatName = repository.user.FindUserBy(c =>
               c.DisplayName.ToLower() == request.displayName.ToLower());

                if((userByEmail == null || (userByEmail != null && userByEmail.ID == request.userID))
                   && (userByDisplatName == null || (userByDisplatName != null && userByDisplatName.ID == request.userID)))
                {
                    if (request.userID == null || request.userID == Guid.Empty)
                    {
                        var generatePassword = RandomPassword();
                        var passwordEncrypt = Utility.Encrypt(generatePassword);

                        var createUserResponse = await repository.user.CreateNewUser(request, passwordEncrypt);

                        if (createUserResponse != null)
                        {
                            if(request.userCounter != null && request.userCounter.Any())
                            {
                                await repository.user.RemoveAllUserCounterByID(createUserResponse.ID);
                                await CreateUserCounterData(createUserResponse.ID, request.userCounter);
                            }
                           
                            SendEmailActivateUser(request.email, hostUrl, createUserResponse.ID.ToString(), generatePassword);
                            response.isSuccess = true;
                        }
                        else
                        {
                            response.isSuccess = false;
                        }
                    }
                    else
                    {
                        var updateUserResult = await repository.user.UpdateUserData(request);
                        if (updateUserResult)
                        {
                            await repository.user.RemoveAllUserCounterByID(request.userID.GetValueOrDefault());
                            await CreateUserCounterData(request.userID.GetValueOrDefault(), request.userCounter);

                            response.isSuccess = true;
                        }
                        else
                        {
                            response.isSuccess = false;
                        }
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.isDuplicated = true;
                }              
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        private async Task<bool> CreateUserCounterData(Guid userID, List<UserCounterData> userCounterData)
        {
            var dateNow = Utility.GetDateNowThai();

            List<TMUserCounter> userCounterList = userCounterData.Select(e => new TMUserCounter
            {
                ID = Guid.NewGuid(),
                Brand_ID = e.brandID,
                DepartmentStore_ID = e.departmentStoreID,
                DistributionChannel_ID = e.channelID,
                User_ID = userID
            }).ToList();

            return await repository.user.CreateUserCounter(userCounterList);
        }

        private bool SendEmailActivateUser(string emailTo, string hostUrl, string userID,string passwordUser)
        {
            try
            {
                string smtpHost = "smtp.gmail.com";
                int port = Int32.Parse("587");
                string userName = "developernuttawut@gmail.com";
                string password = "@min1234";

                // Send Email
                string url = $"{hostUrl}/Users/ActivateUser?userID={userID}";
                string htmlBody = string.Empty;
                //string emailTemplatePath = Path.GetFullPath(Path.Combine("Views\\Email\\EmailTemplateConfirmRegister.html"));
                //using (StreamReader reader = File.OpenText(emailTemplatePath))
                //{
                //    htmlBody = reader.ReadToEnd();
                //}

                //htmlBody = htmlBody.Replace("linkRegister", url);
                MailMessage m = new MailMessage();
                m.From = new MailAddress("developernuttawut@gmail", "Admin");
                m.To.Add(emailTo);
                m.Subject = "Activate User​";
                m.Body =$"{url} {passwordUser}";
                //m.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    smtp.Host = smtpHost;
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = true;
                    smtp.Send(m);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string RandomPassword()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
