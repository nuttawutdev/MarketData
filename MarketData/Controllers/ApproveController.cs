using MarketData.Middleware;
using MarketData.Model.Request.Approve;
using MarketData.Models;
using MarketData.Processes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Controllers
{
    public class ApproveController : Controller
    {
        private readonly Process process;
        public ApproveController(Process process)
        {
            this.process = process;
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult Index()
        {
            ApproveKeyInViewModel dataModel = new ApproveKeyInViewModel();

            try
            {
                var approveOption = process.approve.GetApproveKeyInOption();
                var approveStatus = process.masterData.GetApproveStatus();

                if (approveOption != null)
                {
                    if (approveOption.departmentStore != null && approveOption.departmentStore.Any())
                    {
                        dataModel.departmentStoreList = approveOption.departmentStore.Select(c => new DepartmentStoreKeyInViewModel
                        {
                            departmentStoreID = c.departmentStoreID,
                            departmentStoreName = c.departmentStoreName,
                            distributionChannelID = c.distributionChannelID,
                            retailerGroupID = c.retailerGroupID
                        }).ToList();
                    }

                    if (approveOption.retailerGroup != null && approveOption.retailerGroup.Any())
                    {
                        dataModel.retailerGroupList = approveOption.retailerGroup.Select(c => new RetailerGroupKeyInViewModel
                        {
                            retailerGroupName = c.retailerGroupName,
                            retailerGroupID = c.retailerGroupID
                        }).ToList();
                    }

                    if (approveOption.channel != null && approveOption.channel.Any())
                    {
                        dataModel.channelList = approveOption.channel.Select(c => new ChannelKeyInViewModel
                        {
                            distributionChannelID = c.distributionChannelID,
                            distributionChannelName = c.distributionChannelName
                        }).ToList();
                    }

                    if (approveOption.brand != null && approveOption.brand.Any())
                    {
                        dataModel.brandList = approveOption.brand.Select(c => new BrandKeyInViewModel
                        {
                            brandID = c.brandID,
                            brandName = c.brandName,
                        }).ToList();
                    }

                    if (approveStatus != null && approveStatus.Any())
                    {
                        dataModel.statusList = approveStatus.Select(c => new StatusKeyInViewModel
                        {
                            statusID = c.statusID,
                            statusName = c.statusName
                        }).ToList();
                    }

                    dataModel.yearList = approveOption.year;
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
        public IActionResult Approve_Edit(Guid approveKeyInID)
        {
            var viewData = GetApproveKeyInDetail(approveKeyInID);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult Approve_Edit_View(Guid approveKeyInID)
        {
            var viewData = GetApproveKeyInDetail(approveKeyInID);
            return View(viewData);
        }

        [HttpPost]
        public IActionResult GetApproveKeyInList()
        {
            ApproveKeyInViewModel dataModel = new ApproveKeyInViewModel();

            try
            {
                var approveData = process.approve.GetApproveKeyInList();

                if (approveData != null)
                {
                    dataModel.data = approveData.data.Select(c => new ApproveKeyInDataViewModel
                    {
                        approveDate = c.approveDate,
                        approver = c.approver,
                        brandID = c.brandID,
                        brandName = c.brandName,
                        distributionChannelID = c.distributionChannelID,
                        distributionChannelName = c.distributionChannelName,
                        departmentStoreID = c.departmentStoreID,
                        approveKeyInID = c.approveKeyInID,
                        retailerGroupID = c.retailerGroupID,
                        baKeyInID = c.baKeyInID,
                        month = c.month,
                        departmentStoreName = c.departmentStoreName,
                        statusID = c.statusID,
                        statusName = c.statusName,
                        week = c.week,
                        year = c.year
                    }).ToList();
                }

            }
            catch (Exception ex)
            {
                return Json(dataModel);
            }

            return Json(dataModel);
        }

        public ApproveKeyInDetailViewModel GetApproveKeyInDetail(Guid approveKeyInID)
        {
            try
            {
                var response = process.approve.GetApproveKeyInDetail(approveKeyInID);

                ApproveKeyInDetailViewModel data = new ApproveKeyInDetailViewModel
                {
                    approveKeyInID = approveKeyInID,
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
                    baRemark = response.baRemark,
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
                };

                return data;
            }
            catch (Exception ex)
            {
                return new ApproveKeyInDetailViewModel();
            }

        }

        [HttpPost]
        public async Task<IActionResult> ApproveKeyInData([FromBody] ApproveKeyInDataRequest request)
        {
            var userID = HttpContext.Session.GetString("userID");
            request.userID = new Guid(userID);

            var viewData = await process.approve.ApproveKeyInData(request);
            return Json(viewData);
        }

        [HttpPost]
        public async Task<IActionResult> RejectKeyInData([FromBody] RejectKeyInDataRequest request)
        {
            var userID = HttpContext.Session.GetString("userID");
            request.userID = new Guid(userID);

            var viewData = await process.approve.RejectKeyInData(request);
            return Json(viewData);
        }
    }
}
