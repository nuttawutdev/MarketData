using MarketData.Models;
using MarketData.Processes;
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

        public IActionResult Approve()
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

                    if(approveStatus != null && approveStatus.Any())
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
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(dataModel);
        }
        public IActionResult Approve_Edit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetApproveKeyInList()
        {
            ApproveKeyInListViewModel dataModel = new ApproveKeyInListViewModel();

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
    }
}
