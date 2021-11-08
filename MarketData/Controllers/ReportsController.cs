using MarketData.Middleware;
using MarketData.Model.Request.Report;
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
    public class ReportsController : Controller
    {
        private readonly Process process;
        public ReportsController(Process process)
        {
            this.process = process;
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult Index()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult StoreMarketShareZone()
        {
            StoreMarketShareZoneViewModel dataModel = new StoreMarketShareZoneViewModel();

            try
            {
                var reportOption = process.report.GetOptionReport();

                if (reportOption != null)
                {
                    if (reportOption.departmentStore != null && reportOption.departmentStore.Any())
                    {
                        var groupStore = reportOption.departmentStore.GroupBy(c => c.retailerGroupName);
                        dataModel.departmentStoreList = reportOption.departmentStore.Select(c => new DepartmentStoreViewModel
                        {
                            departmentStoreID = c.departmentStoreID,
                            departmentStoreName = c.departmentStoreName,
                            retailerGroupName = c.retailerGroupName
                        }).OrderBy(a => a.retailerGroupName).ToList();

                        dataModel.brandTypeList = reportOption.brandType.Select(c => new BrandTypeViewModel
                        {
                            brandTypeID = c.brandTypeID,
                            brandTypeName = c.brandTypeName
                        }).ToList();
                    }

                    dataModel.yearList = reportOption.year;
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            return View(dataModel);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult StoreMarketShareValue()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult SelectiveMarket()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult DetailsSales()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult RawData()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult SaleByStoreZone()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult SaleByStoreValue()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetReportStoreMarketShareZone([FromBody] ReportStoreMarketShareRequest request)
        {
            var reportData = process.report.GetReportStoreMarketShareZone(request);
            string fileName = $"StoreMarketShareZone_{DateTime.Now.ToString("ddMMyyyyHHmm")}";

            if (reportData.fileContent != null)
            {
                reportData.fileName = fileName;
                HttpContext.Session.SetString(reportData.fileName, Convert.ToBase64String(reportData.fileContent));
            }

            return Json(reportData);
        }

        [HttpGet]
        public virtual ActionResult Download(string fileName)
        {
            var fileBase64 = HttpContext.Session.GetString(fileName);

            byte[] data = Convert.FromBase64String(fileBase64);

            HttpContext.Session.Remove(fileName);
            return File(
                    data,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"{fileName}.xlsx");
        }

        [HttpPost]
        public IActionResult ExportReportStoreMarketShareZone([FromBody] ReportStoreMarketShareRequest request)
        {
            var reportData = process.report.GetReportStoreMarketShareZone(request);

            if (reportData != null && reportData.fileContent != null)
            {
                return File(
                    reportData.fileContent,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"StoreMarketShareZone_{DateTime.Now.ToString("ddMMyyyyHHmm")}.xlsx");
            }
            else
            {
                return Json(reportData);
            }
        }
    }
}
