using MarketData.Model.Response.KeyIn;
using MarketData.Processes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketData.Models;
using Microsoft.AspNetCore.Http;
using MarketData.Model.Request.KeyIn;

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
            var userID = HttpContext.Session.GetString("userID");

            SaveBAKeyInDetailRequest request = new SaveBAKeyInDetailRequest
            {
                BAKeyInID = new Guid("E81EA953-B2FD-44D3-900B-9719B487AA70"),
                userID = new Guid(userID),
                BAKeyInDetailList = new List<Model.Data.BAKeyInDetailData>
                {
                    new Model.Data.BAKeyInDetailData
                    {
                        ID = new Guid("B27AF927-6C84-4472-B873-0E677EE20899"),
                        amountSale = 2000,
                        rank = 1,
                        wholeSale = 500,
                        sk = 20,
                        mu = 10,
                        fg = 30,
                        ot = 40,
                    },
                     new Model.Data.BAKeyInDetailData
                    {
                        ID = new Guid("23580C70-7CF4-4890-A1BE-3BC28907625A"),
                        amountSale = 5000,
                        rank = 2,
                        wholeSale = 1200,
                        sk = 20,
                        mu = 10,
                        fg = 30,
                        ot = 40,
                    }
                }
            };

            var result = process.keyIn.SaveBAKeyInDetail(request);

            return View();
        }

        public IActionResult KeyinByStore()
        {
            BAKeyInListViewModel dataModel = new BAKeyInListViewModel();

            try
            {
                var userID = HttpContext.Session.GetString("userID");

                if (userID != null)
                {
                    var baKeyInOption = process.keyIn.GetBAKeyInOption(new Guid(userID));
                    var keyInStatus = process.masterData.GetKeyInStatus();

                    if (baKeyInOption != null && baKeyInOption.channel != null && baKeyInOption.channel.Any())
                    {
                        dataModel.channelList = baKeyInOption.channel.Select(c => new ChannelKeyInViewModel
                        {
                            distributionChannelID = c.distributionChannelID,
                            distributionChannelName = c.distributionChannelName
                        }).ToList();
                    }

                    if (baKeyInOption != null && baKeyInOption.departmentStore != null && baKeyInOption.departmentStore.Any())
                    {
                        dataModel.departmentStoreList = baKeyInOption.departmentStore.Select(c => new DepartmentStoreKeyInViewModel
                        {
                            departmentStoreID = c.departmentStoreID,
                            departmentStoreName = c.departmentStoreName
                        }).ToList();
                    }

                    if (baKeyInOption != null && baKeyInOption.brand != null && baKeyInOption.brand.Any())
                    {
                        dataModel.brandList = baKeyInOption.brand.Select(c => new BrandKeyInViewModel
                        {
                            brandID = c.brandID,
                            brandName = c.brandName
                        }).ToList();
                    }

                    if (baKeyInOption != null && baKeyInOption.brand != null && baKeyInOption.brand.Any())
                    {
                        dataModel.retailerGroupList = baKeyInOption.retailerGroup.Select(c => new RetailerGroupKeyInViewModel
                        {
                            retailerGroupID = c.retailerGroupID,
                            retailerGroupName = c.retailerGroupName
                        }).ToList();
                    }

                    if (keyInStatus != null && keyInStatus.data != null && keyInStatus.data.Any())
                    {
                        dataModel.statusList = keyInStatus.data.Select(c => new StatusKeyInViewModel
                        {
                            statusID = c.statusID,
                            statusName = c.statusName
                        }).ToList();
                    }

                    dataModel.yearList = baKeyInOption.year;

                    dataModel.userID = new Guid(userID);
                    return View(dataModel);
                }
                else
                {
                    return RedirectToAction("KeyIn", "Home");
                }
            }
            catch (Exception ex)
            {
                return View(dataModel);
            }
        }

        [HttpPost]
        public IActionResult KeyinByStore_Edit([FromBody] CreateBAKeyInRequest request) 
        { 
            return View();
        }

        #region BA KeyIn Function

        [HttpPost]
        public IActionResult GetBAKeyInList()
        {
            BAKeyInListViewModel dataModel = new BAKeyInListViewModel();

            try
            {
                var userID = HttpContext.Session.GetString("userID");

                if (userID != null)
                {
                    var baKeyInData = process.keyIn.GetBAKeyInList(new Guid(userID));

                    if (baKeyInData != null && baKeyInData.data != null && baKeyInData.data.Any())
                    {
                        dataModel.data = baKeyInData.data.Select(c => new BAKeyInDataViewModel
                        {
                            approveDate = c.approveDate,
                            approver = c.approver,
                            brandID = c.brandID,
                            counter = c.counter,
                            distributionChannelID = c.distributionChannelID,
                            distributionChannelName = c.distributionChannelName,
                            departmentStoreID = c.departmentStoreID,
                            keyInID = c.keyInID,
                            retailerGroupID = c.retailerGroupID,
                            lastEdit = c.lastEdit,
                            month = c.month,
                            remark = c.remark,
                            statusID = c.statusID,
                            statusName = c.statusName,
                            submitDate = c.submitDate,
                            week = c.week,
                            year = c.year
                        }).ToList();
                    }

                    dataModel.userID = new Guid(userID);
                }
            }
            catch (Exception ex)
            {
                return Json(dataModel);
            }

            return Json(dataModel);
        }

        #endregion

    }
}
