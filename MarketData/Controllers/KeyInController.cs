using MarketData.Model.Response.KeyIn;
using MarketData.Processes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketData.Models;
using Microsoft.AspNetCore.Http;

namespace MarketData.Controllers
{
    public class KeyInController : Controller
    {
        private readonly Process process;

        public KeyInController(Process process)
        {
            this.process = process;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult KeyinByBrand()
        {
            return View();
        }
        public IActionResult KeyinByStore()
        {
            var userID = HttpContext.Session.GetString("userID");

            if (userID != null)
            {
                BAKeyInListViewModel data = new BAKeyInListViewModel();

                var baKeyInData = process.keyIn.GetBAKeyInList(new Guid(userID));
                var baKeyInOption = process.keyIn.GetBAKeyInOption(new Guid(userID));
                var keyInStatus = process.masterData.GetKeyInStatus();

                if (baKeyInOption.channel != null && baKeyInOption.channel.Any())
                {
                    data.channelList = baKeyInOption.channel.Select(c => new ChannelKeyInViewModel
                    {
                        distributionChannelID = c.distributionChannelID,
                        distributionChannelName = c.distributionChannelName
                    }).ToList();
                }

                if (baKeyInOption.departmentStore != null && baKeyInOption.departmentStore.Any())
                {
                    data.departmentStoreList = baKeyInOption.departmentStore.Select(c => new DepartmentStoreKeyInViewModel
                    {
                        departmentStoreID = c.departmentStoreID,
                        departmentStoreName = c.departmentStoreName
                    }).ToList();
                }

                if (baKeyInOption.brand != null && baKeyInOption.brand.Any())
                {
                    data.brandList = baKeyInOption.brand.Select(c => new BrandKeyInViewModel
                    {
                        brandID = c.brandID,
                        brandName = c.brandName
                    }).ToList();
                }

                if (baKeyInOption.brand != null && baKeyInOption.brand.Any())
                {
                    data.retailerGroupList = baKeyInOption.retailerGroup.Select(c => new RetailerGroupKeyInViewModel
                    {
                        retailerGroupID = c.retailerGroupID,
                        retailerGroupName = c.retailerGroupName
                    }).ToList();
                }

                if (keyInStatus.data != null && keyInStatus.data.Any())
                {
                    data.statusList = keyInStatus.data.Select(c => new StatusKeyInViewModel
                    {
                        statusID = c.statusID,
                        statusName = c.statusName
                    }).ToList();
                }

                data.yearList = baKeyInData.year;

                return View(data);
            }
            else
            {
                return RedirectToAction("KeyIn", "Home");
            }

        }

        public IActionResult KeyinByStore_Edit()
        {
            return View();
        }

        #region BA KeyIn Function
        //[HttpPost]
        //public IActionResult GetBAKeyInList(Guid userID)
        //{
        //BAKeyInListViewModel BAKeyInListView = new BAKeyInListViewModel();

        //if (ModelState.IsValid)
        //{
        //    var response = process.masterData.GetBrandList();

        //    if (response != null && response.data != null && response.data.Any())
        //    {

        //    }
        //    else
        //    {
        //        BAKeyInListView.brandList = new List<BAKeyInDataViewModel>();
        //    }

        //    return Json(BAKeyInListView);
        //}
        //else
        //{
        //    return Json(BAKeyInListView);
        //}



        //}

        #endregion

    }
}
