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

        public IActionResult Users_Edit()
        {
            return View();
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
    }
}
