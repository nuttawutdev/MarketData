using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MarketData.Middleware;
using MarketData.Model.Request.Adjust;
using MarketData.Model.Response;
using MarketData.Model.Response.AdjustData;
using MarketData.Models;
using MarketData.Processes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketData.Controllers
{
    public class AdjustController : Controller
    {
        private readonly Process process;
        public AdjustController(Process process)
        {
            this.process = process;
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult Index()
        {
            AdjustDataViewModel dataModel = new AdjustDataViewModel();

            try
            {
                var adjustOption = process.adjust.GetAdjustOption();
                var adjustStatus = process.masterData.GetAdjustStatus();

                if (adjustOption != null)
                {
                    if (adjustOption.departmentStore != null && adjustOption.departmentStore.Any())
                    {
                        dataModel.departmentStoreList = adjustOption.departmentStore.Select(c => new DepartmentStoreKeyInViewModel
                        {
                            departmentStoreID = c.departmentStoreID,
                            departmentStoreName = c.departmentStoreName,
                            distributionChannelID = c.distributionChannelID,
                            retailerGroupID = c.retailerGroupID
                        }).ToList();
                    }

                    if (adjustOption.retailerGroup != null && adjustOption.retailerGroup.Any())
                    {
                        dataModel.retailerGroupList = adjustOption.retailerGroup.Select(c => new RetailerGroupKeyInViewModel
                        {
                            retailerGroupName = c.retailerGroupName,
                            retailerGroupID = c.retailerGroupID
                        }).ToList();
                    }

                    if (adjustOption.channel != null && adjustOption.channel.Any())
                    {
                        dataModel.channelList = adjustOption.channel.Select(c => new ChannelKeyInViewModel
                        {
                            distributionChannelID = c.distributionChannelID,
                            distributionChannelName = c.distributionChannelName
                        }).ToList();
                    }

                    if (adjustStatus != null && adjustStatus.Any())
                    {
                        dataModel.statusList = adjustStatus.Select(c => new StatusKeyInViewModel
                        {
                            statusID = c.statusID,
                            statusName = c.statusName,
                        }).ToList();
                    }

                    dataModel.yearList = adjustOption.year;
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(dataModel);


        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult Adjust_Edit(Guid adjustDataID)
        {
            var adjustDetailData = process.adjust.GetAdjustDataDetail(adjustDataID);
            AdjustDetailViewModel dataModel = new AdjustDetailViewModel
            {
                adjustDataID = adjustDetailData.adjustDataID,
                brandDataColumn = adjustDetailData.brandDataColumn,
                channel = adjustDetailData.channel,
                departmentStore = adjustDetailData.departmentStore,
                month = adjustDetailData.month,
                status = adjustDetailData.status,
                retailerGroup = adjustDetailData.retailerGroup,
                universe = adjustDetailData.universe,
                week = adjustDetailData.week,
                year = adjustDetailData.year,
                brandTotalAmount = adjustDetailData.brandTotalAmount,
                data = adjustDetailData.data.Select(c => new AdjustDetailViewData
                {
                    brandID = c.brandID,
                    brandColor = c.brandColor,
                    month = c.month,
                    week = c.week,
                    year = c.year,
                    adjustAmountSale = c.adjustAmountSale,
                    adjustWholeSale = c.adjustWholeSale,
                    adminAmountSale = c.adminAmountSale,
                    amountPreviousYear = c.amountPreviousYear,
                    amountPreviousYearWeek = c.amountPreviousYearWeek,
                    brandKeyInAmount = c.brandKeyInAmount,
                    brandKeyInRank = c.brandKeyInRank,
                    fg = c.fg,
                    mu = c.mu,
                    ot = c.ot,
                    sk = c.sk,
                    brandName = c.brandName,
                    remark = c.remark,
                    rank = c.rank,
                    percentGrowth = c.percentGrowth
                }).ToList()
            };

            return View(dataModel);
        }
        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult Adjust_Edit_View(Guid adjustDataID)
        {
            var adjustDetailData = process.adjust.GetAdjustDataDetail(adjustDataID);
            AdjustDetailViewModel dataModel = new AdjustDetailViewModel
            {
                adjustDataID = adjustDetailData.adjustDataID,
                brandDataColumn = adjustDetailData.brandDataColumn,
                channel = adjustDetailData.channel,
                departmentStore = adjustDetailData.departmentStore,
                month = adjustDetailData.month,
                status = adjustDetailData.status,
                retailerGroup = adjustDetailData.retailerGroup,
                universe = adjustDetailData.universe,
                week = adjustDetailData.week,
                year = adjustDetailData.year,
                brandTotalAmount = adjustDetailData.brandTotalAmount,
                data = adjustDetailData.data.Select(c => new AdjustDetailViewData
                {
                    brandID = c.brandID,
                    brandColor = c.brandColor,
                    month = c.month,
                    week = c.week,
                    year = c.year,
                    adjustAmountSale = c.adjustAmountSale,
                    adjustWholeSale = c.adjustWholeSale,
                    adminAmountSale = c.adminAmountSale,
                    amountPreviousYear = c.amountPreviousYear,
                    amountPreviousYearWeek = c.amountPreviousYearWeek,
                    brandKeyInAmount = c.brandKeyInAmount,
                    brandKeyInRank = c.brandKeyInRank,
                    fg = c.fg,
                    mu = c.mu,
                    ot = c.ot,
                    sk = c.sk,
                    brandName = c.brandName,
                    remark = c.remark,
                    rank = c.rank,
                    percentGrowth = c.percentGrowth
                }).ToList()
            };

            return View(dataModel);
        }
        [HttpPost]
        public IActionResult GetAdjustList([FromBody] GetAdjustListRequest request)
        {
            AdjustListViewModel dataModel = new AdjustListViewModel();

            try
            {
                var adjustDataList = process.adjust.GetAdjustDataList(request);

                dataModel.columnList = adjustDataList.columnList;
                dataModel.data = adjustDataList.data.Select(c => new AdjustViewData
                {
                    year = c.year,
                    month = c.month,
                    week = c.week,
                    brandStatus = c.brandStatus,
                    departmentStoreID = c.departmentStoreID,
                    departmentStoreName = c.departmentStoreName,
                    retailerGroupID = c.retailerGroupID,
                    retailerGroupName = c.retailerGroupName,
                    distributionChannelName = c.distributionChannelName,
                    distributionChannelID = c.distributionChannelID,
                    statusID = c.statusID,
                    statusName = c.statusName
                }).ToList();

                return Json(dataModel);
            }
            catch (Exception ex)
            {
                return Json(dataModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAdjustDataDetail([FromBody] GetAdjustDetailRequest request)
        {
            SaveAdjustDetailResponse response = new SaveAdjustDetailResponse();

            try
            {
                var userDetailSession = HttpContext.Session.GetString("userDetail");
                var userDetail = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);
                request.userID = userDetail.userID;

                response = await process.adjust.CreateAdjustData(request);
                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(response);
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public async Task<IActionResult> SaveAdjustDataDetail([FromBody] SaveAdjustDataRequest request)
        {
            var userDetailSession = HttpContext.Session.GetString("userDetail");
            var userDetail = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

            request.userID = userDetail.userID;
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                response = await process.adjust.SaveAdjustDataDetail(request);
                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(response);
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public async Task<IActionResult> SubmitAdjustDataDetail([FromBody] SaveAdjustDataRequest request)
        {
            var userDetailSession = HttpContext.Session.GetString("userDetail");
            var userDetail = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

            request.userID = userDetail.userID;

            SaveDataResponse response = new SaveDataResponse();

            try
            {
                response = await process.adjust.SubmitAdjustDataDetail(request);
                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(response);
            }
        }
    }
}
