using MarketData.Models;
using MarketData.Processes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IActionResult Users_Edit(Guid userID)
        {
            var viewModel = GetUserDetail(userID);

            if(viewModel != null)
            {
                return View(viewModel);
            }
            else
            {
                return View("Index");
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
                    brandID = c.brandID != null  ? string.Join(",", c.brandID) : null,
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

        public UserDetailViewModel GetUserDetail(Guid userID)
        {
            UserDetailViewModel data = new UserDetailViewModel();
            var response = process.user.GetUserDetail(userID);

            try
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
            catch(Exception ex)
            {
                return null;
            }

            return data;
        }
    }
}
