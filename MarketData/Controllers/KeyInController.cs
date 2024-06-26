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
using MarketData.Model.Response;
using System.Text.Json;
using MarketData.Middleware;

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

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult KeyinByBrand()
        {
            var userDetailSession = HttpContext.Session.GetString("userDetail");
            var userPermission = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

            if (userPermission.approveData)
            {
                AdminKeyInViewModel dataModel = new AdminKeyInViewModel();

                try
                {
                    var adminOption = process.keyIn.GetAdminKeyInOption();
                    var keyInRemark = process.masterData.GetKeyInRemark();

                    if (adminOption != null)
                    {
                        if (adminOption.departmentStore != null && adminOption.departmentStore.Any())
                        {
                            dataModel.departmentStoreList = adminOption.departmentStore.Select(c => new DepartmentStoreKeyInViewModel
                            {
                                departmentStoreID = c.departmentStoreID,
                                departmentStoreName = c.departmentStoreName,
                                distributionChannelID = c.distributionChannelID,
                                retailerGroupID = c.retailerGroupID
                            }).ToList();
                        }

                        if (adminOption.retailerGroup != null && adminOption.retailerGroup.Any())
                        {
                            dataModel.retailerGroupList = adminOption.retailerGroup.Select(c => new RetailerGroupKeyInViewModel
                            {
                                retailerGroupName = c.retailerGroupName,
                                retailerGroupID = c.retailerGroupID
                            }).ToList();
                        }

                        if (adminOption.channel != null && adminOption.channel.Any())
                        {
                            dataModel.channelList = adminOption.channel.Select(c => new ChannelKeyInViewModel
                            {
                                distributionChannelID = c.distributionChannelID,
                                distributionChannelName = c.distributionChannelName
                            }).ToList();
                        }

                        if (adminOption.brand != null && adminOption.brand.Any())
                        {
                            dataModel.brandList = adminOption.brand.Select(c => new BrandKeyInViewModel
                            {
                                brandID = c.brandID,
                                brandName = c.brandName,
                            }).ToList();
                        }

                        dataModel.remarkList = keyInRemark.Select(c => new AdminKeyInRemark
                        {
                            ID = c.ID,
                            remark = c.remark
                        }).ToList();
                        dataModel.yearList = adminOption.year;
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("KeyIn", "Home");
                }

                return View(dataModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult KeyinByStore()
        {
            BAKeyInListViewModel dataModel = new BAKeyInListViewModel();

            var userDetailSession = HttpContext.Session.GetString("userDetail");
            var userPermission = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

            if (userPermission != null && userPermission.keyInData)
            {
                try
                {
                    var baKeyInOption = process.keyIn.GetBAKeyInOption(userPermission.userID);
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

                    dataModel.userID = userPermission.userID;
                    return View(dataModel);

                }
                catch (Exception ex)
                {
                    return View(dataModel);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public async Task<IActionResult> KeyinByStore_Edit(Guid baKeyInID)
        {
            var viewData = await GetBAKeyInDetail(baKeyInID);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public async Task<IActionResult> KeyinByStore_Edit_View(Guid baKeyInID)
        {
            var viewData = await GetBAKeyInDetail(baKeyInID, true);
            return View(viewData);
        }

        #region BA KeyIn Function
        [HttpPost]
        public async Task<IActionResult> CreateBAKeyInDetail([FromBody] CreateBAKeyInRequest request)
        {
            var userDetailSession = HttpContext.Session.GetString("userDetail");
            var userDetail = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

            request.userID = userDetail.userID;

            var viewData = await process.keyIn.CreateBAKeyInDetail(request);
            return Json(viewData);
        }

        public async Task<BAKeyInDetailViewModel> GetBAKeyInDetail(Guid baKeyInID, bool viewOnly = false)
        {
            try
            {
                var response = await process.keyIn.GetBAKeyInDetail(baKeyInID);

                var keyInRemark = process.masterData.GetKeyInRemark();

                BAKeyInDetailViewModel data = new BAKeyInDetailViewModel
                {
                    BAKeyInID = baKeyInID,
                    brand = response.brand,
                    channel = response.channel,
                    month = response.month,
                    year = response.year,
                    status = response.status,
                    week = response.week,
                    departmentStore = response.departmentStore,
                    retailerGroup = response.retailerGroup,
                    universe = response.universe,
                    remark = response.remark,
                    rejectReason = response.rejectReason,
                    BAKeyInDetailList = response.data.Select(c => new BAKeyInDetailData
                    {
                        ID = c.ID,
                        keyInID = c.keyInID,
                        fg = c.fg.HasValue ? c.fg.Value.ToString("N") : string.Empty,
                        amountSale = c.amountSale.HasValue ? c.amountSale.Value.ToString("N") : string.Empty,
                        amountSalePreviousYear = c.amountSalePreviousYear.HasValue ? c.amountSalePreviousYear.Value.ToString("N") : string.Empty,
                        brandID = c.brandID,
                        brandName = c.brandName,
                        brandColor = c.brandColor,
                        channelID = c.channelID,
                        counterID = c.counterID,
                        departmentStoreID = c.departmentStoreID,
                        month = c.month,
                        mu = c.mu.HasValue ? c.mu.Value.ToString("N") : string.Empty,
                        ot = c.ot.HasValue ? c.ot.Value.ToString("N") : string.Empty,
                        rank = c.rank,
                        remark = c.remark,
                        sk = c.sk.HasValue ? c.sk.Value.ToString("N") : string.Empty,
                        week = c.week,
                        wholeSale = c.wholeSale.HasValue ? c.wholeSale.Value.ToString("N") : string.Empty,
                        year = c.year
                    }).ToList(),
                    remarkList = keyInRemark.Select(c => new KeyInRemark
                    {
                        ID = c.ID,
                        remark = c.remark
                    }).ToList()
                };

                return data;
            }
            catch (Exception ex)
            {
                return new BAKeyInDetailViewModel();
            }

        }

        [HttpPost]
        public IActionResult GetBAKeyInList()
        {
            BAKeyInListViewModel dataModel = new BAKeyInListViewModel();

            try
            {
                var userDetailSession = HttpContext.Session.GetString("userDetail");
                var userDetail = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

                if (userDetail != null)
                {
                    var baKeyInData = process.keyIn.GetBAKeyInList(userDetail.userID);

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
                }
            }
            catch (Exception ex)
            {
                return Json(dataModel);
            }

            return Json(dataModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveBAKeyInDetail([FromBody] SaveBAKeyInDetailRequest request)
        {
            SaveDataResponse response;

            if (ModelState.IsValid)
            {
                var userDetailSession = HttpContext.Session.GetString("userDetail");
                var userDetail = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

                request.userID = userDetail.userID;

                response = await process.keyIn.SaveBAKeyInDetail(request);
                return Json(response);
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

        [HttpPost]
        public async Task<IActionResult> SubmitBAKeyInDetail([FromBody] SaveBAKeyInDetailRequest request)
        {
            SaveDataResponse response;

            if (ModelState.IsValid)
            {
                var userDetailSession = HttpContext.Session.GetString("userDetail");
                var userDetail = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

                request.userID = userDetail.userID;

                response = await process.keyIn.SubmitBAKeyInDetail(request);
                return Json(response);
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

        #endregion


        #region Admin KeyIn Function

        [HttpPost]
        public IActionResult GetAdminKeyInDetail([FromBody] GetAdminKeyInRequest request)
        {
            AdminKeyInDetailViewModel dataModel = new AdminKeyInDetailViewModel();

            try
            {
                var response = process.keyIn.GetAdminKeyInData(request);
                dataModel.year = request.year;
                dataModel.month = request.month;
                dataModel.week = request.week;
                dataModel.totalAmountPreviosYear = response.totalAmountPreviosYear;
                dataModel.data = response.data.Select(c => new AdminKeyInDetailData
                {
                    ID = c.ID,
                    departmentStoreName = c.departmentStoreName,
                    retailerGroupID = c.retailerGroupID,
                    fg = c.fg.HasValue ? c.fg.Value.ToString("N") : string.Empty,
                    amountSale = c.amountSale.HasValue ? c.amountSale.Value.ToString("N") : string.Empty,
                    amountSalePreviousYear = c.amountSalePreviousYear.HasValue ? c.amountSalePreviousYear.Value.ToString("N") : string.Empty,
                    brandID = c.brandID,
                    brandName = c.brandName,
                    brandColor = c.brandColor,
                    channelID = c.distributionChannelID,
                    counterID = c.counterID,
                    departmentStoreID = c.departmentStoreID,
                    month = c.month,
                    mu = c.mu.HasValue ? c.mu.Value.ToString("N") : string.Empty,
                    ot = c.ot.HasValue ? c.ot.Value.ToString("N") : string.Empty,
                    rank = c.rank,
                    remark = c.remark,
                    sk = c.sk.HasValue ? c.sk.Value.ToString("N") : string.Empty,
                    week = c.week,
                    wholeSale = c.wholeSale.HasValue ? c.wholeSale.Value.ToString("N") : string.Empty,
                    year = c.year,
                    universe = c.universe
                }).ToList();

                return Json(dataModel);
            }
            catch (Exception ex)
            {
                return Json(dataModel);
            }
        }
       
        [HttpPost]
        public async Task<IActionResult> SaveAdminKeyIn([FromBody] SaveAdminKeyInDetailRequest request)
        {
            SaveDataResponse response;

            if (ModelState.IsValid)
            {
                var userDetailSession = HttpContext.Session.GetString("userDetail");
                var userDetail = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

                request.userID = userDetail.userID;

                response = await process.keyIn.SaveAdminKeyIn(request);
                return Json(response);
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

        #endregion
    }
}

