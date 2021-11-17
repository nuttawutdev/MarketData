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
                            retailerGroupName = c.retailerGroupName,
                            topNumber = c.topNumber
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
                            retailerGroupName = c.retailerGroupName,
                            topNumber = c.topNumber
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
        public IActionResult SelectiveMarket()
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
                            retailerGroupName = c.retailerGroupName,
                            topNumber = c.topNumber
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
        public IActionResult DetailsSales()
        {
            DetailSaleByBrandViewmodel dataModel = new DetailSaleByBrandViewmodel();

            try
            {
                var reportOption = process.report.GetOptionReportDetailSaleBrand();

                if (reportOption != null)
                {
                    if (reportOption.departmentStore != null && reportOption.departmentStore.Any())
                    {
                        var groupStore = reportOption.departmentStore.GroupBy(c => c.retailerGroupName);
                        dataModel.departmentStoreList = reportOption.departmentStore.Select(c => new DepartmentStoreViewModel
                        {
                            departmentStoreID = c.departmentStoreID,
                            departmentStoreName = c.departmentStoreName,
                            retailerGroupName = c.retailerGroupName,
                            topNumber = c.topNumber
                        }).OrderBy(a => a.retailerGroupName).ToList();

                        dataModel.brandList = reportOption.brandList.Select(c => new BrandViewModel
                        {
                            brandID = c.brandID,
                            brandName = c.brandName
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
        public IActionResult RawData()
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
                            retailerGroupName = c.retailerGroupName,
                            topNumber = c.topNumber
                        }).OrderBy(a => a.retailerGroupName).ToList();
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
        public IActionResult SaleByStoreZone()
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
                            retailerGroupName = c.retailerGroupName,
                            topNumber = c.topNumber
                        }).OrderBy(a => a.retailerGroupName).ToList();
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
        public IActionResult SaleByStoreValue()
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
                            retailerGroupName = c.retailerGroupName,
                            topNumber = c.topNumber
                        }).OrderBy(a => a.retailerGroupName).ToList();
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

        [HttpPost]
        public IActionResult GetReportStoreMarketShareZone([FromBody] ReportStoreMarketShareRequest request)
        {
            var reportData = process.report.GetReportStoreMarketShareZone(request);
            string fileName = "DEPARTMENT STORES PANEL";

            if (request.storeRankEnd.HasValue)
            {
                fileName += $"-TOP_{request.storeRankEnd}";
            }

            if (reportData.fileContent != null)
            {
                reportData.referenceFileID = Guid.NewGuid().ToString();
                reportData.fileName = fileName;
                HttpContext.Session.SetString(reportData.referenceFileID, Convert.ToBase64String(reportData.fileContent));
            }

            return Json(reportData);
        }

        [HttpPost]
        public IActionResult GetReportStoreMarketShareValue([FromBody] ReportStoreMarketShareRequest request)
        {
            var reportData = process.report.GetReportStoreMarketShareValue(request);
            string fileName = "Luxury Products";

            if (request.storeRankEnd.HasValue)
            {
                fileName += $"-TOP {request.storeRankEnd}";
            }

            if (reportData.fileContent != null)
            {
                reportData.referenceFileID = Guid.NewGuid().ToString();
                reportData.fileName = fileName;
                HttpContext.Session.SetString(reportData.referenceFileID, Convert.ToBase64String(reportData.fileContent));
            }

            return Json(reportData);
        }

        [HttpPost]
        public IActionResult GetReportSelectvieMarket([FromBody] ReportSelectiveMarketRequest request)
        {
            var reportData = process.report.GetReportSelectiveMarket(request);
            string fileName = "SELECTIVE MARKET THAILAND";

            if (reportData.fileContent != null)
            {
                reportData.referenceFileID = Guid.NewGuid().ToString();
                reportData.fileName = fileName;
                HttpContext.Session.SetString(reportData.referenceFileID, Convert.ToBase64String(reportData.fileContent));
            }

            return Json(reportData);
        }

        [HttpPost]
        public IActionResult GetReportDetailSaleByBrand([FromBody] ReportDetailSaleByBrandRequest request)
        {
            var reportData = process.report.GetReportDetailSaleByBrand(request);
            string fileName = $"Details Sales by Brand {request.brandName}";

            if (reportData.fileContent != null)
            {
                reportData.referenceFileID = Guid.NewGuid().ToString();
                reportData.fileName = fileName;
                HttpContext.Session.SetString(reportData.referenceFileID, Convert.ToBase64String(reportData.fileContent));
            }

            return Json(reportData);
        }

        [HttpPost]
        public IActionResult GetReportRawData([FromBody] ReportExcelDataExportRequest request)
        {
            var reportData = process.report.GetReportExcelDataExporting(request);
            string fileName = "Excel Data Exporting File";

            if (reportData.fileContent != null)
            {
                reportData.referenceFileID = Guid.NewGuid().ToString();
                reportData.fileName = fileName;
                HttpContext.Session.SetString(reportData.referenceFileID, Convert.ToBase64String(reportData.fileContent));
            }

            return Json(reportData);
        }

        [HttpPost]
        public IActionResult GetReportSaleByStoreZone([FromBody] ReportSaleByStoreRequest request)
        {
            var reportData = process.report.GetReportSaleByStoreZone(request);
            string fileName = "Sales by Store (Zone)";

            if (reportData.fileContent != null)
            {
                reportData.referenceFileID = Guid.NewGuid().ToString();
                reportData.fileName = fileName;
                HttpContext.Session.SetString(reportData.referenceFileID, Convert.ToBase64String(reportData.fileContent));
            }

            return Json(reportData);
        }

        [HttpPost]
        public IActionResult GetReportSaleByStoreValue([FromBody] ReportSaleByStoreRequest request)
        {
            var reportData = process.report.GetReportSaleByStoreValue(request);
            string fileName = "Sales by Store (Value)";

            if (reportData.fileContent != null)
            {
                reportData.referenceFileID = Guid.NewGuid().ToString();
                reportData.fileName = fileName;
                HttpContext.Session.SetString(reportData.referenceFileID, Convert.ToBase64String(reportData.fileContent));
            }

            return Json(reportData);
        }

        [HttpGet]
        public virtual ActionResult Download(string fileName, string referenceFileID)
        {
            var fileBase64 = HttpContext.Session.GetString(referenceFileID);

            byte[] data = Convert.FromBase64String(fileBase64);

            HttpContext.Session.Remove(referenceFileID);
            return File(
                    data,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"{fileName}.xlsx");
        }
    }
}
