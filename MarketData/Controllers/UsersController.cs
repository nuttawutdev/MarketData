using MarketData.Middleware;
using MarketData.Model.Request.MasterData;
using MarketData.Model.Request.User;
using MarketData.Model.Response;
using MarketData.Model.Response.MasterData;
using MarketData.Models;
using MarketData.Processes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MarketData.Controllers
{
    public class UsersController : Controller
    {
        private readonly Process process;

        public UsersController(Process process)
        {
            this.process = process;
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult Index()
        {
            UserOptionViewModel viewModel = new UserOptionViewModel();

            try
            {
                var userOption = process.user.GetUserOption();

                if (userOption.departmentStore != null && userOption.departmentStore.Any())
                {
                    viewModel.departmentStoreList = userOption.departmentStore.Select(c => new DepartmentStoreKeyInViewModel
                    {
                        departmentStoreID = c.departmentStoreID,
                        departmentStoreName = c.departmentStoreName,
                        distributionChannelID = c.distributionChannelID,
                        retailerGroupID = c.retailerGroupID
                    }).ToList();
                }

                if (userOption.brand != null && userOption.brand.Any())
                {
                    viewModel.brandList = userOption.brand.Select(c => new BrandKeyInViewModel
                    {
                        brandID = c.brandID,
                        brandName = c.brandName,
                    }).ToList();
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("KeyIn", "Home");
            }
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult Users_Edit(Guid userID)
        {
            var viewModel = GetUserDetail(userID);

            if (viewModel != null)
            {
                return View(viewModel);
            }
            else
            {
                return View("Index");
            }
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult Users_Edit_View(Guid userID)
        {
            var viewModel = GetUserDetail(userID);

            if (viewModel != null)
            {
                return View(viewModel);
            }
            else
            {
                return View("Index");
            }

        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult ManualChangePassword()
        {
            try
            {
                var userDetailSession = HttpContext.Session.GetString("userDetail");
                var userDetail = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

                ChangePasswordViewModel viewModel = new ChangePasswordViewModel
                {
                    userID = userDetail.userID,
                    urlID = Guid.Empty
                };

                return View("../Home/ResetPassword", viewModel);
            }
            catch(Exception ex)
            {
                return View("../Home/Index");
            }          
        }

        [HttpPost]
        public IActionResult GetUserList()
        {
            UserListViewModel viewModel = new UserListViewModel();

            try
            {
                var userData = process.user.GetUserList();
                viewModel.data = userData.data.Select(c => new UserListData
                {
                    email = c.email,
                    userID = c.userID,
                    departmentStoreID = c.departmentStoreID != null ? string.Join(",", c.departmentStoreID) : null,
                    firstName = c.firstName,
                    active = c.active,
                    brandID = c.brandID != null ? string.Join(",", c.brandID) : null,
                    displayName = c.displayName,
                    lastLogin = c.lastLogin,
                    lastName = c.lastName,
                    validateEmail = c.validateEmail
                }).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                return Json(viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserData([FromBody] SaveUserDataRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();
            if (ModelState.IsValid)
            {
                try
                {
                    var userDetailSession = HttpContext.Session.GetString("userDetail");
                    var userDetail = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

                    request.actionBy = userDetail.userID;
                    response = await process.user.SaveUserData(request, $"{Request.Scheme}://{Request.Host.Value}");
                    return Json(response);
                }
                catch (Exception ex)
                {
                    return Json(response);
                }
            }
            else
            {
                response = new SaveDataResponse
                {
                    isSuccess = false,
                    responseError = "Please input required field."
                };
                return Json(response);
            }
        }

        public UserDetailViewModel GetUserDetail(Guid userID)
        {
            UserDetailViewModel data = new UserDetailViewModel();
            var response = process.user.GetUserDetail(userID);

            try
            {
                if (response.email != null)
                {
                    data.userID = response.userID;
                    data.email = response.email;
                    data.active = response.active;
                    data.displayName = response.displayName;
                    data.firstName = response.firstName;
                    data.lastName = response.lastName;
                    data.validateEmail = response.validateEmail;
                    data.viewMaster = response.viewMaster;
                    data.editMaster = response.editMaster;
                    data.editUser = response.editUser;
                    data.viewData = response.viewData;
                    data.keyInData = response.keyInData;
                    data.approveData = response.approveData;
                    data.viewReport = response.viewReport;
                    data.officeUser = response.officeUser;
                    data.brandOfficeID = response.brandOfficeID;
                }

                if (response.departmentStore != null && response.departmentStore.Any())
                {
                    data.departmentStoreList = response.departmentStore.Select(c => new DepartmentStoreKeyInViewModel
                    {
                        departmentStoreID = c.departmentStoreID,
                        departmentStoreName = c.departmentStoreName,
                        distributionChannelID = c.distributionChannelID,
                        retailerGroupID = c.retailerGroupID
                    }).ToList();
                }

                if (response.channel != null && response.channel.Any())
                {
                    data.channelList = response.channel.Select(c => new ChannelKeyInViewModel
                    {
                        distributionChannelID = c.distributionChannelID,
                        distributionChannelName = c.distributionChannelName
                    }).ToList();
                }

                if (response.brand != null && response.brand.Any())
                {
                    data.brandList = response.brand.Select(c => new BrandKeyInViewModel
                    {
                        brandID = c.brandID,
                        brandName = c.brandName,
                    }).ToList();
                }

                if (response.userCounter != null && response.userCounter.Any())
                {
                    data.userCounter = response.userCounter.Select(c => new UserCounterViewModel
                    {
                        userCounterID = c.userCounterID.GetValueOrDefault(),
                        departmentStoreID = c.departmentStoreID,
                        departmentStoreName = c.departmentStoreName,
                        channelID = c.channelID,
                        channelName = c.channelName,
                        brandID = c.brandID,
                        brandName = c.brandName
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return data;
        }

        [HttpGet]
        public async Task<IActionResult> ActivateUser(string refID)
        {
            var activateUserResponse = await process.user.ActivateUser(new Guid(refID));

            if (activateUserResponse.isSuccess)
            {
                return View("ActivateSuccess");
            }
            else
            {
                return RedirectToAction("UrlUnvaliable");
            }
        }

        public IActionResult UrlUnvaliable(string refID)
        {
            return View();
        }

        [HttpGet]
        public IActionResult VerifyUrlResetPassword(string refID)
        {
            var verifyUrlResponse = process.user.VerifyUrlResetPassword(new Guid(refID));

            if (verifyUrlResponse.isSuccess)
            {
                ChangePasswordViewModel viewModel = new ChangePasswordViewModel
                {
                    userID = verifyUrlResponse.userID,
                    urlID = verifyUrlResponse.urlID
                };

                return View("../Home/ResetPassword", viewModel);
            }
            else
            {
                return View("UrlUnvaliable");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel request)
        {
            SaveDataResponse response = new SaveDataResponse();

            if (ModelState.IsValid)
            {
                ChangePasswordRequest internalRequest = new ChangePasswordRequest
                {
                    urlID = request.urlID,
                    userID = request.userID,
                    password = request.password
                };

                response = await process.user.ChangePasssword(internalRequest);
                return Json(response);
            }
            else
            {
                var modelErrors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        modelErrors.Add(modelError.ErrorMessage);
                    }
                }

                response.responseError = modelErrors.FirstOrDefault();
                return Json(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportUser(IFormFile excelFile)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            ImportDataResponse response = new ImportDataResponse();

            var userDetailSession = HttpContext.Session.GetString("userDetail");
            var userDetail = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

            using (var stream = new MemoryStream())
            {
                excelFile.CopyTo(stream);
                stream.Position = 0;

                ImportDataRequest request = new ImportDataRequest
                {
                    fileStream = stream,
                    filePath = excelFile.FileName,
                    userID = userDetail.userID.ToString(), // Get User ID from session
                };

                response = await process.user.ImportUserData(request, $"{Request.Scheme}://{Request.Host.Value}");
            }

            return Json(response);
        }


        [HttpGet]
        public async Task<IActionResult> ResendWelcomeEmail(Guid userID)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                response = await process.user.ResendWelcomeEmail(userID, $"{Request.Scheme}://{Request.Host.Value}");
                return Json(response);
            }
            catch (Exception ex)
            {
                response.responseError = ex.InnerException?.Message ?? ex.Message;
                return Json(response);
            }
        }


        [HttpGet]
        public async Task<IActionResult> ResetPassword(string email)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                response = await process.user.ResetPassword(email, $"{Request.Scheme}://{Request.Host.Value}");
                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(response);
            }
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody]DeleteUserRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var userDetailSession = HttpContext.Session.GetString("userDetail");
                var userDetail = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

                request.actionBy = userDetail.userID;
                response = await process.user.DeleteUser(request);
                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(response);
            }
        }

        public async Task<IActionResult> Test()
        {
            //ChangePasswordRequest internalRequest = new ChangePasswordRequest
            //{
            //    urlID = new Guid("c3f34114-3777-442d-9f0e-4fc4194074c2"),
            //    userID = new Guid("3B1E4B7D-F75D-4A27-A707-8676F112814D"),
            //    password = "1234"
            //};

            //var response = await process.user.ChangePasssword(internalRequest);

            //var reset = process.user.ResetPassword("nuttawut.ppb@gmail.com", $"{Request.Scheme}://{Request.Host.Value}");
            //SaveUserDataRequest request = new SaveUserDataRequest
            //{
            //    email = "nuttawut.ppb@gmail.com",
            //    firstName = "Nutt",
            //    lastName = "Pool",
            //    displayName = "PPB",
            //    actionBy = Guid.NewGuid(),
            //    active = false,
            //    approveData = true
            //};

            //var saceUser = process.user.SaveUserData(request, $"{Request.Scheme}://{Request.Host.Value}");

            //var resendWelcome = process.user.ResendWelcomeEmail(new Guid("3B1E4B7D-F75D-4A27-A707-8676F112814D"), $"{Request.Scheme}://{Request.Host.Value}");

            return View();
        }
    }
}
