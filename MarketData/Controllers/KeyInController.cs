﻿using MarketData.Model.Response.KeyIn;
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
        public async Task<IActionResult> KeyinByStore_Edit([FromBody] CreateBAKeyInRequest request) 
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
