using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketData.Model.Request.Adjust;
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

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("role");
            if (role == "Admin")
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
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult Adjust_Edit(string year, string month, string week, Guid departmentStoreID)
        {
            var role = HttpContext.Session.GetString("role");
            if (role == "Admin")
            {
                GetAdjustDetailRequest request = new GetAdjustDetailRequest
                {
                    year = year,
                    month = month,
                    week = week,
                    departmentStoreID = departmentStoreID
                };

                var adjustDetailData = process.adjust.GetAdjustDataDetail(request);

                AdjustDetailViewModel dataModel = new AdjustDetailViewModel
                {
                    data = adjustDetailData.data.Select(c => new AdjustDetailViewData
                    {
                        brandID = c.brandID,
                        adjustAmountSale = c.adjustAmountSale,
                        adjustWholeSale = c.adjustWholeSale,
                        adminAmountSale = c.adminAmountSale,
                        amountPreviousYear = c.amountPreviousYear,
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
            else
            {
                return RedirectToAction("Index", "Home");
            }

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
    }
}
