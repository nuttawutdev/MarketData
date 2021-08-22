using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketData.Model.Request.Adjust;
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

        public IActionResult Adjust_Edit(Guid adjustDataID)
        {
            var role = HttpContext.Session.GetString("role");
            if (role == "Admin")
            {
                AdjustDetailViewModel dataModel = new AdjustDetailViewModel
                {
                    departmentStore = "The Mall Bangkae",
                    channel = "Counter",
                    year = "2021",
                    month = "August",
                    week = "2",
                    retailerGroup = "The Mall",
                    universe = "LPD",
                    status = "Pending",
                    brandDataColumn = new List<string>
                    {
                        { "LAN-Amt.Sales"},{"LAN-Rank"}, { "BIO-Amt.Sales"},{"BIO-Rank"}
                    },
                    data = new List<AdjustDetailViewData>()
                    {
                        new AdjustDetailViewData
                        {
                            brandID = new Guid("70e012b1-d217-4192-a088-7f5e7e6636fe"),
                            brandName = "Covermark",
                            amountPreviousYear= 40000,
                            adminAmountSale = 500000,
                            adjustAmountSale = 500000,
                            adjustWholeSale = 30000,
                            rank = 1,
                            sk = 25,
                            mu = 25,
                            fg = 25,
                            ot = 25,
                            remark = "",
                            percentGrowth = 20,
                           brandKeyInAmount = new Dictionary<string, decimal?>()
                           {
                               { "LAN",50000},{ "BIO",40000},
                           },
                           brandKeyInRank = new Dictionary<string, string>()
                           {
                                { "LAN","1"},{ "BIO","1"},
                           }
                        },
                        new AdjustDetailViewData
                        {
                            brandID = new Guid("269e27cb-d386-4b9a-933c-81e77db2a36d"),
                            brandName = "Tell ME",
                            amountPreviousYear= null,
                            adminAmountSale = null,
                            adjustAmountSale = null,
                            adjustWholeSale = null,
                            rank = 3,
                            sk = null,
                            mu = null,
                            fg = null,
                            ot = null,
                            remark = "ไม่มียอดขาย",
                            percentGrowth = null,
                           brandKeyInAmount = new Dictionary<string, decimal?>()
                           {
                               { "LAN",null},{ "BIO",null},
                           },
                           brandKeyInRank = new Dictionary<string, string>()
                           {
                                { "LAN",null},{ "BIO",null},
                           }
                        },
                        new AdjustDetailViewData
                        {
                            brandID = new Guid("36680587-1abe-474c-878b-9ea94e02ba0b"),
                            brandName = "Lancome",
                            amountPreviousYear= null,
                            adminAmountSale = 45000,
                            adjustAmountSale = 45000,
                            adjustWholeSale = 20000,
                            rank = 2,
                            sk = null,
                            mu = null,
                            fg = null,
                            ot = null,
                            remark = "",
                            percentGrowth = null,
                           brandKeyInAmount = new Dictionary<string, decimal?>()
                           {
                               { "LAN",40000},{ "BIO",3000},
                           },
                           brandKeyInRank = new Dictionary<string, string>()
                           {
                                { "LAN","2"},  { "BIO","2"},
                           }
                        },
                    }
                };

                //var adjustDetailData = process.adjust.GetAdjustDataDetail(adjustDataID);

                //AdjustDetailViewModel dataModel = new AdjustDetailViewModel
                //{
                //    data = adjustDetailData.data.Select(c => new AdjustDetailViewData
                //    {
                //        brandID = c.brandID,
                //        adjustAmountSale = c.adjustAmountSale,
                //        adjustWholeSale = c.adjustWholeSale,
                //        adminAmountSale = c.adminAmountSale,
                //        amountPreviousYear = c.amountPreviousYear,
                //        brandKeyInAmount = c.brandKeyInAmount,
                //        brandKeyInRank = c.brandKeyInRank,
                //        fg = c.fg,
                //        mu = c.mu,
                //        ot = c.ot,
                //        sk = c.sk,
                //        brandName = c.brandName,
                //        remark = c.remark,
                //        rank = c.rank,
                //        percentGrowth = c.percentGrowth

                //    }).ToList()
                //};

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

        [HttpPost]
        public async Task<IActionResult> GetAdjustDataDetail([FromBody] GetAdjustDetailRequest request)
        {
            SaveAdjustDetailResponse response = new SaveAdjustDetailResponse();

            try
            {
                response = await process.adjust.CreateAdjustData(request);
                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(response);
            }
        }
    }
}
