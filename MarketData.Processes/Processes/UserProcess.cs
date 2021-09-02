using ExcelDataReader;
using MarketData.Helper;
using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Request.MasterData;
using MarketData.Model.Request.User;
using MarketData.Model.Response;
using MarketData.Model.Response.MasterData;
using MarketData.Model.Response.User;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static MarketData.Helper.Utility;

namespace MarketData.Processes.Processes
{
    public class UserProcess
    {
        private readonly Repository repository;
        private static Random random = new Random();
        private AppSettingHelper appsettingHelper;
        private readonly string smtpHost, userName, password;
        private readonly int port;
        public UserProcess(Repository repository)
        {
            this.repository = repository;
            appsettingHelper = new AppSettingHelper();
            smtpHost = appsettingHelper.GetConfiguration("EmailSetting:SmtpHost");
            port = Int32.Parse(appsettingHelper.GetConfiguration("EmailSetting:Port"));
            userName = appsettingHelper.GetConfiguration("EmailSetting:UserName");
            password = appsettingHelper.GetConfiguration("EmailSetting:Password");
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            LoginResponse response = new LoginResponse();

            try
            {
                var encryptPassword = Encrypt(request.password);

                var userData = repository.user.FindUserBy(
                    c => c.Email.ToLower() == request.userName.ToLower());

                if (userData != null)
                {
                    if (!userData.ActiveFlag)
                    {
                        response.userLocked = true;
                    }
                    else if (!userData.ValidateEmailFlag)
                    {
                        response.userNotValidate = true;
                    }
                    else if (userData.OnlineFlag)
                    {
                        response.userOnline = true;
                    }
                    else
                    {
                        if (userData.Password == encryptPassword)
                        {
                            var userToken = await repository.user.CreateUserToken(userData.ID);

                            if (userToken != null)
                            {
                                userData.OnlineFlag = true;
                                userData.Last_Login = Utility.GetDateNowThai();
                                await repository.user.UpdateUser(userData);

                                response.userDetail = GetUserDetailData(userData.ID);
                                response.tokenID = userToken.Token_ID;
                                response.isSuccess = true;
                            }
                        }
                        else
                        {
                            if (userData.WrongPasswordCount == 3)
                            {
                                // ผิดเกิน 3 ครั้ง
                                // Lock User
                                userData.WrongPasswordCount = 0;
                                userData.ActiveFlag = false;
                            }
                            else
                            {
                                userData.WrongPasswordCount = userData.WrongPasswordCount + 1;
                            }

                            await repository.user.UpdateUser(userData);
                            response.wrongPassword = true;
                        }
                    }
                }
                else
                {
                    response.emailNotExist = true;
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

                var getDepartmentStoreResponse = repository.masterData.GetDepartmentStoreListBy(c => c.Active_Flag);
                var getBrandResponse = repository.masterData.GetBrandListBy(c => c.Active_Flag);
                var channelResponse = repository.masterData.GetDistributionChannelListBy(c => c.Active_Flag && c.Delete_Flag != true);

                if (userData != null)
                {
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

                    var userCounterData = repository.user.GetUserCounterBy(e => e.User_ID == userID);

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


            }
            catch (Exception ex)
            {
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public GetUserDetailResponse GetUserDetailData(Guid userID)
        {
            GetUserDetailResponse response = new GetUserDetailResponse();

            try
            {
                var userData = repository.user.FindUserBy(c => c.ID == userID);

                if (userData != null)
                {
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
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> SaveUserData(SaveUserDataRequest request, string hostUrl)
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

                if ((userByEmail == null || (userByEmail != null && userByEmail.ID == request.userID))
                   && (userByDisplatName == null || (userByDisplatName != null && userByDisplatName.ID == request.userID)))
                {
                    if (request.userID == null || request.userID == Guid.Empty)
                    {
                        var generatePassword = RandomPassword();
                        var passwordEncrypt = Utility.Encrypt(generatePassword);

                        var createUserResponse = await repository.user.CreateNewUser(request, passwordEncrypt);

                        if (createUserResponse != null)
                        {
                            if (request.userCounter != null && request.userCounter.Any())
                            {
                                await repository.user.RemoveAllUserCounterByID(createUserResponse.ID);
                                await CreateUserCounterData(createUserResponse.ID, request.userCounter);
                            }

                            var dateExpireLink = Utility.GetDateNowThai().AddDays(30);

                            var urlActivateUserData = await repository.url.CreateUrl(createUserResponse.ID.ToString(), dateExpireLink, TypeUrl.ActivateUser.ToString());

                            if (urlActivateUserData != null)
                            {
                                await SendEmailActivateUser(request.email, hostUrl, urlActivateUserData.ID.ToString(), generatePassword);
                            }

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

        public async Task<ActivateUserResponse> ActivateUser(Guid urlID)
        {
            ActivateUserResponse response = new ActivateUserResponse();

            try
            {
                var urlData = repository.url.GetUrlDataBy(c => c.ID == urlID);
                if (urlData != null)
                {
                    if (urlData.Flag_Active && urlData.Expire_Date > Utility.GetDateNowThai())
                    {
                        var ref1 = urlData.Ref1;
                        var activateUserResponse = await repository.user.ActivateUser(new Guid(ref1));
                        if (activateUserResponse)
                        {
                            urlData.Flag_Active = false;
                            await repository.url.UpdateUrlData(urlData);

                            response.isSuccess = true;
                        }
                        else
                        {
                            response.isSuccess = false;
                        }
                    }
                    else
                    {
                        if (urlData.Expire_Date < Utility.GetDateNowThai())
                        {
                            response.urlExpire = true;
                        }
                        else if (!urlData.Flag_Active)
                        {
                            response.unActive = true;
                        }
                    }
                }
                else
                {
                    response.urlNotFound = true;
                    response.isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> ResendWelcomeEmail(Guid userID, string hostUrl)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var userData = repository.user.FindUserBy(c => c.ID == userID);
                var dateExpireLink = Utility.GetDateNowThai().AddDays(30);

                await repository.url.UnActiveOldUrl(userID.ToString(), TypeUrl.ActivateUser.ToString());

                var urlActivateUserData = await repository.url.CreateUrl(userData.ID.ToString(), dateExpireLink, TypeUrl.ActivateUser.ToString());

                if (urlActivateUserData != null)
                {
                    var passwordDecrypt = Utility.Decrypt(userData.Password);
                    response.isSuccess = await SendEmailActivateUser(userData.Email, hostUrl, urlActivateUserData.ID.ToString(), passwordDecrypt);
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> ResetPassword(string email, string hostUrl)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var userData = repository.user.FindUserBy(c => c.Email.ToLower() == email.ToLower());

                if (userData != null)
                {
                    await repository.url.UnActiveOldUrl(userData.ID.ToString(), TypeUrl.ResetPassword.ToString());

                    var dateExpireLink = GetDateNowThai().AddHours(4);
                    var urlResetPassword = await repository.url.CreateUrl(userData.ID.ToString(), dateExpireLink, TypeUrl.ResetPassword.ToString());

                    if (urlResetPassword != null)
                    {
                        response.isSuccess = await SendEmailResetPassword(userData.Email, hostUrl, urlResetPassword.ID.ToString());
                    }
                }
                else
                {
                    response.notExistEmail = true;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> ChangePasssword(ChangePasswordRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var userData = repository.user.FindUserBy(c => c.ID == request.userID);
                var urlData = repository.url.GetUrlDataBy(c => c.ID == request.urlID);

                string passwordEncrypt = Encrypt(request.password);

                var updateUserPasswordResult = await repository.user.ChangeUserPassword(request.userID, passwordEncrypt);
                if (updateUserPasswordResult)
                {
                    urlData.Flag_Active = false;
                    await repository.url.UpdateUrlData(urlData);

                    response.isSuccess = true;
                }
                else
                {
                    response.isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public VerifyUrlResetPasswordResponse VerifyUrlResetPassword(Guid urlID)
        {
            VerifyUrlResetPasswordResponse response = new VerifyUrlResetPasswordResponse();

            try
            {
                var urlData = repository.url.GetUrlDataBy(c => c.ID == urlID);
                if (urlData != null)
                {
                    if (urlData.Flag_Active && urlData.Expire_Date > Utility.GetDateNowThai())
                    {
                        var ref1 = urlData.Ref1;
                        Guid userID = new Guid(ref1);
                        var userData = repository.user.FindUserBy(c => c.ID == userID);

                        if (userData != null)
                        {
                            response.isSuccess = true;
                            response.urlID = urlID;
                            response.userID = userData.ID;
                        }
                        else
                        {
                            response.isSuccess = false;
                        }
                    }
                    else
                    {
                        if (DateTime.Compare(urlData.Expire_Date, Utility.GetDateNowThai()) < 0)
                        {
                            response.urlExpire = true;
                        }
                        else if (!urlData.Flag_Active)
                        {
                            response.unActive = true;
                        }
                    }
                }
                else
                {
                    response.urlNotFound = true;
                    response.isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public async Task<ImportDataResponse> ImportUserData(ImportDataRequest request, string hostUrl)
        {
            ImportDataResponse response = new ImportDataResponse();

            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                List<SaveUserDataRequest> saveUserList = new List<SaveUserDataRequest>();
                List<ImportResultByRowResponse> importResult = new List<ImportResultByRowResponse>();

                var getDepartmentStoreResponse = repository.masterData.GetDepartmentStoreListBy(c => c.Active_Flag);
                var getBrandResponse = repository.masterData.GetBrandListBy(c => c.Active_Flag);
                var channelResponse = repository.masterData.GetDistributionChannelListBy(c => c.Active_Flag && c.Delete_Flag != true);

                using (var reader = ExcelReaderFactory.CreateReader(request.fileStream))
                {
                    while (reader.Read()) //Each row of the file
                    {
                        // Validate Column File
                        if (reader.Depth == 0)
                        {
                            string column1 = reader.GetValue(0)?.ToString();
                            string column2 = reader.GetValue(1)?.ToString();
                            string column3 = reader.GetValue(2)?.ToString();
                            string column4 = reader.GetValue(3)?.ToString();
                            string column5 = reader.GetValue(4)?.ToString();
                            string column6 = reader.GetValue(5)?.ToString();
                            string column7 = reader.GetValue(6)?.ToString();
                            string column8 = reader.GetValue(7)?.ToString();
                            string column9 = reader.GetValue(8)?.ToString();
                            string column10 = reader.GetValue(9)?.ToString();
                            string column11 = reader.GetValue(10)?.ToString();
                            string column12 = reader.GetValue(11)?.ToString();
                            string column13 = reader.GetValue(12)?.ToString();
                            string column14 = reader.GetValue(13)?.ToString();

                            if ((column1 != null && column1.ToLower() != "first name") ||
                                (column2 != null && column2.ToLower() != "last name") ||
                                (column3 != null && column3.ToLower() != "email") ||
                                (column4 != null && column4.ToLower() != "display name") ||
                                (column5 != null && column5.ToLower() != "department store") ||
                                (column6 != null && column6.ToLower() != "brand") ||
                                (column7 != null && column7.ToLower() != "distribution channel") ||
                                (column8 != null && column8.ToLower() != "view master permission") ||
                                (column9 != null && column9.ToLower() != "edit master permission") ||
                                (column10 != null && column10.ToLower() != "edit users permission") ||
                                (column11 != null && column11.ToLower() != "view data permission") ||
                                (column12 != null && column12.ToLower() != "key-in data permission") ||
                                (column13 != null && column13.ToLower() != "approve data permission") ||
                                (column14 != null && column14.ToLower() != "view report permission"))
                            {
                                response.isSuccess = false;
                                response.wrongFormatFile = true;
                                return response;
                            }
                        }

                        if (reader.Depth != 0)
                        {
                            string firstName = reader.GetValue(0)?.ToString();
                            string lastName = reader.GetValue(1)?.ToString();
                            string email = reader.GetValue(2)?.ToString();
                            string displayName = reader.GetValue(3)?.ToString();
                            string departmentStore = reader.GetValue(4)?.ToString();
                            string brand = reader.GetValue(5)?.ToString();
                            string channel = reader.GetValue(6)?.ToString();
                            string viewMaster = reader.GetValue(7)?.ToString();
                            string editMaster = reader.GetValue(8)?.ToString();
                            string editUser = reader.GetValue(9)?.ToString();
                            string viewData = reader.GetValue(10)?.ToString();
                            string keyIn = reader.GetValue(11)?.ToString();
                            string approve = reader.GetValue(12)?.ToString();
                            string viewReport = reader.GetValue(13)?.ToString();

                            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(displayName)
                                    && !string.IsNullOrWhiteSpace(viewMaster) && !string.IsNullOrWhiteSpace(editMaster)
                                    && !string.IsNullOrWhiteSpace(editUser) && !string.IsNullOrWhiteSpace(viewData)
                                    && !string.IsNullOrWhiteSpace(keyIn) && !string.IsNullOrWhiteSpace(approve)
                                    && !string.IsNullOrWhiteSpace(viewReport))
                            {
                                SaveUserDataRequest saveUserRequest = new SaveUserDataRequest
                                {
                                    firstName = firstName,
                                    lastName = lastName,
                                    actionBy = new Guid(request.userID),
                                    displayName = displayName,
                                    email = email,
                                    active = false,
                                    viewMaster = viewMaster == "1",
                                    viewData = viewData == "1",
                                    editUser = editUser == "1",
                                    keyInData = keyIn == "1",
                                    editMaster = editMaster == "1",
                                    approveData = approve == "1",
                                    viewReport = viewReport == "1",
                                    validateEmail = false,
                                    row = reader.Depth + 1
                                };

                                if (!string.IsNullOrWhiteSpace(brand) && !string.IsNullOrWhiteSpace(departmentStore)
                                    && !string.IsNullOrWhiteSpace(channel))
                                {
                                    var departMentStoreData = getDepartmentStoreResponse.FirstOrDefault(d => d.Department_Store_Name.ToLower() == departmentStore.ToLower());
                                    var brandData = getBrandResponse.FirstOrDefault(d => d.Brand_Name.ToLower() == brand.ToLower());
                                    var channelData = channelResponse.FirstOrDefault(d => d.Distribution_Channel_Name.ToLower() == channel.ToLower());

                                    if (departMentStoreData != null && brandData != null && channelData != null)
                                    {
                                        saveUserRequest.userCounter = new List<UserCounterData>()
                                        {
                                            new UserCounterData
                                            {
                                                brandID = brandData.Brand_ID,
                                                channelID = channelData.Distribution_Channel_ID,
                                                departmentStoreID = departMentStoreData.Department_Store_ID
                                            }
                                        };
                                    }
                                }

                                saveUserList.Add(saveUserRequest);
                            }
                            else
                            {
                                int row = reader.Depth + 1;
                                ImportResultByRowResponse resultImport = new ImportResultByRowResponse
                                {
                                    row = row,
                                    result = $"Row {row} Required value in column "
                                };

                                if (string.IsNullOrWhiteSpace(email))
                                {
                                    resultImport.result += "Email, ";
                                }

                                if (string.IsNullOrWhiteSpace(displayName))
                                {
                                    resultImport.result += "Display name, ";
                                }

                                if (string.IsNullOrWhiteSpace(viewMaster))
                                {
                                    resultImport.result += "View Master permission, ";
                                }

                                if (string.IsNullOrWhiteSpace(editMaster))
                                {
                                    resultImport.result += "Edit Master permission, ";
                                }

                                if (string.IsNullOrWhiteSpace(editUser))
                                {
                                    resultImport.result += "Edit users permission, ";
                                }

                                if (string.IsNullOrWhiteSpace(viewData))
                                {
                                    resultImport.result += "View data permission, ";
                                }

                                if (string.IsNullOrWhiteSpace(keyIn))
                                {
                                    resultImport.result += "Key-in data permission, ";
                                }

                                if (string.IsNullOrWhiteSpace(approve))
                                {
                                    resultImport.result += "Approve data permission, ";
                                }

                                if (string.IsNullOrWhiteSpace(viewReport))
                                {
                                    resultImport.result += "View report permission, ";
                                }

                                importResult.Add(resultImport);
                                response.countImportFailed = response.countImportFailed + 1;
                            }
                        }
                    }
                }

                if (saveUserList.Any())
                {
                    foreach (var saveUserRequest in saveUserList)
                    {
                        var result = await SaveUserData(saveUserRequest, hostUrl);

                        if (result.isSuccess)
                        {
                            response.countImportSuccess = response.countImportSuccess + 1;
                        }
                        else
                        {
                            ImportResultByRowResponse resultImport = new ImportResultByRowResponse
                            {
                                row = saveUserRequest.row
                            };

                            if (result.isDuplicated == true)
                            {
                                resultImport.result = $"Row {saveUserRequest.row} Data is duplicate.";
                            }
                            else
                            {
                                resultImport.result = $"Row {saveUserRequest.row} Save data failed.";
                            }

                            importResult.Add(resultImport);
                            response.countImportFailed = response.countImportFailed + 1;
                        }

                    }

                    response.importResult = importResult.OrderBy(c => c.row).ToList();

                    byte[] bytes = null;
                    using (var ms = new MemoryStream())
                    {
                        TextWriter tw = new StreamWriter(ms);

                        foreach (var itemResult in response.importResult)
                        {
                            tw.WriteLine(itemResult.result);
                        }

                        tw.Flush();
                        ms.Position = 0;
                        bytes = ms.ToArray();
                        tw.Close();
                    }

                    response.fileName = "ImportUserDataResult_" + Utility.GetDateNowThai().ToString("dd-MM-yyyy-HH-mm");
                    response.fileResult = Convert.ToBase64String(bytes);
                    response.isSuccess = true;
                }
                else
                {
                    response.isSuccess = false;
                    response.wrongFormatFile = true;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
            }

            return response;
        }

        public bool ValidateToken(string tokenID)
        {
            try
            {
                var userTokenData = repository.user.GetUserTokenBy(c => c.Token_ID == tokenID);
                if (userTokenData != null)
                {
                    if (!userTokenData.FlagActive
                        || DateTime.Compare(userTokenData.Token_ExpireTime, Utility.GetDateNowThai()) < 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> RefreshToken(Guid userID)
        {
            try
            {
                var userTokenData = await repository.user.CreateUserToken(userID);
                if (userTokenData != null)
                {
                    return userTokenData.Token_ID;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
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

        private async Task<bool> SendEmailActivateUser(string emailTo, string hostUrl, string urlID, string passwordUser)
        {
            try
            {
                // Send Email
                string url = $"{hostUrl}/Users/ActivateUser?refID={urlID}";
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
                m.Body = $"{url} {passwordUser}";
                //m.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    smtp.Host = smtpHost;
                    smtp.Port = port;
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(m);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> SendEmailResetPassword(string emailTo, string hostUrl, string urlID)
        {
            try
            {
                // Send Email
                string url = $"{hostUrl}/Users/VerifyUrlResetPassword?refID={urlID}";
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
                m.Subject = "Reset Password​";
                m.Body = $"{url}";
                //m.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    smtp.Host = smtpHost;
                    smtp.Port = port;
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(m);
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
