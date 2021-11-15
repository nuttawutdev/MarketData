using ClosedXML.Excel;
using MarketData.Helper;
using MarketData.Model.Entiry;
using MarketData.Model.Request.Report;
using MarketData.Model.Response.Report;
using MarketData.Repositories;
using Newtonsoft.Json;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MarketData.Processes.Processes
{
    public class ReportProcess
    {
        private readonly Repository repository;
        private string[] monthList = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        public ReportProcess(Repository repository)
        {
            this.repository = repository;
        }

        public GetOptionReportResponse GetOptionReport()
        {
            GetOptionReportResponse response = new GetOptionReportResponse();

            try
            {
                var allDepartmentStore = repository.masterData.GetDepartmentStoreList().Where(c => c.active);
                var brandTypeList = repository.masterData.GetBrandTypeList().Where(c => c.Active_Flag);
                var topStoreList = repository.masterData.GetTopDepartmentStoreList();

                response.departmentStore = allDepartmentStore.Select(c => new Model.Data.DepartmentStoreData
                {
                    departmentStoreID = c.departmentStoreID,
                    departmentStoreName = c.departmentStoreName,
                    topNumber = topStoreList.FirstOrDefault(e => e.DepartmentStore_ID == c.departmentStoreID)?.TopNumber,
                    retailerGroupName = c.retailerGroupName
                }).ToList();

                response.brandType = brandTypeList.Select(c => new Model.Data.BrandTypeData
                {
                    brandTypeID = c.Brand_Type_ID,
                    brandTypeName = c.Brand_Type_Name
                }).ToList();

                List<string> yearList = new List<string>();
                string currentYear = Utility.GetDateNowThai().Year.ToString();

                yearList.Add(currentYear);

                int startYear = 2000;

                for (int i = startYear; i < Utility.GetDateNowThai().Year; i++)
                {
                    yearList.Add(i.ToString());
                }
                response.year = yearList.OrderByDescending(c => c).ToList();
            }
            catch (Exception ex)
            {

            }

            return response;
        }

        public GetOptionReportResponse GetOptionReportDetailSaleBrand()
        {
            GetOptionReportResponse response = new GetOptionReportResponse();

            try
            {
                var allDepartmentStore = repository.masterData.GetDepartmentStoreList().Where(c => c.active);
                var brandList = repository.masterData.GetBrandListBy(c => c.Active_Flag);
                var topStoreList = repository.masterData.GetTopDepartmentStoreList();

                response.departmentStore = allDepartmentStore.Select(c => new Model.Data.DepartmentStoreData
                {
                    departmentStoreID = c.departmentStoreID,
                    departmentStoreName = c.departmentStoreName,
                    retailerGroupName = c.retailerGroupName,
                    topNumber = topStoreList.FirstOrDefault(e => e.DepartmentStore_ID == c.departmentStoreID)?.TopNumber,
                }).ToList();

                response.brandList = brandList.Select(c => new Model.Data.BrandData
                {
                    brandID = c.Brand_ID,
                    brandName = c.Brand_Name
                }).OrderBy(e => e.brandName).ToList();

                List<string> yearList = new List<string>();
                string currentYear = Utility.GetDateNowThai().Year.ToString();

                yearList.Add(currentYear);

                int startYear = 2000;

                for (int i = startYear; i < Utility.GetDateNowThai().Year; i++)
                {
                    yearList.Add(i.ToString());
                }
                response.year = yearList.OrderByDescending(c => c).ToList();
            }
            catch (Exception ex)
            {

            }

            return response;
        }

        public GetOptionReportResponse GetOptionTopDepartmentStore()
        {
            GetOptionReportResponse response = new GetOptionReportResponse();

            try
            {
                var allDepartmentStore = repository.masterData.GetDepartmentStoreList().Where(c => c.active);

                response.departmentStore = allDepartmentStore.Select(c => new Model.Data.DepartmentStoreData
                {
                    departmentStoreID = c.departmentStoreID,
                    departmentStoreName = c.departmentStoreName,
                    retailerGroupName = c.retailerGroupName
                }).ToList();

            }
            catch (Exception ex)
            {

            }

            return response;
        }

        public GenerateReportResponse GetReportStoreMarketShareZone(ReportStoreMarketShareRequest request)
        {
            GenerateReportResponse response = new GenerateReportResponse();

            try
            {
                (List<GroupStoreRanking> groupStoreStartYear, List<GroupStoreRanking> groupStoreCompareYear, List<GroupStoreRanking> groupStoreCompareOldYear) = GetDataForReportStoreMarketShare(request);

                if (groupStoreStartYear.Any() || groupStoreCompareYear.Any() || groupStoreCompareOldYear.Any())
                {
                    (byte[] fileContent, string filePreview) = GenerateReportStoreMarketShareZone(request, groupStoreStartYear, groupStoreCompareYear, groupStoreCompareOldYear);

                    response.fileContent = fileContent;
                    response.filePreview = filePreview;
                    response.success = true;
                }
                else
                {
                    response.success = false;
                    response.responseError = "ไม่พบข้อมูล";
                }

            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public GenerateReportResponse GetReportStoreMarketShareValue(ReportStoreMarketShareRequest request)
        {
            GenerateReportResponse response = new GenerateReportResponse();

            try
            {
                (List<GroupStoreRanking> groupStoreStartYear, List<GroupStoreRanking> groupStoreCompareYear, List<GroupStoreRanking> groupStoreCompareOldYear) = GetDataForReportStoreMarketShare(request);
                if (groupStoreStartYear.Any() || groupStoreCompareYear.Any() || groupStoreCompareOldYear.Any())
                {
                    (byte[] fileContent, string filePreview) = GenerateReportStoreMarketShareValue(request, groupStoreStartYear, groupStoreCompareYear, groupStoreCompareOldYear);

                    response.fileContent = fileContent;
                    response.filePreview = filePreview;
                    response.success = true;
                }
                else
                {
                    response.success = false;
                    response.responseError = "ไม่พบข้อมูล";
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public GenerateReportResponse GetReportSelectiveMarket(ReportSelectiveMarketRequest request)
        {
            GenerateReportResponse response = new GenerateReportResponse();

            try
            {
                (List<GroupBrandRanking> groupBrandStartYear, List<GroupBrandRanking> groupBrandCompareYear) = GetDataForReportSelectiveMarket(request);
                if (groupBrandStartYear.Any() || groupBrandCompareYear.Any())
                {
                    (byte[] fileContent, string filePreview) = GenerateReportSelectiveMarket(request, groupBrandStartYear, groupBrandCompareYear);

                    response.fileContent = fileContent;
                    response.filePreview = filePreview;
                    response.success = true;
                }
                else
                {
                    response.success = false;
                    response.responseError = "ไม่พบข้อมูล";
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public GenerateReportResponse GetReportDetailSaleByBrand(ReportDetailSaleByBrandRequest request)
        {
            GenerateReportResponse response = new GenerateReportResponse();

            try
            {
                var reportData = GetDataForReportDetailSaleByBrand(request);

                if (reportData.Any())
                {
                    (byte[] fileContent, string filePreview) = GenerateReportDetailSaleByBrand(request, reportData);

                    response.fileContent = fileContent;
                    response.filePreview = filePreview;
                    response.success = true;
                }
                else
                {
                    response.success = false;
                    response.responseError = "ไม่พบข้อมูล";
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public GenerateReportResponse GetReportExcelDataExporting(ReportExcelDataExportRequest request)
        {
            GenerateReportResponse response = new GenerateReportResponse();

            try
            {
                var reportData = GetDataForReportDataExporting(request);

                if (reportData.Any())
                {
                    (byte[] fileContent, string filePreview) = GenerateReportExcelDataExporting(request, reportData);

                    response.fileContent = fileContent;
                    response.filePreview = filePreview;
                    response.success = true;
                }
                else
                {
                    response.success = false;
                    response.responseError = "ไม่พบข้อมูล";
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        private (List<GroupStoreRanking>, List<GroupStoreRanking>, List<GroupStoreRanking>) GetDataForReportStoreMarketShare(ReportStoreMarketShareRequest request)
        {
            var jsonRequest = JsonConvert.SerializeObject(request);

            List<GroupStoreRanking> groupStoreStartYear = new List<GroupStoreRanking>();
            List<GroupStoreRanking> groupStoreCompareYear = new List<GroupStoreRanking>();
            List<GroupStoreRanking> groupStoreCompareOldYear = new List<GroupStoreRanking>();

            // MTD
            if (string.IsNullOrWhiteSpace(request.endWeek))
            {
                List<GroupStoreRanking> groupBrandStore = new List<GroupStoreRanking>();
                var brandRankingData = repository.report.GetBrandRankingBy(
                    c => c.Sales_Week == request.startWeek
                    && c.Sales_Month == request.startMonth
                    && (c.Sales_Year == request.startYear || c.Sales_Year == request.compareYear)
                    && (request.brandType == null || c.Brand_Type_ID == request.brandType)
                    && (request.universe == null || c.Universe == request.universe)
                    && (request.departmentStoreList == null
                    || !request.departmentStoreList.Any()
                    || (request.departmentStoreList != null && request.departmentStoreList.Contains(c.Store_Id))));

                if (brandRankingData.Any())
                {
                    var brandRankingStartYear = brandRankingData.Where(w => w.Sales_Year == request.startYear);

                    groupStoreStartYear = brandRankingStartYear.GroupBy(
                        x => new
                        {
                            x.Store_Id,
                            x.Department_Store_Name
                        })
                    .Select(e => new GroupStoreRanking
                    {
                        storeName = e.Key.Department_Store_Name,
                        storeID = e.Key.Store_Id,
                        sumStore = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                              : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                              : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                        brandDetail = request.saleType == "Amount" ? e.OrderByDescending(s => s.Amount_Sales).ToList()
                              : request.saleType == "Whole" ? e.OrderByDescending(s => s.Whole_Sales).ToList()
                              : request.saleType == "Net" ? e.OrderByDescending(s => s.Net_Sales).ToList() : e.ToList()
                    }).OrderByDescending(s => s.sumStore).ToList();

                    if (request.storeRankEnd.HasValue)
                    {
                        if (!request.storeRankStart.HasValue)
                        {
                            request.storeRankStart = 1;
                        }

                        for (int i = request.storeRankStart.GetValueOrDefault() - 1; i <= request.storeRankEnd.GetValueOrDefault() - 1; i++)
                        {
                            if (groupStoreStartYear.ElementAtOrDefault(i) != null)
                            {
                                groupBrandStore.Add(groupStoreStartYear.ElementAt(i));
                            }
                        }

                        groupStoreStartYear = groupBrandStore;
                    }

                    var storeFilter = groupStoreStartYear.Select(e => e.storeID);
                    var brandRankingCompareYear = brandRankingData.Where(
                        w => w.Sales_Year == request.compareYear
                        && storeFilter.Contains(w.Store_Id));

                    groupStoreCompareYear = brandRankingCompareYear.GroupBy(
                    x => new
                    {
                        x.Store_Id,
                        x.Department_Store_Name
                    })
                    .Select(e => new GroupStoreRanking
                    {
                        storeName = e.Key.Department_Store_Name,
                        storeID = e.Key.Store_Id,
                        sumStore = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                              : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                              : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                        brandDetail = request.saleType == "Amount" ? e.OrderByDescending(s => s.Amount_Sales).ToList()
                              : request.saleType == "Whole" ? e.OrderByDescending(s => s.Whole_Sales).ToList()
                              : request.saleType == "Net" ? e.OrderByDescending(s => s.Net_Sales).ToList() : e.ToList()
                    }).OrderByDescending(s => s.sumStore).ToList();

                    var oldCompareYear = (int.Parse(request.compareYear) - 1).ToString();
                    var brandRankingDataOlpCompare = repository.report.GetBrandRankingBy(
                       c => c.Sales_Week == request.startWeek
                       && c.Sales_Month == request.startMonth
                       && c.Sales_Year == oldCompareYear
                       && (request.brandType == null || c.Brand_Type_ID == request.brandType)
                       && (request.universe == null || c.Universe == request.universe)
                       && (request.departmentStoreList == null
                       || !request.departmentStoreList.Any()
                       || (request.departmentStoreList != null && request.departmentStoreList.Contains(c.Store_Id))));

                    groupStoreCompareOldYear = brandRankingDataOlpCompare.Where(c => storeFilter.Contains(c.Store_Id)).GroupBy(
                    x => new
                    {
                        x.Store_Id,
                        x.Department_Store_Name
                    })
                    .Select(e => new GroupStoreRanking
                    {
                        storeName = e.Key.Department_Store_Name,
                        storeID = e.Key.Store_Id,
                        sumStore = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                              : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                              : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                        brandDetail = request.saleType == "Amount" ? e.OrderByDescending(s => s.Amount_Sales).ToList()
                              : request.saleType == "Whole" ? e.OrderByDescending(s => s.Whole_Sales).ToList()
                              : request.saleType == "Net" ? e.OrderByDescending(s => s.Net_Sales).ToList() : e.ToList()
                    }).OrderByDescending(s => s.sumStore).ToList();
                }
            }
            // YTD
            else
            {
                List<GroupStoreRanking> groupBrandStore = new List<GroupStoreRanking>();
                int timeFilterStart = int.Parse(request.startYear + request.startMonth + request.startWeek);
                int timeFilterEnd = int.Parse(request.endYear + request.endMonth + request.endWeek);

                var brandRankingData = repository.report.GetBrandRankingBy(
                   c => (request.brandType == null || c.Brand_Type_ID == request.brandType)
                   && (request.universe == null || c.Universe == request.universe)
                   && (request.departmentStoreList == null
                   || !request.departmentStoreList.Any()
                   || (request.departmentStoreList != null && request.departmentStoreList.Contains(c.Store_Id)))
                   && (c.Time_Keyin >= timeFilterStart && c.Time_Keyin <= timeFilterEnd));

                groupStoreStartYear = brandRankingData.GroupBy(
                    x => new
                    {
                        x.Store_Id,
                        x.Department_Store_Name
                    })
                    .Select(e => new GroupStoreRanking
                    {
                        storeName = e.Key.Department_Store_Name,
                        storeID = e.Key.Store_Id,
                        sumStore = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                                  : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                                  : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                        brandDetail = request.saleType == "Amount" ? e.OrderByDescending(s => s.Amount_Sales).ToList()
                                  : request.saleType == "Whole" ? e.OrderByDescending(s => s.Whole_Sales).ToList()
                                  : request.saleType == "Net" ? e.OrderByDescending(s => s.Net_Sales).ToList() : e.ToList()
                    }).OrderByDescending(s => s.sumStore).ToList();

                if (request.storeRankEnd.HasValue)
                {
                    if (!request.storeRankStart.HasValue)
                    {
                        request.storeRankStart = 1;
                    }

                    for (int i = request.storeRankStart.GetValueOrDefault() - 1; i <= request.storeRankEnd.GetValueOrDefault() - 1; i++)
                    {
                        if (groupStoreStartYear.ElementAtOrDefault(i) != null)
                        {
                            groupBrandStore.Add(groupStoreStartYear.ElementAt(i));
                        }
                    }

                    groupStoreStartYear = groupBrandStore;
                }

                var storeFilter = groupStoreStartYear.Select(e => e.storeID);
                int timeFilterCompareStart = int.Parse(request.compareYear + request.startMonth + request.startWeek);
                int timeFilterCompareEnd = int.Parse(request.compareYear + request.endMonth + request.endWeek);

                var brandRankingCompareData = repository.report.GetBrandRankingBy(
                   c => (request.brandType == null || c.Brand_Type_ID == request.brandType)
                   && (request.universe == null || c.Universe == request.universe)
                   && storeFilter.Contains(c.Store_Id)
                   && (c.Time_Keyin >= timeFilterCompareStart && c.Time_Keyin <= timeFilterCompareEnd));

                groupStoreCompareYear = brandRankingCompareData.GroupBy(
                   x => new
                   {
                       x.Store_Id,
                       x.Department_Store_Name
                   })
                   .Select(e => new GroupStoreRanking
                   {
                       storeName = e.Key.Department_Store_Name,
                       storeID = e.Key.Store_Id,
                       sumStore = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                                  : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                                  : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                       brandDetail = request.saleType == "Amount" ? e.OrderByDescending(s => s.Amount_Sales).ToList()
                                  : request.saleType == "Whole" ? e.OrderByDescending(s => s.Whole_Sales).ToList()
                                  : request.saleType == "Net" ? e.OrderByDescending(s => s.Net_Sales).ToList() : e.ToList()
                   }).OrderByDescending(s => s.sumStore).ToList();

                var oldCompareYear = (int.Parse(request.compareYear) - 1).ToString();
                int timeFilterOldCompareStart = int.Parse(oldCompareYear + request.startMonth + request.startWeek);
                int timeFilterOldCompareEnd = int.Parse(oldCompareYear + request.endMonth + request.endWeek);

                var brandRankingDataOlpCompare = repository.report.GetBrandRankingBy(
                   c => (request.brandType == null || c.Brand_Type_ID == request.brandType)
                   && (request.universe == null || c.Universe == request.universe)
                   && storeFilter.Contains(c.Store_Id)
                   && (c.Time_Keyin >= timeFilterOldCompareStart && c.Time_Keyin <= timeFilterOldCompareEnd));

                groupStoreCompareOldYear = brandRankingDataOlpCompare.Where(c => storeFilter.Contains(c.Store_Id)).GroupBy(
                  x => new
                  {
                      x.Store_Id,
                      x.Department_Store_Name
                  })
                  .Select(e => new GroupStoreRanking
                  {
                      storeName = e.Key.Department_Store_Name,
                      storeID = e.Key.Store_Id,
                      sumStore = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                                  : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                                  : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                      brandDetail = request.saleType == "Amount" ? e.OrderByDescending(s => s.Amount_Sales).ToList()
                                  : request.saleType == "Whole" ? e.OrderByDescending(s => s.Whole_Sales).ToList()
                                  : request.saleType == "Net" ? e.OrderByDescending(s => s.Net_Sales).ToList() : e.ToList()
                  }).OrderByDescending(s => s.sumStore).ToList();
            }

            return (groupStoreStartYear, groupStoreCompareYear, groupStoreCompareOldYear);
        }

        private (List<GroupBrandRanking>, List<GroupBrandRanking>) GetDataForReportSelectiveMarket(ReportSelectiveMarketRequest request)
        {
            List<GroupBrandRanking> groupBrandStartYear = new List<GroupBrandRanking>();
            List<GroupBrandRanking> groupBrandCompareYear = new List<GroupBrandRanking>();
            List<GroupBrandRanking> groupBrandCompareOldYear = new List<GroupBrandRanking>();
            var lorealStore = repository.report.GetLorealStore().Select(d => d.Store_Id);

            // MTD
            if (string.IsNullOrWhiteSpace(request.endWeek))
            {
                List<GroupStoreRanking> groupBrandStore = new List<GroupStoreRanking>();
                var brandRankingData = repository.report.GetBrandRankingBy(
                    c => c.Sales_Week == request.startWeek
                    && c.Sales_Month == request.startMonth
                    && (c.Sales_Year == request.startYear || c.Sales_Year == request.compareYear)
                    && (request.brandType == null || c.Brand_Type_ID == request.brandType)
                    && (request.universe == null || c.Universe == request.universe)
                    && (request.departmentStoreList == null
                    || !request.departmentStoreList.Any()
                    || (request.departmentStoreList != null && request.departmentStoreList.Contains(c.Store_Id))));

                if (request.lorealStore)
                {
                    brandRankingData = brandRankingData.Where(c => lorealStore.Contains(c.Store_Id)).ToList();
                }

                if (brandRankingData.Any())
                {
                    var brandRankingStartYear = brandRankingData.Where(w => w.Sales_Year == request.startYear);

                    groupBrandStartYear = brandRankingStartYear.GroupBy(
                        x => new
                        {
                            x.Brand_ID,
                            x.Brand_Name
                        })
                    .Select(e => new GroupBrandRanking
                    {
                        brandID = e.Key.Brand_ID,
                        brandName = e.Key.Brand_Name,
                        color = e.FirstOrDefault().Report_Color,
                        sumBrand = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                              : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                              : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                        storeDetail = request.saleType == "Amount" ? e.OrderByDescending(s => s.Amount_Sales).ToList()
                              : request.saleType == "Whole" ? e.OrderByDescending(s => s.Whole_Sales).ToList()
                              : request.saleType == "Net" ? e.OrderByDescending(s => s.Net_Sales).ToList() : e.ToList()
                    }).OrderByDescending(s => s.sumBrand).ToList();

                    var brandRankingCompareYear = brandRankingData.Where(w => w.Sales_Year == request.compareYear);

                    groupBrandCompareYear = brandRankingCompareYear.GroupBy(
                    x => new
                    {
                        x.Brand_ID,
                        x.Brand_Name
                    })
                    .Select(e => new GroupBrandRanking
                    {
                        brandID = e.Key.Brand_ID,
                        brandName = e.Key.Brand_Name,
                        color = e.FirstOrDefault().Report_Color,
                        sumBrand = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                              : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                              : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                        storeDetail = request.saleType == "Amount" ? e.OrderByDescending(s => s.Amount_Sales).ToList()
                              : request.saleType == "Whole" ? e.OrderByDescending(s => s.Whole_Sales).ToList()
                              : request.saleType == "Net" ? e.OrderByDescending(s => s.Net_Sales).ToList() : e.ToList()
                    }).OrderByDescending(s => s.sumBrand).ToList();
                }
            }
            // YTD
            else
            {
                int timeFilterStart = int.Parse(request.startYear + request.startMonth + request.startWeek);
                int timeFilterEnd = int.Parse(request.endYear + request.endMonth + request.endWeek);

                var brandRankingData = repository.report.GetBrandRankingBy(
                   c => (request.brandType == null || c.Brand_Type_ID == request.brandType)
                   && (request.universe == null || c.Universe == request.universe)
                   && (request.departmentStoreList == null
                   || !request.departmentStoreList.Any()
                   || (request.departmentStoreList != null && request.departmentStoreList.Contains(c.Store_Id)))
                   && (c.Time_Keyin >= timeFilterStart && c.Time_Keyin <= timeFilterEnd));

                if (request.lorealStore)
                {
                    brandRankingData = brandRankingData.Where(c => lorealStore.Contains(c.Store_Id)).ToList();
                }

                groupBrandStartYear = brandRankingData.GroupBy(
                   x => new
                   {
                       x.Brand_ID,
                       x.Brand_Name
                   })
                    .Select(e => new GroupBrandRanking
                    {
                        brandID = e.Key.Brand_ID,
                        brandName = e.Key.Brand_Name,
                        color = e.FirstOrDefault().Report_Color,
                        sumBrand = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                              : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                              : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                        storeDetail = request.saleType == "Amount" ? e.OrderByDescending(s => s.Amount_Sales).ToList()
                              : request.saleType == "Whole" ? e.OrderByDescending(s => s.Whole_Sales).ToList()
                              : request.saleType == "Net" ? e.OrderByDescending(s => s.Net_Sales).ToList() : e.ToList()
                    }).OrderByDescending(s => s.sumBrand).ToList();

                int timeFilterCompareStart = int.Parse(request.compareYear + request.startMonth + request.startWeek);
                int timeFilterCompareEnd = int.Parse(request.compareYear + request.endMonth + request.endWeek);

                var brandRankingCompareData = repository.report.GetBrandRankingBy(
                   c => (request.brandType == null || c.Brand_Type_ID == request.brandType)
                   && (request.universe == null || c.Universe == request.universe)
                    && (request.departmentStoreList == null
                   || !request.departmentStoreList.Any()
                   || (request.departmentStoreList != null && request.departmentStoreList.Contains(c.Store_Id)))
                   && (c.Time_Keyin >= timeFilterCompareStart && c.Time_Keyin <= timeFilterCompareEnd));

                if (!request.lorealStore)
                {
                    brandRankingCompareData = brandRankingCompareData.Where(c => lorealStore.Contains(c.Store_Id)).ToList();
                }

                groupBrandCompareYear = brandRankingCompareData.GroupBy(
                  x => new
                  {
                      x.Brand_ID,
                      x.Brand_Name
                  })
                    .Select(e => new GroupBrandRanking
                    {
                        brandID = e.Key.Brand_ID,
                        brandName = e.Key.Brand_Name,
                        color = e.FirstOrDefault().Report_Color,
                        sumBrand = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                              : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                              : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                        storeDetail = request.saleType == "Amount" ? e.OrderByDescending(s => s.Amount_Sales).ToList()
                              : request.saleType == "Whole" ? e.OrderByDescending(s => s.Whole_Sales).ToList()
                              : request.saleType == "Net" ? e.OrderByDescending(s => s.Net_Sales).ToList() : e.ToList()
                    }).OrderByDescending(s => s.sumBrand).ToList();
            }

            return (groupBrandStartYear, groupBrandCompareYear);
        }

        private List<GroupStoreRanking> GetDataForReportDetailSaleByBrand(ReportDetailSaleByBrandRequest request)
        {
            List<GroupStoreRanking> groupStoreStartYear = new List<GroupStoreRanking>();
            var lorealStore = repository.report.GetLorealStore().Select(d => d.Store_Id);

            // MTD
            if (string.IsNullOrWhiteSpace(request.endWeek))
            {
                var brandRankingData = repository.report.GetBrandRankingBy(
                    c => c.Sales_Week == request.startWeek
                    && c.Sales_Month == request.startMonth
                    && c.Sales_Year == request.startYear
                    && c.Brand_ID == request.brandID
                    && (request.departmentStoreList == null
                    || !request.departmentStoreList.Any()
                    || (request.departmentStoreList != null && request.departmentStoreList.Contains(c.Store_Id))));

                if (brandRankingData.Any())
                {
                    groupStoreStartYear = brandRankingData.GroupBy(
                        x => new
                        {
                            x.Store_Id,
                            x.Department_Store_Name,
                            x.Sales_Year,
                            x.Sales_Month
                        })
                    .Select(e => new GroupStoreRanking
                    {
                        storeName = e.Key.Department_Store_Name,
                        storeID = e.Key.Store_Id,
                        year = e.Key.Sales_Year,
                        month = e.Key.Sales_Month,
                        sumStore = e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                    }).OrderBy(s => s.year).ThenBy(b => b.month).ToList();
                }
            }
            // YTD
            else
            {
                int timeFilterStart = int.Parse(request.startYear + request.startMonth + request.startWeek);
                int timeFilterEnd = int.Parse(request.endYear + request.endMonth + request.endWeek);

                var brandRankingData = repository.report.GetBrandRankingBy(
                   c =>
                   (request.departmentStoreList == null
                   || !request.departmentStoreList.Any()
                   || (request.departmentStoreList != null && request.departmentStoreList.Contains(c.Store_Id)))
                    && c.Brand_ID == request.brandID
                   && (c.Time_Keyin >= timeFilterStart && c.Time_Keyin <= timeFilterEnd));

                groupStoreStartYear = brandRankingData.GroupBy(
                         x => new
                         {
                             x.Store_Id,
                             x.Department_Store_Name,
                             x.Sales_Year,
                             x.Sales_Month
                         })
                     .Select(e => new GroupStoreRanking
                     {
                         storeName = e.Key.Department_Store_Name,
                         storeID = e.Key.Store_Id,
                         year = e.Key.Sales_Year,
                         month = e.Key.Sales_Month,
                         sumStore = e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                     }).OrderBy(s => s.year).ThenBy(b => b.month).ToList();
            }

            return groupStoreStartYear;
        }

        private List<Data_Exporting> GetDataForReportDataExporting(ReportExcelDataExportRequest request)
        {
            List<Data_Exporting> data = new List<Data_Exporting>();

            // MTD
            if (string.IsNullOrWhiteSpace(request.endWeek))
            {
                data = repository.report.GetDataExportingBy(
                    c => c.Sales_Week == request.startWeek
                    && c.Sales_Month == request.startMonth
                    && c.Sales_Year == request.startYear
                    && (request.universe == null || c.Universe == request.universe)
                    && (request.departmentStoreList == null
                    || !request.departmentStoreList.Any()
                    || (request.departmentStoreList != null && request.departmentStoreList.Contains(c.Store_Id)))).OrderBy(s => s.Store_Name).ToList();
            }
            // YTD
            else
            {
                int timeFilterStart = int.Parse(request.startYear + request.startMonth + request.startWeek);
                int timeFilterEnd = int.Parse(request.endYear + request.endMonth + request.endWeek);

                data = repository.report.GetDataExportingBy(
                   c =>
                   (request.departmentStoreList == null
                   || !request.departmentStoreList.Any()
                   || (request.departmentStoreList != null && request.departmentStoreList.Contains(c.Store_Id)))
                   && (request.universe == null || c.Universe == request.universe)
                   && (c.Time_Keyin >= timeFilterStart && c.Time_Keyin <= timeFilterEnd)).OrderBy(s => s.Store_Name).ToList();
            }

            return data;
        }

        private (byte[], string) GenerateReportStoreMarketShareZone(ReportStoreMarketShareRequest request, List<GroupStoreRanking> listGroup, List<GroupStoreRanking> listGroupCompare, List<GroupStoreRanking> listGroupOldCompare)
        {

            using (var workbook = new XLWorkbook())
            {
                Color yellowHead = Color.FromArgb(250, 250, 181);
                Color greenHead = Color.FromArgb(184, 246, 184);
                Color storeHead = Color.FromArgb(199, 188, 222);
                Color totalHead = Color.FromArgb(239, 82, 227);
                Color black = Color.FromArgb(0, 0, 0);
                Color lorealHead = Color.FromArgb(229, 132, 121);

                XLColor yellowXL = XLColor.FromArgb(yellowHead.A, yellowHead.R, yellowHead.G, yellowHead.B);
                XLColor greenXL = XLColor.FromArgb(greenHead.A, greenHead.R, greenHead.G, greenHead.B);
                XLColor storeXL = XLColor.FromArgb(storeHead.A, storeHead.R, storeHead.G, storeHead.B);
                XLColor totalXL = XLColor.FromArgb(totalHead.A, totalHead.R, totalHead.G, totalHead.B);
                XLColor blackXL = XLColor.FromArgb(black.A, black.R, black.G, black.B);
                XLColor lorealXL = XLColor.FromArgb(lorealHead.A, lorealHead.R, lorealHead.G, lorealHead.B);

                var worksheet = workbook.Worksheets.Add("StoreMarketShareZone");
                if (!request.brandRankEnd.HasValue)
                {
                    var allBrandActive = repository.masterData.GetBrandListBy(c => c.Brand_ID != null);      
                    request.brandRankEnd = allBrandActive.Count();
                }

                int columnBrand = 6;
                for (int i = 1; i <= request.brandRankEnd; i++)
                {
                    worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(7, columnBrand + 1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(7, columnBrand + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(7, columnBrand + 1)).Merge();
                    worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(7, columnBrand + 1)).Value = $"#{i}";
                    columnBrand = columnBrand + 2;
                }

                columnBrand++;
                // Loreal Brand
                List<string> brandLorealList = new List<string>();
                var brandLorealListCurrent = listGroup.SelectMany(c => c.brandDetail.Where(e => e.Is_Loreal_Brand)).GroupBy(d => d.Brand_Name).Select(x => x.Key).OrderBy(d => d).ToList();
                var brandLorealListCompare = listGroupCompare.SelectMany(c => c.brandDetail.Where(e => e.Is_Loreal_Brand)).GroupBy(d => d.Brand_Name).Select(x => x.Key).OrderBy(d => d).ToList();
                var brandLorealListOldCompare = listGroupOldCompare.SelectMany(c => c.brandDetail.Where(e => e.Is_Loreal_Brand)).GroupBy(d => d.Brand_Name).Select(x => x.Key).OrderBy(d => d).ToList();

                brandLorealList.AddRange(brandLorealListCurrent);
                brandLorealList.AddRange(brandLorealListCompare);
                brandLorealList.AddRange(brandLorealListOldCompare);

                brandLorealList = brandLorealList.GroupBy(d => d).Select(x => x.Key).OrderBy(d => d).ToList();

                for (int i = 0; i < brandLorealList.Count(); i++)
                {
                    worksheet.Column(columnBrand).Width = 22;
                    worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(7, columnBrand + 1)).Style.Fill.BackgroundColor = lorealXL;
                    worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(6, columnBrand + 1)).Value = $"{brandLorealList[i]}";
                    worksheet.Range(worksheet.Cell(7, columnBrand), worksheet.Cell(7, columnBrand + 1)).Value = $"If Not In Top {request.brandRankEnd}";
                    worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(6, columnBrand + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(6, columnBrand + 1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(6, columnBrand + 1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                    worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(6, columnBrand + 1)).Merge();
                    worksheet.Range(worksheet.Cell(7, columnBrand), worksheet.Cell(7, columnBrand + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(7, columnBrand), worksheet.Cell(7, columnBrand + 1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(7, columnBrand), worksheet.Cell(7, columnBrand + 1)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(7, columnBrand), worksheet.Cell(7, columnBrand + 1)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(7, columnBrand), worksheet.Cell(7, columnBrand + 1)).Merge();
                    columnBrand = columnBrand + 2;
                }

                #region Header

                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, columnBrand - 1)).Merge();
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, columnBrand - 1)).Value = $"DEPARTMENT STORES PANEL - TOP {request.brandRankEnd}";
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, columnBrand - 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, columnBrand - 1)).Merge();
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, columnBrand - 1)).Value = "MARKET SHARE AND GROWTH";
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, columnBrand - 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, columnBrand - 1)).Merge();

                string dateRepport = string.Empty;
                int monthStart = int.Parse(request.startMonth);

                dateRepport = $"{monthList[monthStart - 1]}/{request.startYear}";
                dateRepport += request.endMonth != null ? $" - {monthList[int.Parse(request.endMonth) - 1]}/{request.endYear}" : "";

                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, columnBrand - 1)).SetValue(Convert.ToString(dateRepport));
                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, columnBrand - 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 22;
                worksheet.Column(3).Width = 22;
                worksheet.Column(4).Width = 19;
                worksheet.Column(5).Width = 19;

                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(7, 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(7, 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(7, 1)).Merge();
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(7, 1)).Value = "DEPARTMENT STORES";

                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(7, 1)).Style.Fill.BackgroundColor = yellowXL;
                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(7, 5)).Style.Fill.BackgroundColor = yellowXL;

                worksheet.Cell(4, 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(5, 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(6, 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(7, 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(4, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(4, 2).Value = "RETAIL";
                worksheet.Cell(5, 2).Value = "SALES";
                string periodTimeCompare = $"w{request.startWeek},{request.startMonth}/{request.compareYear}";
                periodTimeCompare += !string.IsNullOrWhiteSpace(request.endWeek) ? $" - w{request.endWeek},{request.endMonth}/{request.compareYear}" : "";

                worksheet.Cell(6, 2).SetValue(Convert.ToString(periodTimeCompare));
                worksheet.Cell(4, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(5, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(6, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(4, 3).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(4, 3).Value = "RETAIL";
                worksheet.Cell(5, 3).Value = "SALES";
                string periodTime = $"w{request.startWeek},{request.startMonth}/{request.startYear}";
                periodTime += !string.IsNullOrWhiteSpace(request.endWeek) ? $" - w{request.endWeek},{request.endMonth}/{request.endYear}" : "";

                worksheet.Cell(6, 3).SetValue(Convert.ToString(periodTime));
                worksheet.Cell(4, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(5, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(6, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(4, 3).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(5, 3).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(6, 3).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(7, 3).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(4, 4).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(4, 4).Value = "%OF";
                worksheet.Cell(5, 4).Value = "GROWTH*";
                worksheet.Cell(6, 4).SetValue(Convert.ToString(request.startYear));
                worksheet.Cell(4, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(5, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(6, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(4, 4).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(5, 4).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(6, 4).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(7, 4).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(4, 5).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(4, 5).Value = "%OF";
                worksheet.Cell(5, 5).Value = "SHARE*";
                worksheet.Cell(6, 5).SetValue(Convert.ToString(request.startYear)); //// Gen
                worksheet.Cell(4, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(5, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(6, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(4, 5).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(5, 5).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(6, 5).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(7, 5).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(5, columnBrand - 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(5, columnBrand - 1)).Merge();
                worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(5, columnBrand - 1)).Value = "RANKING";
                worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(5, columnBrand - 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(5, columnBrand - 1)).Style.Fill.BackgroundColor = greenXL;
                #endregion

                int rowData = 8;
                decimal sumAllStore = listGroup.Sum(c => c.sumStore);
                decimal sumAllStoreCompare = listGroupCompare.Sum(c => c.sumStore);

                foreach (var itemGroup in listGroup)
                {
                    var storeCompare = listGroupCompare.FirstOrDefault(c => c.storeID == itemGroup.storeID);

                    worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 1)).Merge();
                    worksheet.Range(worksheet.Cell(rowData, 2), worksheet.Cell(rowData + 1, 2)).Merge();
                    worksheet.Range(worksheet.Cell(rowData, 3), worksheet.Cell(rowData + 1, 3)).Merge();
                    worksheet.Range(worksheet.Cell(rowData, 4), worksheet.Cell(rowData + 1, 4)).Merge();
                    worksheet.Range(worksheet.Cell(rowData, 5), worksheet.Cell(rowData + 1, 5)).Merge();

                    worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(rowData, 2), worksheet.Cell(rowData + 1, 2)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(rowData, 3), worksheet.Cell(rowData + 1, 3)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(rowData, 4), worksheet.Cell(rowData + 1, 4)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(rowData, 5), worksheet.Cell(rowData + 1, 5)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(rowData, 2), worksheet.Cell(rowData + 1, 2)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Range(worksheet.Cell(rowData, 3), worksheet.Cell(rowData + 1, 3)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Range(worksheet.Cell(rowData, 4), worksheet.Cell(rowData + 1, 4)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Range(worksheet.Cell(rowData, 5), worksheet.Cell(rowData + 1, 5)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 1)).Value = itemGroup.storeName;
                    worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 1)).Style.Fill.BackgroundColor = storeXL;
                    worksheet.Range(worksheet.Cell(rowData, 2), worksheet.Cell(rowData + 1, 2)).SetValue(storeCompare != null ? string.Format("{0:#,0}", storeCompare.sumStore) : "0");
                    worksheet.Range(worksheet.Cell(rowData, 3), worksheet.Cell(rowData + 1, 3)).SetValue(string.Format("{0:#,0}", itemGroup.sumStore));

                    var percentGrowth = Math.Round(storeCompare != null && storeCompare.sumStore != 0 ? ((itemGroup.sumStore / storeCompare.sumStore) - 1) * 100 : -100, 2);
                    worksheet.Range(worksheet.Cell(rowData, 4), worksheet.Cell(rowData + 1, 4)).SetValue($"{percentGrowth}%");

                    var percentShare = Math.Round((itemGroup.sumStore / sumAllStore) * 100, 2);
                    worksheet.Range(worksheet.Cell(rowData, 5), worksheet.Cell(rowData + 1, 5)).SetValue($"{percentShare}%");

                    int columnBrandDetail = 6;
                    List<GroupBrandRanking> listBrandSelect = new List<GroupBrandRanking>();

                    if (!request.brandRankStart.HasValue)
                    {
                        request.brandRankStart = 1;
                    }

                    var groupBrandData = itemGroup.brandDetail.GroupBy(
                     x => new
                     {
                         x.Brand_ID,
                         x.Brand_Name
                     })
                     .Select(e => new GroupBrandRanking
                     {
                         brandID = e.Key.Brand_ID,
                         brandName = e.Key.Brand_Name,
                         color = e.FirstOrDefault().Report_Color,
                         sumBrand = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                      : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                      : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                         storeDetail = e.ToList()
                     }).OrderByDescending(s => s.sumBrand).ToList();


                    for (int i = request.brandRankStart.GetValueOrDefault() - 1; i <= request.brandRankEnd.GetValueOrDefault() - 1; i++)
                    {
                        if (groupBrandData.ElementAtOrDefault(i) != null)
                        {
                            listBrandSelect.Add(groupBrandData.ElementAtOrDefault(i));
                        }
                    }

                    int countBrandRender = 0;
                    foreach (var itemBrand in listBrandSelect)
                    {
                        if (!string.IsNullOrWhiteSpace(itemBrand.color) && itemBrand.color.ToLower() != "#ffffff")
                        {
                            try
                            {
                                Color colorBrand = System.Drawing.ColorTranslator.FromHtml(itemBrand.color);
                                XLColor colorBrandXL = XLColor.FromArgb(colorBrand.A, colorBrand.R, colorBrand.G, colorBrand.B);

                                worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Style.Fill.BackgroundColor = colorBrandXL;
                            }
                            catch (Exception ex)
                            {

                            }
                        }


                        if (storeCompare != null)
                        {
                            var brandCompare = storeCompare.brandDetail.FirstOrDefault(c => c.Brand_ID == itemBrand.brandID);
                            decimal percentGrowthBrand = -100;
                            percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Amount_Sales.GetValueOrDefault() != 0
                                    ? ((itemBrand.sumBrand / brandCompare.Amount_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);

                            //if (request.saleType == "Amount")
                            //{
                            //    percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Amount_Sales.GetValueOrDefault() != 0
                            //        ? ((itemBrand.Amount_Sales.GetValueOrDefault() / brandCompare.Amount_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            //}
                            //else if (request.saleType == "Whole")
                            //{
                            //    percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Whole_Sales.GetValueOrDefault() != 0
                            //         ? ((itemBrand.Whole_Sales.GetValueOrDefault() / brandCompare.Whole_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            //}
                            //else if (request.saleType == "Net")
                            //{
                            //    percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Net_Sales.GetValueOrDefault() != 0
                            //        ? ((itemBrand.Net_Sales.GetValueOrDefault() / brandCompare.Net_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            //}


                            worksheet.Cell(rowData + 1, columnBrandDetail + 1).SetValue($"{percentGrowthBrand}%");
                        }
                        else
                        {
                            worksheet.Cell(rowData + 1, columnBrandDetail + 1).SetValue($"-100%");
                        }

                        worksheet.Cell(rowData + 1, columnBrandDetail + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(rowData + 1, columnBrandDetail + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Merge();
                        worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).SetValue(itemBrand.brandName);

                        decimal percentShareBrand = 0;
                        percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.sumBrand / itemGroup.sumStore) * 100, 2) : 0;
                        //if (request.saleType == "Amount")
                        //{
                        //    percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Amount_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        //}
                        //else if (request.saleType == "Whole")
                        //{
                        //    percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Whole_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        //}
                        //else if (request.saleType == "Net")
                        //{
                        //    percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Net_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        //}

                        worksheet.Cell(rowData + 1, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(rowData + 1, columnBrandDetail).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 1, columnBrandDetail).SetValue($"{percentShareBrand}%");
                        columnBrandDetail = columnBrandDetail + 2;
                        countBrandRender++;
                    }

                    if (countBrandRender < request.brandRankEnd.GetValueOrDefault())
                    {
                        for (int i = countBrandRender; i < request.brandRankEnd.GetValueOrDefault(); i++)
                        {
                            worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 1, columnBrandDetail + 1)).Merge();
                            worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 1, columnBrandDetail + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            columnBrandDetail = columnBrandDetail + 2;
                            countBrandRender++;
                        }
                    }

                    var listBrandTopSelectName = listBrandSelect.Select(c => c.brandName);
                    columnBrandDetail++;

                    foreach (var itemBrandLoreal in brandLorealList)
                    {
                        bool haveData = false;
                        for (int i = 0; i < itemGroup.brandDetail.Count(); i++)
                        {
                            var brandNotTopDetail = itemGroup.brandDetail[i];
                            if (brandNotTopDetail.Brand_Name == itemBrandLoreal
                                && !listBrandTopSelectName.Contains(brandNotTopDetail.Brand_Name))
                            {
                                try
                                {
                                    Color colorBrand = System.Drawing.ColorTranslator.FromHtml(brandNotTopDetail.Report_Color);
                                    XLColor colorBrandXL = XLColor.FromArgb(colorBrand.A, colorBrand.R, colorBrand.G, colorBrand.B);
                                    worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Style.Fill.BackgroundColor = colorBrandXL;
                                }
                                catch (Exception ex)
                                {

                                }

                                haveData = true;
                                worksheet.Column(columnBrandDetail).Width = 11;
                                worksheet.Column(columnBrandDetail + 1).Width = 11;
                                worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Value = $"{itemBrandLoreal} [#{i + 1}]";
                                worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Merge();

                                decimal percentShareBrand = 0;
                                if (request.saleType == "Amount")
                                {
                                    percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((brandNotTopDetail.Amount_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                                }
                                else if (request.saleType == "Whole")
                                {
                                    percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((brandNotTopDetail.Whole_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                                }
                                else if (request.saleType == "Net")
                                {
                                    percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((brandNotTopDetail.Net_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                                }

                                worksheet.Cell(rowData + 1, columnBrandDetail).SetValue($"{percentShareBrand}%");
                                worksheet.Cell(rowData + 1, columnBrandDetail).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                worksheet.Cell(rowData + 1, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                if (storeCompare != null)
                                {
                                    var brandCompare = storeCompare.brandDetail.FirstOrDefault(c => c.Brand_ID == brandNotTopDetail.Brand_ID);
                                    decimal percentGrowthBrand = Math.Round(-100M, 2);
                                    if (request.saleType == "Amount")
                                    {
                                        percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Amount_Sales.GetValueOrDefault() != 0 ? ((brandNotTopDetail.Amount_Sales.GetValueOrDefault() / brandCompare.Amount_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                                    }
                                    else if (request.saleType == "Whole")
                                    {
                                        percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Whole_Sales.GetValueOrDefault() != 0 ? ((brandNotTopDetail.Whole_Sales.GetValueOrDefault() / brandCompare.Whole_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                                    }
                                    else if (request.saleType == "Net")
                                    {
                                        percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Net_Sales.GetValueOrDefault() != 0 ? ((brandNotTopDetail.Net_Sales.GetValueOrDefault() / brandCompare.Net_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                                    }

                                    worksheet.Cell(rowData + 1, columnBrandDetail + 1).SetValue($"{percentGrowthBrand}%");
                                }
                                else
                                {
                                    worksheet.Cell(rowData + 1, columnBrandDetail + 1).SetValue($"-100%");
                                }

                                worksheet.Cell(rowData + 1, columnBrandDetail + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                worksheet.Cell(rowData + 1, columnBrandDetail + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                break;
                            }
                        }

                        if (!haveData)
                        {
                            worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 1, columnBrandDetail + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 1, columnBrandDetail + 1)).Merge();
                        }


                        columnBrandDetail = columnBrandDetail + 2;
                    }

                    rowData = rowData + 2;
                }

                var percentGrowthTotal = Math.Round(((sumAllStore / sumAllStoreCompare) - 1) * 100, 2);
                var allBrandDetail = listGroup.SelectMany(c => c.brandDetail);
                var allBrandDetailCompare = listGroupCompare.SelectMany(c => c.brandDetail);
                var allBrandDetailOldCompare = listGroupOldCompare.SelectMany(c => c.brandDetail);

                #region GroupData total
                var groupBrandDetail = allBrandDetail.GroupBy(
                       x => new
                       {
                           x.Brand_ID,
                           x.Brand_Name
                       })
                       .Select(e => new
                       {
                           brandID = e.Key.Brand_ID,
                           brandName = e.Key.Brand_Name,
                           Report_Color = e.FirstOrDefault().Report_Color,
                           sumTotalBrand = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                        : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                        : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                           detail = e.ToList()
                       }).OrderByDescending(s => s.sumTotalBrand).ToList();

                var groupBrandDetailCompare = allBrandDetailCompare.GroupBy(
                     x => new
                     {
                         x.Brand_ID,
                         x.Brand_Name
                     })
                     .Select(e => new
                     {
                         brandID = e.Key.Brand_ID,
                         brandName = e.Key.Brand_Name,
                         Report_Color = e.FirstOrDefault().Report_Color,
                         sumTotalBrand = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                        : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                        : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                         detail = e.ToList()
                     }).OrderByDescending(s => s.sumTotalBrand).ToList();

                var groupBrandDetailOldCompare = allBrandDetailOldCompare.GroupBy(
                    x => new
                    {
                        x.Brand_ID,
                        x.Brand_Name
                    })
                    .Select(e => new
                    {
                        brandID = e.Key.Brand_ID,
                        brandName = e.Key.Brand_Name,
                        Report_Color = e.FirstOrDefault().Report_Color,
                        sumTotalBrand = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                        : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                        : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                        detail = e.ToList()
                    }).OrderByDescending(s => s.sumTotalBrand).ToList();

                #endregion

                #region Total 1
                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 1)).Merge();
                worksheet.Range(worksheet.Cell(rowData, 2), worksheet.Cell(rowData + 1, 2)).Merge();
                worksheet.Range(worksheet.Cell(rowData, 3), worksheet.Cell(rowData + 1, 3)).Merge();
                worksheet.Range(worksheet.Cell(rowData, 4), worksheet.Cell(rowData + 1, 4)).Merge();
                worksheet.Range(worksheet.Cell(rowData, 5), worksheet.Cell(rowData + 1, 5)).Merge();

                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                worksheet.Range(worksheet.Cell(rowData, 2), worksheet.Cell(rowData + 1, 2)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                worksheet.Range(worksheet.Cell(rowData, 3), worksheet.Cell(rowData + 1, 3)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                worksheet.Range(worksheet.Cell(rowData, 4), worksheet.Cell(rowData + 1, 4)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                worksheet.Range(worksheet.Cell(rowData, 5), worksheet.Cell(rowData + 1, 5)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 2)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 3)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 4)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 5)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 1)).Style.Fill.BackgroundColor = totalXL;
                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 1)).Value = $"TOTAL {request.startYear}";
                worksheet.Range(worksheet.Cell(rowData, 2), worksheet.Cell(rowData + 1, 2)).SetValue(string.Format("{0:#,0}", sumAllStoreCompare));
                worksheet.Range(worksheet.Cell(rowData, 3), worksheet.Cell(rowData + 1, 3)).SetValue(string.Format("{0:#,0}", sumAllStore));
                worksheet.Range(worksheet.Cell(rowData, 4), worksheet.Cell(rowData + 1, 4)).SetValue($"{percentGrowthTotal}%");
                worksheet.Range(worksheet.Cell(rowData, 5), worksheet.Cell(rowData + 1, 5)).SetValue($"100%");

                int columnBrandTotal = 6;
                List<string> brandTotalSelect = new List<string>();
                for (int i = 0; i < request.brandRankEnd; i++)
                {
                    if (i < groupBrandDetail.Count())
                    {
                        var brandDetail = groupBrandDetail[i];

                        try
                        {
                            Color colorBrand = System.Drawing.ColorTranslator.FromHtml(brandDetail.Report_Color);
                            XLColor colorBrandXL = XLColor.FromArgb(colorBrand.A, colorBrand.R, colorBrand.G, colorBrand.B);
                            worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Style.Fill.BackgroundColor = colorBrandXL;
                        }
                        catch (Exception ex)
                        {

                        }

                        brandTotalSelect.Add(brandDetail.brandName);
                        var brandCompare = groupBrandDetailCompare.FirstOrDefault(c => c.brandID == brandDetail.brandID);

                        var percentShareBrand = Math.Round((brandDetail.sumTotalBrand / sumAllStore) * 100, 2);
                        worksheet.Cell(rowData + 1, columnBrandTotal).SetValue($"{percentShareBrand}%");
                        worksheet.Cell(rowData + 1, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 1, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 1, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(rowData + 1, columnBrandTotal + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        if (brandCompare != null)
                        {
                            var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumTotalBrand > 0 ? ((brandDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
                            worksheet.Cell(rowData + 1, columnBrandTotal + 1).SetValue($"{percentGrowthBrand}%");
                        }
                        else
                        {
                            worksheet.Cell(rowData + 1, columnBrandTotal + 1).SetValue($"-100%");
                        }

                        worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Merge();
                        worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).SetValue(brandDetail.brandName);

                        columnBrandTotal = columnBrandTotal + 2;
                    }
                    else
                    {

                        worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData + 1, columnBrandTotal + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData + 1, columnBrandTotal + 1)).Merge();
                        columnBrandTotal = columnBrandTotal + 2;
                    }


                }

                columnBrandTotal++;

                foreach (var itemBrandLoreal in brandLorealList)
                {
                    bool haveData = false;
                    for (int i = 0; i < groupBrandDetail.Count(); i++)
                    {
                        var brandNotTopDetail = groupBrandDetail[i];
                        if (brandNotTopDetail.brandName == itemBrandLoreal
                            && !brandTotalSelect.Contains(brandNotTopDetail.brandName))
                        {
                            try
                            {
                                Color colorBrand = System.Drawing.ColorTranslator.FromHtml(brandNotTopDetail.Report_Color);
                                XLColor colorBrandXL = XLColor.FromArgb(colorBrand.A, colorBrand.R, colorBrand.G, colorBrand.B);
                                worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Style.Fill.BackgroundColor = colorBrandXL;
                            }
                            catch (Exception ex)
                            {

                            }

                            haveData = true;
                            worksheet.Column(columnBrandTotal).Width = 11;
                            worksheet.Column(columnBrandTotal + 1).Width = 11;

                            worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Value = $"{itemBrandLoreal} [#{i + 1}]";
                            worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Merge();
                            worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            var percentShareBrand = Math.Round((brandNotTopDetail.sumTotalBrand / sumAllStore) * 100, 2);
                            worksheet.Cell(rowData + 1, columnBrandTotal).SetValue($"{percentShareBrand}%");
                            worksheet.Cell(rowData + 1, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(rowData + 1, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(rowData + 1, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            worksheet.Cell(rowData + 1, columnBrandTotal + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            var brandCompare = groupBrandDetailCompare.FirstOrDefault(c => c.brandID == brandNotTopDetail.brandID);

                            if (brandCompare != null)
                            {
                                var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumTotalBrand > 0 ? ((brandNotTopDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
                                worksheet.Cell(rowData + 1, columnBrandTotal + 1).SetValue($"{percentGrowthBrand}%");
                            }
                            else
                            {
                                worksheet.Cell(rowData + 1, columnBrandTotal + 1).SetValue($"-100%");
                            }

                            break;
                        }
                    }

                    if (!haveData)
                    {
                        worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData + 1, columnBrandTotal + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData + 1, columnBrandTotal + 1)).Merge();
                    }

                    columnBrandTotal = columnBrandTotal + 2;
                }
                #endregion

                #region Total 2

                worksheet.Range(worksheet.Cell(rowData + 2, 1), worksheet.Cell(rowData + 3, 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                worksheet.Range(worksheet.Cell(rowData + 2, 2), worksheet.Cell(rowData + 3, 2)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                worksheet.Range(worksheet.Cell(rowData + 2, 1), worksheet.Cell(rowData + 3, 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(rowData + 2, 1), worksheet.Cell(rowData + 3, 2)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(rowData + 2, 1), worksheet.Cell(rowData + 3, 3)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(rowData + 2, 1), worksheet.Cell(rowData + 3, 4)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(rowData + 2, 1), worksheet.Cell(rowData + 3, 5)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Range(worksheet.Cell(rowData + 2, 1), worksheet.Cell(rowData + 3, 1)).Merge();
                worksheet.Range(worksheet.Cell(rowData + 2, 2), worksheet.Cell(rowData + 3, 2)).Merge();
                worksheet.Range(worksheet.Cell(rowData + 2, 3), worksheet.Cell(rowData + 3, 3)).Merge();
                worksheet.Range(worksheet.Cell(rowData + 2, 4), worksheet.Cell(rowData + 3, 4)).Merge();
                worksheet.Range(worksheet.Cell(rowData + 2, 5), worksheet.Cell(rowData + 3, 5)).Merge();

                worksheet.Range(worksheet.Cell(rowData + 2, 3), worksheet.Cell(rowData + 3, 3)).Style.Fill.BackgroundColor = blackXL;
                worksheet.Range(worksheet.Cell(rowData + 2, 4), worksheet.Cell(rowData + 3, 4)).Style.Fill.BackgroundColor = blackXL;
                worksheet.Range(worksheet.Cell(rowData + 2, 5), worksheet.Cell(rowData + 3, 5)).Style.Fill.BackgroundColor = blackXL;

                worksheet.Range(worksheet.Cell(rowData + 2, 1), worksheet.Cell(rowData + 3, 1)).Style.Fill.BackgroundColor = totalXL;
                worksheet.Range(worksheet.Cell(rowData + 2, 1), worksheet.Cell(rowData + 3, 1)).Value = $"TOTAL {request.compareYear}";
                worksheet.Range(worksheet.Cell(rowData + 2, 2), worksheet.Cell(rowData + 3, 2)).SetValue(string.Format("{0:#,0}", sumAllStoreCompare));

                int columnBrandTotalCompare = 6;

                for (int i = 0; i < request.brandRankEnd; i++)
                {
                    if (i < groupBrandDetailCompare.Count())
                    {
                        var brandDetail = groupBrandDetailCompare.ElementAtOrDefault(i);

                        try
                        {
                            Color colorBrand = System.Drawing.ColorTranslator.FromHtml(brandDetail.Report_Color);
                            XLColor colorBrandXL = XLColor.FromArgb(colorBrand.A, colorBrand.R, colorBrand.G, colorBrand.B);
                            worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Style.Fill.BackgroundColor = colorBrandXL;
                        }
                        catch (Exception ex)
                        {

                        }

                        var brandCompare = groupBrandDetailOldCompare.FirstOrDefault(c => c.brandID == brandDetail.brandID);

                        var percentShareBrand = Math.Round((brandDetail.sumTotalBrand / sumAllStoreCompare) * 100, 2);
                        worksheet.Cell(rowData + 3, columnBrandTotalCompare).SetValue($"{percentShareBrand}%");

                        worksheet.Cell(rowData + 3, columnBrandTotalCompare).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 3, columnBrandTotalCompare + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 3, columnBrandTotalCompare).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(rowData + 3, columnBrandTotalCompare + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        if (brandCompare != null)
                        {
                            var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumTotalBrand > 0 ? ((brandDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
                            worksheet.Cell(rowData + 3, columnBrandTotalCompare + 1).SetValue($"{percentGrowthBrand}%");
                        }
                        else
                        {
                            worksheet.Cell(rowData + 3, columnBrandTotalCompare + 1).SetValue($"-100%");
                        }

                        worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Merge();
                        worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).SetValue(brandDetail.brandName);
                        columnBrandTotalCompare = columnBrandTotalCompare + 2;
                    }
                    else
                    {
                        worksheet.Range(worksheet.Cell(rowData+2, columnBrandTotalCompare), worksheet.Cell(rowData + 3, columnBrandTotal + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(worksheet.Cell(rowData+2, columnBrandTotalCompare), worksheet.Cell(rowData + 3, columnBrandTotal + 1)).Merge();
                        columnBrandTotalCompare = columnBrandTotalCompare + 2;
                    }
                }

                columnBrandTotalCompare++;

                foreach (var itemBrandLoreal in brandLorealList)
                {
                    bool haveData = false;
                    for (int i = 0; i < groupBrandDetailCompare.Count(); i++)
                    {
                        var brandNotTopDetail = groupBrandDetailCompare[i];
                        if (brandNotTopDetail.brandName == itemBrandLoreal
                            && !brandTotalSelect.Contains(brandNotTopDetail.brandName))
                        {
                            try
                            {
                                Color colorBrand = System.Drawing.ColorTranslator.FromHtml(brandNotTopDetail.Report_Color);
                                XLColor colorBrandXL = XLColor.FromArgb(colorBrand.A, colorBrand.R, colorBrand.G, colorBrand.B);
                                worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Style.Fill.BackgroundColor = colorBrandXL;
                            }
                            catch (Exception ex)
                            {

                            }

                            haveData = true;
                            worksheet.Column(columnBrandTotalCompare).Width = 11;
                            worksheet.Column(columnBrandTotalCompare + 1).Width = 11;

                            worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Value = $"{itemBrandLoreal} [#{i + 1}]";
                            worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Merge();
                            worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            var percentShareBrand = Math.Round((brandNotTopDetail.sumTotalBrand / sumAllStoreCompare) * 100, 2);
                            worksheet.Cell(rowData + 3, columnBrandTotalCompare).SetValue($"{percentShareBrand}%");
                            worksheet.Cell(rowData + 3, columnBrandTotalCompare).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(rowData + 3, columnBrandTotalCompare + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(rowData + 3, columnBrandTotalCompare).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            worksheet.Cell(rowData + 3, columnBrandTotalCompare + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            var brandCompare = groupBrandDetailOldCompare.FirstOrDefault(c => c.brandID == brandNotTopDetail.brandID);

                            if (brandCompare != null)
                            {
                                var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumTotalBrand > 0 ? ((brandNotTopDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
                                worksheet.Cell(rowData + 3, columnBrandTotalCompare + 1).SetValue($"{percentGrowthBrand}%");
                            }
                            else
                            {
                                worksheet.Cell(rowData + 3, columnBrandTotalCompare + 1).SetValue($"-100%");
                            }

                            break;
                        }
                    }

                    if (!haveData)
                    {
                        worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 3, columnBrandTotalCompare + 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 3, columnBrandTotalCompare + 1)).Merge();
                    }

                    columnBrandTotalCompare = columnBrandTotalCompare + 2;
                }
                #endregion

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string htmlBody = string.Empty;

                    if (request.preview)
                    {
                        Workbook workbookC = new Workbook();
                        workbookC.LoadFromStream(stream);
                        Worksheet sheet = workbookC.Worksheets[0];

                        string fileSave = $"Report5{Guid.NewGuid()}";

                        sheet.SaveToHtml(fileSave);

                        string excelHtmlPath = Path.GetFullPath(Path.Combine(fileSave));
                        using (StreamReader reader = File.OpenText(excelHtmlPath))
                        {
                            htmlBody = reader.ReadToEnd();
                        }

                        var regex = new Regex(@"<[hH][2][^>]*>[^<]*</[hH][2]\s*>", RegexOptions.Compiled | RegexOptions.Multiline);
                        htmlBody = regex.Replace(htmlBody, "");

                        File.Delete(excelHtmlPath);
                    }

                    return (content, htmlBody);
                }
            }
        }

        private (byte[], string) GenerateReportStoreMarketShareValue(ReportStoreMarketShareRequest request, List<GroupStoreRanking> listGroup, List<GroupStoreRanking> listGroupCompare, List<GroupStoreRanking> listGroupOldCompare)
        {
            using (var workbook = new XLWorkbook())
            {
                Color yellowHead = Color.FromArgb(250, 250, 181);
                Color greenHead = Color.FromArgb(184, 246, 184);
                Color storeHead = Color.FromArgb(199, 188, 222);
                Color countStoreColor = Color.FromArgb(250, 216, 213);
                Color totalHead = Color.FromArgb(239, 82, 227);
                Color black = Color.FromArgb(0, 0, 0);
                Color lorealHead = Color.FromArgb(229, 132, 121);
                Color whiteColor = Color.FromArgb(254, 254, 254);
                Color percentTotalColor = Color.FromArgb(200, 213, 119);

                XLColor yellowXL = XLColor.FromArgb(yellowHead.A, yellowHead.R, yellowHead.G, yellowHead.B);
                XLColor greenXL = XLColor.FromArgb(greenHead.A, greenHead.R, greenHead.G, greenHead.B);
                XLColor storeXL = XLColor.FromArgb(storeHead.A, storeHead.R, storeHead.G, storeHead.B);
                XLColor totalXL = XLColor.FromArgb(totalHead.A, totalHead.R, totalHead.G, totalHead.B);
                XLColor blackXL = XLColor.FromArgb(black.A, black.R, black.G, black.B);
                XLColor lorealXL = XLColor.FromArgb(lorealHead.A, lorealHead.R, lorealHead.G, lorealHead.B);
                XLColor countStorelXL = XLColor.FromArgb(countStoreColor.A, countStoreColor.R, countStoreColor.G, countStoreColor.B);
                XLColor whiteXL = XLColor.FromArgb(whiteColor.A, whiteColor.R, whiteColor.G, whiteColor.B);
                XLColor percentTotalColorXL = XLColor.FromArgb(percentTotalColor.A, percentTotalColor.R, percentTotalColor.G, percentTotalColor.B);

                #region Header
                var worksheet = workbook.Worksheets.Add("StoreMarketShareValue");
                int columnBrand = 3;

                if (!request.brandRankEnd.HasValue)
                {
                    var allBrandActive = repository.masterData.GetBrandListBy(c => c.Brand_ID != null);
                    request.brandRankEnd = allBrandActive.Count();
                }

                for (int i = 1; i <= request.brandRankEnd; i++)
                {
                    worksheet.Column(columnBrand).Width = 19;
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Merge();
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).SetValue($"#{i}");
                    columnBrand++;
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Merge();
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).SetValue("%");
                    columnBrand++;
                }

                columnBrand++;

                // Loreal Brand
                List<string> brandLorealList = new List<string>();
                var brandLorealListCurrent = listGroup.SelectMany(c => c.brandDetail.Where(e => e.Is_Loreal_Brand)).GroupBy(d => d.Brand_Name).Select(x => x.Key).OrderBy(d => d).ToList();
                var brandLorealListCompare = listGroupCompare.SelectMany(c => c.brandDetail.Where(e => e.Is_Loreal_Brand)).GroupBy(d => d.Brand_Name).Select(x => x.Key).OrderBy(d => d).ToList();
                var brandLorealListOldCompare = listGroupOldCompare.SelectMany(c => c.brandDetail.Where(e => e.Is_Loreal_Brand)).GroupBy(d => d.Brand_Name).Select(x => x.Key).OrderBy(d => d).ToList();

                brandLorealList.AddRange(brandLorealListCurrent);
                brandLorealList.AddRange(brandLorealListCompare);
                brandLorealList.AddRange(brandLorealListOldCompare);

                brandLorealList = brandLorealList.GroupBy(d => d).Select(x => x.Key).OrderBy(d => d).ToList();

                for (int i = 0; i < brandLorealList.Count(); i++)
                {
                    worksheet.Column(columnBrand).Width = 25;
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Style.Fill.BackgroundColor = lorealXL;

                    worksheet.Cell(5, columnBrand).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, columnBrand).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                    worksheet.Cell(6, columnBrand).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(6, columnBrand).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(6, columnBrand).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    worksheet.Cell(5, columnBrand).Value = $"{brandLorealList[i]}";
                    worksheet.Cell(6, columnBrand).Value = $"If Not In Top {request.brandRankEnd}";
                    worksheet.Cell(5, columnBrand).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(6, columnBrand).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    columnBrand++;

                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Merge();
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).SetValue("%");
                    columnBrand++;
                }

                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, columnBrand - 1)).Merge();
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, columnBrand - 1)).Value = $"Luxury Products - TOP {request.brandRankEnd}";
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, columnBrand - 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, columnBrand - 1)).Merge();
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, columnBrand - 1)).Value = "Brand Ranking Perfomance Key Counters";
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, columnBrand - 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, columnBrand - 1)).Merge();

                string dateRepport = string.Empty;
                int monthStart = int.Parse(request.startMonth);

                dateRepport = $"{monthList[monthStart - 1]}/{request.startYear}";
                dateRepport += request.endMonth != null ? $" - {monthList[int.Parse(request.endMonth) - 1]}/{request.endYear}" : "";

                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, columnBrand - 1)).SetValue(Convert.ToString(dateRepport));
                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, columnBrand - 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Column(1).Width = 3;
                worksheet.Column(2).Width = 30;

                worksheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(6, 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(6, 1)).Merge();
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(6, 1)).Style.Fill.BackgroundColor = yellowXL;
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(6, 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(6, 2)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(6, 2)).Merge();
                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(6, 2)).Value = "DEPARTMENT STORES";

                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(6, 2)).Style.Fill.BackgroundColor = yellowXL;
                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(6, 2)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, columnBrand - 1)).Merge();
                worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, columnBrand - 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, columnBrand - 1)).Style.Fill.BackgroundColor = greenXL;
                #endregion

                int rowData = 7;
                int countStore = 1;
                decimal sumAllStore = listGroup.Sum(c => c.sumStore);
                decimal sumAllStoreCompare = listGroupCompare.Sum(c => c.sumStore);

                foreach (var itemGroup in listGroup)
                {
                    var storeCompare = listGroupCompare.FirstOrDefault(c => c.storeID == itemGroup.storeID);

                    worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 2, 1)).Merge();
                    worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 2, 1)).Value = countStore;
                    worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 2, 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 2, 1)).Style.Fill.BackgroundColor = countStorelXL;

                    countStore++;

                    worksheet.Range(worksheet.Cell(rowData, 2), worksheet.Cell(rowData + 2, 2)).Style.Fill.BackgroundColor = storeXL;

                    worksheet.Cell(rowData, 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData + 1, 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData + 2, 2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData + 2, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(rowData, 2).SetValue(itemGroup.storeName);

                    worksheet.Cell(rowData + 1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(rowData + 1, 2).SetValue(string.Format("{0:#,0}", itemGroup.sumStore));

                    int columnBrandDetail = 3;
                    List<GroupBrandRanking> listBrandSelect = new List<GroupBrandRanking>();

                    if (!request.brandRankStart.HasValue)
                    {
                        request.brandRankStart = 1;
                    }

                    var groupBrandData = itemGroup.brandDetail.GroupBy(
                       x => new
                       {
                           x.Brand_ID,
                           x.Brand_Name
                       })
                       .Select(e => new GroupBrandRanking
                       {
                           brandID = e.Key.Brand_ID,
                           brandName = e.Key.Brand_Name,
                           color = e.FirstOrDefault().Report_Color,
                           sumBrand = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                        : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                        : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                           storeDetail = e.ToList()
                       }).OrderByDescending(s => s.sumBrand).ToList();

                    for (int i = request.brandRankStart.GetValueOrDefault() - 1; i <= request.brandRankEnd.GetValueOrDefault() - 1; i++)
                    {
                        if (groupBrandData.ElementAtOrDefault(i) != null)
                        {
                            listBrandSelect.Add(groupBrandData.ElementAtOrDefault(i));
                        }
                    }

                    int countBrandRender = 0;
                    foreach (var itemBrand in listBrandSelect)
                    {
                        worksheet.Cell(rowData, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(rowData, columnBrandDetail).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData, columnBrandDetail).SetValue(itemBrand.brandName);

                        worksheet.Cell(rowData + 1, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(rowData + 1, columnBrandDetail).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                        decimal percentShareBrand = 0;
                        worksheet.Cell(rowData + 1, columnBrandDetail).SetValue(string.Format("{0:#,0}", itemBrand.sumBrand));
                        percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.sumBrand / itemGroup.sumStore) * 100, 2) : 0;

                        //if (request.saleType == "Amount")
                        //{
                        //    worksheet.Cell(rowData + 1, columnBrandDetail).SetValue(string.Format("{0:#,0}", itemBrand.sumBrand));
                        //    percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.sumBrand / itemGroup.sumStore) * 100, 2) : 0;
                        //}
                        //else if (request.saleType == "Whole")
                        //{
                        //    worksheet.Cell(rowData + 1, columnBrandDetail).SetValue(string.Format("{0:#,0}", itemBrand.Whole_Sales));
                        //    percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Whole_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        //}
                        //else if (request.saleType == "Net")
                        //{
                        //    worksheet.Cell(rowData + 1, columnBrandDetail).SetValue(string.Format("{0:#,0}", itemBrand.Net_Sales));
                        //    percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Net_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        //}

                        worksheet.Cell(rowData + 2, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(rowData + 2, columnBrandDetail).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 2, columnBrandDetail).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 2, columnBrandDetail).SetValue($"{percentShareBrand}%");

                        worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 2, columnBrandDetail)).Style.Fill.BackgroundColor = whiteXL;

                        try
                        {
                            Color colorBrand = System.Drawing.ColorTranslator.FromHtml(itemBrand.color);
                            XLColor colorBrandXL = XLColor.FromArgb(colorBrand.A, colorBrand.R, colorBrand.G, colorBrand.B);

                            worksheet.Cell(rowData, columnBrandDetail).Style.Fill.BackgroundColor = colorBrandXL;
                        }
                        catch (Exception ex)
                        {

                        }

                        columnBrandDetail++;

                        if (storeCompare != null)
                        {
                            var brandCompare = storeCompare.brandDetail.FirstOrDefault(c => c.Brand_ID == itemBrand.brandID);
                            decimal percentGrowthBrand = -100;

                            percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Amount_Sales.GetValueOrDefault() != 0
                                   ? ((itemBrand.sumBrand / brandCompare.Amount_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);

                            //if (request.saleType == "Amount")
                            //{
                            //    percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Amount_Sales.GetValueOrDefault() != 0
                            //        ? ((itemBrand.Amount_Sales.GetValueOrDefault() / brandCompare.Amount_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            //}
                            //else if (request.saleType == "Whole")
                            //{
                            //    percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Whole_Sales.GetValueOrDefault() != 0
                            //         ? ((itemBrand.Whole_Sales.GetValueOrDefault() / brandCompare.Whole_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            //}
                            //else if (request.saleType == "Net")
                            //{
                            //    percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Net_Sales.GetValueOrDefault() != 0
                            //        ? ((itemBrand.Net_Sales.GetValueOrDefault() / brandCompare.Net_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            //}

                            worksheet.Cell(rowData, columnBrandDetail).SetValue($"{percentGrowthBrand}%");
                        }
                        else
                        {
                            worksheet.Cell(rowData, columnBrandDetail).SetValue($"-100%");
                        }

                        worksheet.Cell(rowData, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(rowData, columnBrandDetail).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 1, columnBrandDetail).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 2, columnBrandDetail).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 2, columnBrandDetail).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 2, columnBrandDetail)).Style.Fill.BackgroundColor = whiteXL;
                        countBrandRender++;
                        columnBrandDetail++;
                    }

                    if (countBrandRender < request.brandRankEnd.GetValueOrDefault())
                    {
                        for (int i = countBrandRender; i < request.brandRankEnd.GetValueOrDefault(); i++)
                        {
                            worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 2, columnBrandDetail)).Merge();
                            worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 2, columnBrandDetail)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            columnBrandDetail++;
                            worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 2, columnBrandDetail)).Merge();
                            worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 2, columnBrandDetail)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            columnBrandDetail++;
                            countBrandRender++;
                        }
                    }

                    var listBrandTopSelectName = listBrandSelect.Select(c => c.brandName);
                    columnBrandDetail++;

                    foreach (var itemBrandLoreal in brandLorealList)
                    {
                        bool haveData = false;
                        for (int i = 0; i < itemGroup.brandDetail.Count(); i++)
                        {
                            var brandNotTopDetail = itemGroup.brandDetail[i];
                            if (brandNotTopDetail.Brand_Name == itemBrandLoreal
                                && !listBrandTopSelectName.Contains(brandNotTopDetail.Brand_Name))
                            {
                                worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 2, columnBrandDetail)).Style.Fill.BackgroundColor = whiteXL;

                                try
                                {
                                    Color colorBrand = System.Drawing.ColorTranslator.FromHtml(brandNotTopDetail.Report_Color);
                                    XLColor colorBrandXL = XLColor.FromArgb(colorBrand.A, colorBrand.R, colorBrand.G, colorBrand.B);
                                    worksheet.Cell(rowData, columnBrandDetail).Style.Fill.BackgroundColor = colorBrandXL;
                                }
                                catch (Exception ex)
                                {

                                }

                                haveData = true;
                                worksheet.Cell(rowData, columnBrandDetail).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                worksheet.Cell(rowData + 1, columnBrandDetail).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                worksheet.Cell(rowData, columnBrandDetail).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                worksheet.Cell(rowData + 1, columnBrandDetail).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

                                worksheet.Cell(rowData, columnBrandDetail).SetValue($"{itemBrandLoreal} [#{i + 1}]");
                                worksheet.Cell(rowData, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                worksheet.Cell(rowData + 1, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                decimal percentShareBrand = 0;
                                if (request.saleType == "Amount")
                                {
                                    worksheet.Cell(rowData + 1, columnBrandDetail).SetValue(string.Format("{0:#,0}", brandNotTopDetail.Amount_Sales));
                                    percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((brandNotTopDetail.Amount_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                                }
                                else if (request.saleType == "Whole")
                                {
                                    worksheet.Cell(rowData + 1, columnBrandDetail).SetValue(string.Format("{0:#,0}", brandNotTopDetail.Whole_Sales));
                                    percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((brandNotTopDetail.Whole_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                                }
                                else if (request.saleType == "Net")
                                {
                                    worksheet.Cell(rowData + 1, columnBrandDetail).SetValue(string.Format("{0:#,0}", brandNotTopDetail.Net_Sales));
                                    percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((brandNotTopDetail.Net_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                                }

                                worksheet.Cell(rowData + 2, columnBrandDetail).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                worksheet.Cell(rowData + 2, columnBrandDetail).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                worksheet.Cell(rowData + 2, columnBrandDetail).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                                worksheet.Cell(rowData + 2, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                worksheet.Cell(rowData + 2, columnBrandDetail).SetValue($"{percentShareBrand}%");

                                columnBrandDetail++;

                                worksheet.Cell(rowData, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                if (storeCompare != null)
                                {
                                    var brandCompare = storeCompare.brandDetail.FirstOrDefault(c => c.Brand_ID == brandNotTopDetail.Brand_ID);
                                    decimal percentGrowthBrand = Math.Round(-100M, 2);
                                    if (request.saleType == "Amount")
                                    {
                                        percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Amount_Sales.GetValueOrDefault() != 0 ? ((brandNotTopDetail.Amount_Sales.GetValueOrDefault() / brandCompare.Amount_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                                    }
                                    else if (request.saleType == "Whole")
                                    {
                                        percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Whole_Sales.GetValueOrDefault() != 0 ? ((brandNotTopDetail.Whole_Sales.GetValueOrDefault() / brandCompare.Whole_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                                    }
                                    else if (request.saleType == "Net")
                                    {
                                        percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Net_Sales.GetValueOrDefault() != 0 ? ((brandNotTopDetail.Net_Sales.GetValueOrDefault() / brandCompare.Net_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                                    }

                                    worksheet.Cell(rowData, columnBrandDetail).SetValue($"{percentGrowthBrand}%");
                                }
                                else
                                {
                                    worksheet.Cell(rowData, columnBrandDetail).SetValue($"-100%");
                                }

                                worksheet.Cell(rowData, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                worksheet.Cell(rowData, columnBrandDetail).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                worksheet.Cell(rowData + 1, columnBrandDetail).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                worksheet.Cell(rowData + 2, columnBrandDetail).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                worksheet.Cell(rowData + 2, columnBrandDetail).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 2, columnBrandDetail)).Style.Fill.BackgroundColor = whiteXL;

                                columnBrandDetail++;
                                break;
                            }
                        }

                        if (!haveData)
                        {
                            worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 2, columnBrandDetail)).Merge();
                            worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 2, columnBrandDetail)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            columnBrandDetail++;
                            worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 2, columnBrandDetail)).Merge();
                            worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData + 2, columnBrandDetail)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            columnBrandDetail++;
                        }

                    }

                    rowData = rowData + 3;
                }

                var percentGrowthTotal = Math.Round(((sumAllStore / sumAllStoreCompare) - 1) * 100, 2);
                var allBrandDetail = listGroup.SelectMany(c => c.brandDetail);
                var allBrandDetailCompare = listGroupCompare.SelectMany(c => c.brandDetail);
                var allBrandDetailOldCompare = listGroupOldCompare.SelectMany(c => c.brandDetail);

                #region Group Total
                var groupBrandDetail = allBrandDetail.GroupBy(
                       x => new
                       {
                           x.Brand_ID,
                           x.Brand_Name
                       })
                       .Select(e => new
                       {
                           brandID = e.Key.Brand_ID,
                           brandName = e.Key.Brand_Name,
                           sumTotalBrand = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                        : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                        : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                           detail = e.ToList()
                       }).OrderByDescending(s => s.sumTotalBrand).ToList();

                var groupBrandDetailCompare = allBrandDetailCompare.GroupBy(
                     x => new
                     {
                         x.Brand_ID,
                         x.Brand_Name
                     })
                     .Select(e => new
                     {
                         brandID = e.Key.Brand_ID,
                         brandName = e.Key.Brand_Name,
                         sumTotalBrand = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                        : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                        : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                         detail = e.ToList()
                     }).OrderByDescending(s => s.sumTotalBrand).ToList();

                var groupBrandDetailOldCompare = allBrandDetailOldCompare.GroupBy(
                    x => new
                    {
                        x.Brand_ID,
                        x.Brand_Name
                    })
                    .Select(e => new
                    {
                        brandID = e.Key.Brand_ID,
                        brandName = e.Key.Brand_Name,
                        sumTotalBrand = request.saleType == "Amount" ? e.Sum(d => d.Amount_Sales.GetValueOrDefault())
                        : request.saleType == "Whole" ? e.Sum(d => d.Whole_Sales.GetValueOrDefault())
                        : request.saleType == "Net" ? e.Sum(d => d.Net_Sales.GetValueOrDefault()) : 0,
                        detail = e.ToList()
                    }).OrderByDescending(s => s.sumTotalBrand).ToList();


                rowData++;
                #endregion

                #region Total 1
                worksheet.Cell(rowData, 2).Value = "Year";
                worksheet.Cell(rowData, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(rowData, 2).Style.Fill.BackgroundColor = storeXL;
                worksheet.Cell(rowData, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowData, 1).Style.Fill.BackgroundColor = storeXL;
                worksheet.Cell(rowData, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowData + 1, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowData + 2, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(rowData + 1, 2).Value = $"TOTAL {request.startYear}";
                worksheet.Cell(rowData + 1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(rowData + 1, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(rowData + 2, 2).Value = $"TOTAL {request.compareYear}";
                worksheet.Cell(rowData + 2, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(rowData + 2, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                int columnBrandTotal = 3;
                List<string> brandTotalSelect = new List<string>();

                for (int i = 0; i < request.brandRankEnd; i++)
                {
                    if (i < groupBrandDetail.Count())
                    {
                        var brandDetail = groupBrandDetail[i];
                        brandTotalSelect.Add(brandDetail.brandName);

                        worksheet.Cell(rowData, columnBrandTotal).Value = brandDetail.brandName;
                        worksheet.Cell(rowData, columnBrandTotal).Style.Fill.BackgroundColor = storeXL;

                        worksheet.Cell(rowData, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(rowData, columnBrandTotal + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(rowData, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        var brandCompare = groupBrandDetailCompare.FirstOrDefault(c => c.brandID == brandDetail.brandID);

                        worksheet.Cell(rowData + 1, columnBrandTotal).SetValue(string.Format("{0:#,0}", brandDetail.sumTotalBrand));
                        worksheet.Cell(rowData + 1, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(rowData + 1, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        if (brandCompare != null)
                        {
                            worksheet.Cell(rowData + 2, columnBrandTotal).SetValue(string.Format("{0:#,0}", brandCompare.sumTotalBrand));
                            worksheet.Cell(rowData + 2, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            worksheet.Cell(rowData + 2, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumTotalBrand > 0 ? ((brandDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
                            worksheet.Cell(rowData, columnBrandTotal + 1).SetValue($"{percentGrowthBrand}%");
                        }
                        else
                        {
                            worksheet.Cell(rowData, columnBrandTotal + 1).SetValue($"-100%");
                        }

                        worksheet.Cell(rowData, columnBrandTotal + 1).Style.Fill.BackgroundColor = percentTotalColorXL;
                        worksheet.Cell(rowData, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 1, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 2, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        columnBrandTotal = columnBrandTotal + 2;
                    }
                    else
                    {
                        worksheet.Cell(rowData, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 1, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 2, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 1, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 2, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        columnBrandTotal = columnBrandTotal + 2;
                    }

                }

                columnBrandTotal++;

                foreach (var itemBrandLoreal in brandLorealList)
                {
                    bool haveData = false;

                    for (int i = 0; i < groupBrandDetail.Count(); i++)
                    {
                        var brandNotTopDetail = groupBrandDetail[i];
                        if (brandNotTopDetail.brandName == itemBrandLoreal
                            && !brandTotalSelect.Contains(brandNotTopDetail.brandName))
                        {
                            haveData = true;

                            worksheet.Cell(rowData, columnBrandTotal).Value = $"{itemBrandLoreal} [#{i + 1}]";
                            worksheet.Cell(rowData, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Cell(rowData, columnBrandTotal + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Cell(rowData, columnBrandTotal).Style.Fill.BackgroundColor = storeXL;
                            worksheet.Cell(rowData, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            var brandCompare = groupBrandDetailCompare.FirstOrDefault(c => c.brandID == brandNotTopDetail.brandID);

                            worksheet.Cell(rowData + 1, columnBrandTotal).SetValue(string.Format("{0:#,0}", brandNotTopDetail.sumTotalBrand));
                            worksheet.Cell(rowData + 1, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            worksheet.Cell(rowData + 1, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            if (brandCompare != null)
                            {
                                var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumTotalBrand > 0 ? ((brandNotTopDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
                                worksheet.Cell(rowData, columnBrandTotal + 1).SetValue($"{percentGrowthBrand}%");

                                worksheet.Cell(rowData + 2, columnBrandTotal).SetValue(string.Format("{0:#,0}", brandCompare.sumTotalBrand));
                                worksheet.Cell(rowData + 2, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                worksheet.Cell(rowData + 2, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                            else
                            {
                                worksheet.Cell(rowData, columnBrandTotal + 1).SetValue($"-100%");
                            }

                            worksheet.Cell(rowData, columnBrandTotal + 1).Style.Fill.BackgroundColor = percentTotalColorXL;
                            worksheet.Cell(rowData, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(rowData + 1, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(rowData + 2, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            break;
                        }
                    }

                    if (!haveData)
                    {
                        worksheet.Cell(rowData, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 1, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 2, columnBrandTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        worksheet.Cell(rowData, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 1, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowData + 2, columnBrandTotal + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    columnBrandTotal = columnBrandTotal + 2;
                }
                #endregion

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string htmlBody = string.Empty;

                    if (request.preview)
                    {
                        Workbook workbookC = new Workbook();
                        workbookC.LoadFromStream(stream);
                        Worksheet sheet = workbookC.Worksheets[0];

                        string fileSave = $"Report2{Guid.NewGuid()}";

                        sheet.SaveToHtml(fileSave);

                        string excelHtmlPath = Path.GetFullPath(Path.Combine(fileSave));
                        using (StreamReader reader = File.OpenText(excelHtmlPath))
                        {
                            htmlBody = reader.ReadToEnd();
                        }

                        var regex = new Regex(@"<[hH][2][^>]*>[^<]*</[hH][2]\s*>", RegexOptions.Compiled | RegexOptions.Multiline);
                        htmlBody = regex.Replace(htmlBody, "");

                        File.Delete(excelHtmlPath);
                    }

                    return (content, htmlBody);
                }
            }
        }

        private (byte[], string) GenerateReportSelectiveMarket(ReportSelectiveMarketRequest request, List<GroupBrandRanking> listGroup, List<GroupBrandRanking> listGroupCompare)
        {
            using (var workbook = new XLWorkbook())
            {
                var brandFragrances = repository.report.GetBrandFragances();
                Color yellowHead = Color.FromArgb(250, 250, 181);
                Color yellowHead2 = Color.FromArgb(250, 243, 27);
                Color whiteColor = Color.FromArgb(254, 254, 254);
                Color blueColor = Color.FromArgb(109, 192, 218);
                Color greenColor = Color.FromArgb(26, 244, 31);
                Color browColor = Color.FromArgb(236, 204, 185);
                Color redColor = Color.FromArgb(254, 89, 89);
                Color blackColor = Color.FromArgb(0, 0, 0);

                XLColor yellowXL = XLColor.FromArgb(yellowHead.A, yellowHead.R, yellowHead.G, yellowHead.B);
                XLColor yellowXL2 = XLColor.FromArgb(yellowHead2.A, yellowHead2.R, yellowHead2.G, yellowHead2.B);
                XLColor whiteXL = XLColor.FromArgb(whiteColor.A, whiteColor.R, whiteColor.G, whiteColor.B);
                XLColor blueXL = XLColor.FromArgb(blueColor.A, blueColor.R, blueColor.G, blueColor.B);
                XLColor greenXL = XLColor.FromArgb(greenColor.A, greenColor.R, greenColor.G, greenColor.B);
                XLColor browXL = XLColor.FromArgb(browColor.A, browColor.R, browColor.G, browColor.B);
                XLColor redXL = XLColor.FromArgb(redColor.A, redColor.R, redColor.G, redColor.B);
                XLColor blackXL = XLColor.FromArgb(blackColor.A, blackColor.R, blackColor.G, blackColor.B);

                #region Header
                var worksheet = workbook.Worksheets.Add("SelectiveMarket");
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 10)).Merge();
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 10)).Value = $"SELECTIVE MARKET THAILAND";
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 10)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, 10)).Merge();

                string dateRepport = string.Empty;
                int monthStart = int.Parse(request.startMonth);
                dateRepport = $"{monthList[monthStart - 1]}/{request.startYear}";
                dateRepport += request.endMonth != null ? $" - {monthList[int.Parse(request.endMonth) - 1]}/{request.endYear}" : "";

                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, 10)).SetValue(Convert.ToString(dateRepport)); ///
                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, 10)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Column(1).Width = 3;
                worksheet.Column(2).Width = 19;

                worksheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(4, 2)).Merge();
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(4, 2)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(4, 2)).Style.Fill.BackgroundColor = whiteXL;

                worksheet.Cell(5, 1).Value = "#";
                worksheet.Cell(5, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(5, 1).Style.Fill.BackgroundColor = blueXL;

                worksheet.Cell(5, 2).Value = "Brand";
                worksheet.Cell(5, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(5, 2).Style.Fill.BackgroundColor = blueXL;
                worksheet.Row(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                string typeReport = string.Empty;
                string periodTimeCompare = $"w{request.startWeek},{request.startMonth}/{request.compareYear}";
                periodTimeCompare += !string.IsNullOrWhiteSpace(request.endWeek) ? $" - w{request.endWeek},{request.endMonth}/{request.compareYear}" : "";

                string periodTime = $"w{request.startWeek},{request.startMonth}/{request.startYear}";
                periodTime += !string.IsNullOrWhiteSpace(request.endWeek) ? $" - w{request.endWeek},{request.endMonth}/{request.endYear}" : "";

                int totalColumn = 1;
                if (request.saleType == "Amount")
                {
                    typeReport = "TOTAL MARKET";
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 5)).Merge();
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 5)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 5)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 5)).Style.Fill.BackgroundColor = yellowXL;
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 5)).SetValue(Convert.ToString(periodTimeCompare));

                    worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(4, 10)).Merge();
                    worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(4, 10)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(4, 10)).Style.Fill.BackgroundColor = yellowXL2;
                    worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(4, 10)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(4, 10)).SetValue(Convert.ToString(periodTime));

                    worksheet.Cell(5, 3).Value = "Value";
                    worksheet.Cell(5, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 3).Style.Fill.BackgroundColor = greenXL;

                    worksheet.Cell(5, 4).Value = "Market Share";
                    worksheet.Cell(5, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 4).Style.Fill.BackgroundColor = browXL;

                    worksheet.Cell(5, 5).Value = "# POS";
                    worksheet.Cell(5, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 5).Style.Fill.BackgroundColor = browXL;

                    worksheet.Cell(5, 6).Value = "Value";
                    worksheet.Cell(5, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 6).Style.Fill.BackgroundColor = greenXL;

                    worksheet.Cell(5, 7).Value = "Market Share";
                    worksheet.Cell(5, 8).Value = "Growth";
                    worksheet.Cell(5, 9).Value = "MS Evol";
                    worksheet.Cell(5, 10).Value = "# POS";

                    worksheet.Cell(5, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 7).Style.Fill.BackgroundColor = browXL;

                    worksheet.Cell(5, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 8).Style.Fill.BackgroundColor = browXL;

                    worksheet.Cell(5, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 9).Style.Fill.BackgroundColor = browXL;

                    worksheet.Cell(5, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 10).Style.Fill.BackgroundColor = browXL;

                    worksheet.Column(3).Width = 19;
                    worksheet.Column(6).Width = 19;
                    worksheet.Column(4).Width = 19;
                    worksheet.Column(7).Width = 19;
                    worksheet.Column(8).Width = 10;
                    worksheet.Column(9).Width = 10;

                    totalColumn = 10;
                }
                else if (request.saleType == "Whole")
                {
                    typeReport = "TOTAL MARKET - BULK SALES";
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 5)).Merge();
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 5)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 5)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 5)).Style.Fill.BackgroundColor = yellowXL;
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 5)).SetValue(Convert.ToString(periodTimeCompare));

                    worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(4, 9)).Merge();
                    worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(4, 9)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(4, 9)).Style.Fill.BackgroundColor = yellowXL2;
                    worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(4, 9)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(4, 9)).SetValue(Convert.ToString(periodTime));

                    worksheet.Cell(5, 3).Value = "Total Sale";
                    worksheet.Cell(5, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 3).Style.Fill.BackgroundColor = greenXL;

                    worksheet.Cell(5, 4).Value = "Bulk Sale";
                    worksheet.Cell(5, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 4).Style.Fill.BackgroundColor = greenXL;

                    worksheet.Cell(5, 5).Value = "Bulk Share";
                    worksheet.Cell(5, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 5).Style.Fill.BackgroundColor = browXL;

                    worksheet.Cell(5, 6).Value = "Total Sale";
                    worksheet.Cell(5, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 6).Style.Fill.BackgroundColor = greenXL;

                    worksheet.Cell(5, 7).Value = "Bulk Sale";
                    worksheet.Cell(5, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 7).Style.Fill.BackgroundColor = greenXL;

                    worksheet.Cell(5, 8).Value = "Bulk Share";
                    worksheet.Cell(5, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 8).Style.Fill.BackgroundColor = browXL;

                    worksheet.Cell(5, 9).Value = "Bulk Growth";
                    worksheet.Cell(5, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 9).Style.Fill.BackgroundColor = browXL;

                    worksheet.Column(3).Width = 19;
                    worksheet.Column(4).Width = 19;
                    worksheet.Column(5).Width = 19;
                    worksheet.Column(6).Width = 19;
                    worksheet.Column(7).Width = 19;
                    worksheet.Column(8).Width = 10;
                    worksheet.Column(9).Width = 10;

                    totalColumn = 9;
                }
                else if (request.saleType == "Net")
                {
                    typeReport = "TOTAL MARKET WO BULK";
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 4)).Merge();
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 4)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 4)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 4)).Style.Fill.BackgroundColor = yellowXL;
                    worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 4)).SetValue(Convert.ToString(periodTimeCompare));

                    worksheet.Range(worksheet.Cell(4, 5), worksheet.Cell(4, 8)).Merge();
                    worksheet.Range(worksheet.Cell(4, 5), worksheet.Cell(4, 8)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(worksheet.Cell(4, 5), worksheet.Cell(4, 8)).Style.Fill.BackgroundColor = yellowXL2;
                    worksheet.Range(worksheet.Cell(4, 5), worksheet.Cell(4, 8)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(4, 5), worksheet.Cell(4, 8)).SetValue(Convert.ToString(periodTime));

                    worksheet.Cell(5, 3).Value = "Value";
                    worksheet.Cell(5, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 3).Style.Fill.BackgroundColor = greenXL;

                    worksheet.Cell(5, 4).Value = "Market Share";
                    worksheet.Cell(5, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 4).Style.Fill.BackgroundColor = browXL;

                    worksheet.Cell(5, 5).Value = "Value";
                    worksheet.Cell(5, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 5).Style.Fill.BackgroundColor = greenXL;

                    worksheet.Cell(5, 6).Value = "Market Share";
                    worksheet.Cell(5, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 6).Style.Fill.BackgroundColor = browXL;

                    worksheet.Cell(5, 7).Value = "Growth";
                    worksheet.Cell(5, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 7).Style.Fill.BackgroundColor = browXL;

                    worksheet.Cell(5, 8).Value = "MS Evol";
                    worksheet.Cell(5, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, 8).Style.Fill.BackgroundColor = browXL;

                    totalColumn = 8;
                    worksheet.Column(3).Width = 19;
                    worksheet.Column(4).Width = 19;
                    worksheet.Column(5).Width = 19;
                    worksheet.Column(6).Width = 19;
                    worksheet.Column(7).Width = 10;
                    worksheet.Column(8).Width = 10;
                }

                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 10)).Merge();
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 10)).Value = typeReport;
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 10)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                #endregion

                int rowData = 6;
                int countBrand = 1;
                decimal sumTotalCurrent = listGroup.Sum(c => c.sumBrand);
                decimal sumTotalCompare = listGroupCompare.Sum(c => c.sumBrand);
                int totalStoreCompare = 0;
                int totalStore = 0;
                bool addFranganceCompare = false;
                bool addFrangance = false;

                foreach (var itemGroup in listGroup)
                {
                    try
                    {
                        Color colorBrand = System.Drawing.ColorTranslator.FromHtml(itemGroup.color);
                        XLColor colorBrandXL = XLColor.FromArgb(colorBrand.A, colorBrand.R, colorBrand.G, colorBrand.B);

                        worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData, totalColumn)).Style.Fill.BackgroundColor = colorBrandXL;
                    }
                    catch (Exception ex)
                    {

                    }

                    var brandCompare = listGroupCompare.FirstOrDefault(c => c.brandID == itemGroup.brandID);

                    worksheet.Row(rowData).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    worksheet.Cell(rowData, 1).Value = countBrand;
                    worksheet.Cell(rowData, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    worksheet.Cell(rowData, 2).Value = itemGroup.brandName;
                    worksheet.Cell(rowData, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                    for (int i = 3; i <= totalColumn; i++)
                    {
                        worksheet.Cell(rowData, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    if (request.saleType != "Whole")
                    {
                        if (itemGroup.sumBrand > 0 || (brandCompare != null && brandCompare.sumBrand > 0))
                        {
                            worksheet.Cell(rowData, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(rowData, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            worksheet.Cell(rowData, 3).SetValue(brandCompare != null ? string.Format("{0:#,0}", brandCompare.sumBrand) : "0");

                            var percentShareBrandCompare = brandCompare != null && brandCompare.sumBrand > 0 ? Math.Round((brandCompare.sumBrand / sumTotalCompare) * 100, 2) : 0;
                            worksheet.Cell(rowData, 4).SetValue($"{percentShareBrandCompare}%");
                            worksheet.Cell(rowData, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            // Check Brand Fragance
                            if (brandCompare != null)
                            {
                                if (brandFragrances.Select(e => e.Brand_ID).Contains(brandCompare.brandID))
                                {
                                    if (!addFranganceCompare)
                                    {
                                        addFranganceCompare = true;
                                        totalStoreCompare += brandCompare.storeDetail.GroupBy(c => c.Store_Id).Count();
                                    }
                                }
                                else
                                {
                                    totalStoreCompare += brandCompare.storeDetail.GroupBy(c => c.Store_Id).Count();
                                }
                            }

                            worksheet.Cell(rowData, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(rowData, 5).SetValue(brandCompare != null ? $"{ brandCompare.storeDetail.GroupBy(c => c.Store_Id).Count()}" : "0");

                            worksheet.Cell(rowData, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(rowData, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            worksheet.Cell(rowData, 6).SetValue(string.Format("{0:#,0}", itemGroup.sumBrand));

                            var percentShareBrand = itemGroup.sumBrand > 0 ? Math.Round((itemGroup.sumBrand / sumTotalCurrent) * 100, 2) : 0;
                            worksheet.Cell(rowData, 7).SetValue($"{percentShareBrandCompare}%");
                            worksheet.Cell(rowData, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumBrand > 0 ? ((itemGroup.sumBrand / brandCompare.sumBrand) - 1) * 100 : -100, 2);
                            worksheet.Cell(rowData, 8).SetValue($"{percentGrowthBrand}%");
                            worksheet.Cell(rowData, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            if (request.saleType == "Amount")
                            {
                                worksheet.Cell(rowData, 9).SetValue($"{percentShareBrand - percentShareBrandCompare}%");
                                worksheet.Cell(rowData, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                worksheet.Cell(rowData, 10).SetValue($"{itemGroup.storeDetail.GroupBy(c => c.Store_Id).Count()}");
                                worksheet.Cell(rowData, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }

                            if (brandFragrances.Select(e => e.Brand_ID).Contains(itemGroup.brandID))
                            {
                                if (!addFrangance)
                                {
                                    addFrangance = true;
                                    totalStore += itemGroup.storeDetail.GroupBy(c => c.Store_Id).Count();
                                }
                            }
                            else
                            {
                                totalStore += itemGroup.storeDetail.GroupBy(c => c.Store_Id).Count();
                            }
                        }
                    }
                    else
                    {
                        if (itemGroup.sumBrand > 0 || itemGroup.storeDetail.Sum(c => c.Amount_Sales) > 0
                            || (brandCompare != null && (brandCompare.sumBrand > 0 || brandCompare.storeDetail.Sum(c => c.Amount_Sales) > 0)))
                        {

                            var totalCompareBrandAmount = brandCompare != null ? brandCompare.storeDetail.Sum(c => c.Amount_Sales.GetValueOrDefault()) : 0;
                            var totalBrandAmount = brandCompare != null ? brandCompare.storeDetail.Sum(c => c.Amount_Sales.GetValueOrDefault()) : 0;

                            worksheet.Cell(rowData, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            worksheet.Cell(rowData, 3).SetValue(string.Format("{0:#,0}", totalCompareBrandAmount));

                            worksheet.Cell(rowData, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            worksheet.Cell(rowData, 4).SetValue(brandCompare != null ? string.Format("{0:#,0}", brandCompare.sumBrand) : "0");

                            var percentShareBrandCompare = brandCompare != null && brandCompare.sumBrand > 0 && totalCompareBrandAmount > 0 ? Math.Round((brandCompare.sumBrand / totalCompareBrandAmount) * 100, 2) : 0;
                            worksheet.Cell(rowData, 5).SetValue($"{percentShareBrandCompare}%");

                            worksheet.Cell(rowData, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            worksheet.Cell(rowData, 6).SetValue(brandCompare != null ? string.Format("{0:#,0}", itemGroup.storeDetail.Sum(c => c.Amount_Sales)) : "0");

                            worksheet.Cell(rowData, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            worksheet.Cell(rowData, 7).SetValue(brandCompare != null ? string.Format("{0:#,0}", itemGroup.sumBrand) : "0");

                            var percentShareBrand = itemGroup.sumBrand > 0 ? Math.Round((itemGroup.sumBrand / totalBrandAmount) * 100, 2) : 0;
                            worksheet.Cell(rowData, 8).SetValue($"{percentShareBrandCompare}%");

                            var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumBrand > 0 ? ((itemGroup.sumBrand / brandCompare.sumBrand) - 1) * 100 : -100, 2);
                            worksheet.Cell(rowData, 9).SetValue($"{percentGrowthBrand}%");
                        }
                    }
                    countBrand++;
                    rowData++;

                }

                worksheet.Row(rowData).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData, 2)).Merge();
                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData, 2)).Value = "TOTAL";

                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData, 2)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData, 2)).Style.Border.OutsideBorderColor = blackXL;
                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData, totalColumn)).Style.Fill.BackgroundColor = redXL;


                for (int i = 3; i <= totalColumn; i++)
                {
                    worksheet.Cell(rowData, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, i).Style.Border.OutsideBorderColor = blackXL;
                }

                if (request.saleType != "Whole")
                {
                    worksheet.Cell(rowData, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(rowData, 3).SetValue(string.Format("{0:#,0}", sumTotalCompare));
                    worksheet.Cell(rowData, 4).SetValue($"100%");
                    worksheet.Cell(rowData, 5).SetValue(string.Format("{0:#,0}", totalStoreCompare));

                    worksheet.Cell(rowData, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(rowData, 6).SetValue(string.Format("{0:#,0}", sumTotalCurrent));
                    worksheet.Cell(rowData, 7).SetValue($"100%");

                    var percentGrowthTotal = Math.Round(sumTotalCompare > 0 ? ((sumTotalCurrent / sumTotalCompare) - 1) * 100 : -100, 2);
                    worksheet.Cell(rowData, 8).SetValue($"{percentGrowthTotal}%");

                    if (request.saleType == "Amount")
                    {
                        worksheet.Cell(rowData, 9).SetValue($"0.00%");
                        worksheet.Cell(rowData, 10).SetValue(string.Format("{0:#,0}", totalStore));
                    }
                }
                else
                {
                    var totalAmountSaleCompare = listGroupCompare.SelectMany(c => c.storeDetail.Where(e => e.Amount_Sales > 0)).Sum(d => d.Amount_Sales.GetValueOrDefault());
                    var totalAmountSale = listGroup.SelectMany(c => c.storeDetail.Where(e => e.Amount_Sales > 0)).Sum(d => d.Amount_Sales.GetValueOrDefault());

                    worksheet.Cell(rowData, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(rowData, 3).SetValue(string.Format("{0:#,0}", totalAmountSaleCompare));
                    worksheet.Cell(rowData, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(rowData, 4).SetValue(string.Format("{0:#,0}", sumTotalCompare));

                    var percentShareTotalCompare = totalAmountSaleCompare > 0 ? Math.Round(sumTotalCompare / totalAmountSaleCompare * 100, 2) : 0;
                    worksheet.Cell(rowData, 5).SetValue($"{percentShareTotalCompare}%");

                    worksheet.Cell(rowData, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(rowData, 6).SetValue(string.Format("{0:#,0}", totalAmountSale));
                    worksheet.Cell(rowData, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(rowData, 7).SetValue(string.Format("{0:#,0}", sumTotalCurrent));
                    var percentShareTotal = sumTotalCurrent > 0 ? Math.Round(sumTotalCurrent / totalAmountSale * 100, 2) : 0;
                    worksheet.Cell(rowData, 8).SetValue($"{percentShareTotal}%");

                    var percentGrowthTotal = Math.Round(sumTotalCompare > 0 ? ((sumTotalCurrent / sumTotalCompare) - 1) * 100 : -100, 2);
                    worksheet.Cell(rowData, 9).SetValue($"{percentGrowthTotal}%");
                }

                if (!request.preview)
                {
                    worksheet.Row(rowData).Style.Font.SetFontColor(whiteXL);
                }


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string htmlBody = string.Empty;

                    if (request.preview)
                    {
                        Workbook workbookC = new Workbook();
                        workbookC.LoadFromStream(stream);
                        Worksheet sheet = workbookC.Worksheets[0];

                        string fileSave = $"Report3{Guid.NewGuid()}";

                        sheet.SaveToHtml(fileSave);

                        string excelHtmlPath = Path.GetFullPath(Path.Combine(fileSave));
                        using (StreamReader reader = File.OpenText(excelHtmlPath))
                        {
                            htmlBody = reader.ReadToEnd();
                        }

                        var regex = new Regex(@"<[hH][2][^>]*>[^<]*</[hH][2]\s*>", RegexOptions.Compiled | RegexOptions.Multiline);
                        htmlBody = regex.Replace(htmlBody, "");

                        File.Delete(excelHtmlPath);
                    }

                    return (content, htmlBody);
                }
            }
        }

        private (byte[], string) GenerateReportDetailSaleByBrand(ReportDetailSaleByBrandRequest request, List<GroupStoreRanking> listGroup)
        {
            using (var workbook = new XLWorkbook())
            {
                var brandFragrances = repository.report.GetBrandFragances();
                Color yellowHead = Color.FromArgb(250, 250, 181);
                Color yellowHead2 = Color.FromArgb(250, 243, 27);
                Color totalColor = Color.FromArgb(139, 177, 245);
                Color contentColor = Color.FromArgb(243, 202, 245);
                Color sumtotalColor1 = Color.FromArgb(196, 227, 213);
                Color sumtotalColor2 = Color.FromArgb(93, 230, 240);

                XLColor yellowXL = XLColor.FromArgb(yellowHead.A, yellowHead.R, yellowHead.G, yellowHead.B);
                XLColor yellowXL2 = XLColor.FromArgb(yellowHead2.A, yellowHead2.R, yellowHead2.G, yellowHead2.B);
                XLColor totalXL = XLColor.FromArgb(totalColor.A, totalColor.R, totalColor.G, totalColor.B);
                XLColor contentXL = XLColor.FromArgb(contentColor.A, contentColor.R, contentColor.G, contentColor.B);
                XLColor sumtotalColor1XL = XLColor.FromArgb(sumtotalColor1.A, sumtotalColor1.R, sumtotalColor1.G, sumtotalColor1.B);
                XLColor sumtotalColor2XL = XLColor.FromArgb(sumtotalColor2.A, sumtotalColor2.R, sumtotalColor2.G, sumtotalColor2.B);

                #region Header
                var periodTime = GetPeriodTime(request.startMonth, request.startYear, request.endMonth, request.endYear);
                int counHead = 3 + periodTime.Count();
                var worksheet = workbook.Worksheets.Add("DetailSaleByBrand");
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, counHead)).Merge();
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, counHead)).Value = $"Details Sales by Brand {request.brandName}";
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, counHead)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, counHead)).Merge();
                string dateRepport = string.Empty;

                int monthStart = int.Parse(request.startMonth);

                dateRepport = $"{monthList[monthStart - 1]}/{request.startYear}";
                dateRepport += request.endMonth != null ? $" - {monthList[int.Parse(request.endMonth) - 1]}/{request.endYear}" : "";

                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, counHead)).Merge();
                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, counHead)).Value = dateRepport;
                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, counHead)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                #endregion

                worksheet.Column(1).Width = 3;
                worksheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Column(2).Width = 30;
                worksheet.Row(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Row(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(5, 1)).Merge();
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(5, 1)).Value = "#";
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(5, 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(5, 2)).Merge();
                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(5, 2)).Value = "DEPARTMENT STORES";
                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(5, 2)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(5, 2)).Style.Fill.BackgroundColor = yellowXL2;



                worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 2 + periodTime.Count())).Merge();
                worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 2 + periodTime.Count())).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 2 + periodTime.Count())).Value = "Month";
                worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 2 + periodTime.Count())).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 2 + periodTime.Count())).Style.Fill.BackgroundColor = yellowXL;

                int columnnPeriod = 3;
                foreach (var period in periodTime)
                {
                    worksheet.Column(columnnPeriod).Width = 15;
                    worksheet.Cell(5, columnnPeriod).SetValue(Convert.ToString($"{period.monthDisplay}-{period.yearDisplay}"));
                    worksheet.Cell(5, columnnPeriod).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(5, columnnPeriod).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    columnnPeriod++;
                }

                worksheet.Column(columnnPeriod).Width = 19;
                worksheet.Range(worksheet.Cell(4, columnnPeriod), worksheet.Cell(5, columnnPeriod)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(4, columnnPeriod), worksheet.Cell(5, columnnPeriod)).Style.Fill.BackgroundColor = totalXL;
                worksheet.Range(worksheet.Cell(4, columnnPeriod), worksheet.Cell(5, columnnPeriod)).Merge();
                worksheet.Range(worksheet.Cell(4, columnnPeriod), worksheet.Cell(5, columnnPeriod)).Value = "TOTAL";

                var allStoreList = listGroup.GroupBy(
                         x => new
                         {
                             x.storeName,
                             x.storeID,
                         })
                     .Select(e => new
                     {
                         storeName = e.Key.storeName,
                         storeID = e.Key.storeID,
                     }).OrderBy(z => z.storeName).ToList();

                int rowStore = 6;
                int countStore = 1;

                foreach (var itemStore in allStoreList)
                {
                    worksheet.Cell(rowStore, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowStore, 1).SetValue(Convert.ToString($"{countStore}"));
                    worksheet.Cell(rowStore, 2).SetValue($"{itemStore.storeName}");
                    worksheet.Cell(rowStore, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    int columnnPeriodDetail = 3;
                    decimal sumTotalOnPeriod = 0;
                    foreach (var itemPeriod in periodTime)
                    {
                        worksheet.Cell(rowStore, columnnPeriodDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(rowStore, columnnPeriodDetail).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        var totalInPeriod = listGroup.FirstOrDefault(c => c.month == itemPeriod.month && c.year == itemPeriod.year && c.storeID == itemStore.storeID);
                        worksheet.Cell(rowStore, columnnPeriodDetail).SetValue(totalInPeriod != null ? string.Format("{0:#,0}", totalInPeriod.sumStore) : "0");

                        if (totalInPeriod != null)
                        {
                            sumTotalOnPeriod += totalInPeriod.sumStore;
                        }
                        columnnPeriodDetail++;
                    }

                    if (countStore % 2 == 0)
                    {
                        worksheet.Range(worksheet.Cell(rowStore, 1), worksheet.Cell(rowStore, columnnPeriodDetail)).Style.Fill.BackgroundColor = contentXL;
                    }

                    worksheet.Cell(rowStore, columnnPeriodDetail).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowStore, columnnPeriodDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(rowStore, columnnPeriodDetail).SetValue(string.Format("{0:#,0}", sumTotalOnPeriod));

                    rowStore++;
                    countStore++;
                }

                worksheet.Range(worksheet.Cell(rowStore, 1), worksheet.Cell(rowStore, 2)).Merge();
                worksheet.Range(worksheet.Cell(rowStore, 1), worksheet.Cell(rowStore, 2)).Value = "TOTAL";
                worksheet.Range(worksheet.Cell(rowStore, 1), worksheet.Cell(rowStore, 2)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(rowStore, 1), worksheet.Cell(rowStore, 2)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(rowStore, 1), worksheet.Cell(rowStore, 2)).Style.Fill.BackgroundColor = sumtotalColor1XL;

                int columnnPeriodTotal = 3;
                decimal allSummaryPeriod = 0;
                foreach (var itemPeriod in periodTime)
                {
                    worksheet.Cell(rowStore, columnnPeriodTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowStore, columnnPeriodTotal).Style.Fill.BackgroundColor = sumtotalColor1XL;
                    worksheet.Cell(rowStore, columnnPeriodTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    var sumTotalPeriod = listGroup.Where(c => c.month == itemPeriod.month && c.year == itemPeriod.year).Sum(c => c.sumStore);
                    worksheet.Cell(rowStore, columnnPeriodTotal).SetValue(string.Format("{0:#,0}", sumTotalPeriod));

                    allSummaryPeriod += sumTotalPeriod;
                    columnnPeriodTotal++;
                }

                worksheet.Cell(rowStore, columnnPeriodTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                worksheet.Cell(rowStore, columnnPeriodTotal).Style.Fill.BackgroundColor = sumtotalColor2XL;
                worksheet.Cell(rowStore, columnnPeriodTotal).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowStore, columnnPeriodTotal).SetValue(string.Format("{0:#,0}", allSummaryPeriod));

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string htmlBody = string.Empty;

                    if (request.preview)
                    {
                        Workbook workbookC = new Workbook();
                        workbookC.LoadFromStream(stream);
                        Worksheet sheet = workbookC.Worksheets[0];

                        string fileSave = $"Report4{Guid.NewGuid()}";

                        sheet.SaveToHtml(fileSave);

                        string excelHtmlPath = Path.GetFullPath(Path.Combine(fileSave));
                        using (StreamReader reader = File.OpenText(excelHtmlPath))
                        {
                            htmlBody = reader.ReadToEnd();
                        }

                        var regex = new Regex(@"<[hH][2][^>]*>[^<]*</[hH][2]\s*>", RegexOptions.Compiled | RegexOptions.Multiline);
                        htmlBody = regex.Replace(htmlBody, "");

                        File.Delete(excelHtmlPath);
                    }

                    return (content, htmlBody);
                }
            }
        }

        private (byte[], string) GenerateReportExcelDataExporting(ReportExcelDataExportRequest request, List<Data_Exporting> listData)
        {
            using (var workbook = new XLWorkbook())
            {
                var brandFragrances = repository.report.GetBrandFragances();
                Color colorHead = Color.FromArgb(250, 108, 250);
                XLColor headXL = XLColor.FromArgb(colorHead.A, colorHead.R, colorHead.G, colorHead.B);

                #region Header
                var worksheet = workbook.Worksheets.Add("ExcelDataExporting");

                worksheet.Row(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                string dateRepport = string.Empty;

                int monthStart = int.Parse(request.startMonth);

                dateRepport = $"{monthList[monthStart - 1]}/{request.startYear}";
                dateRepport += request.endMonth != null ? $" - {monthList[int.Parse(request.endMonth) - 1]}/{request.endYear}" : "";

                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 13)).Merge();
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 13)).Value = $"Excel Data Exporing File {dateRepport}";
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 13)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Column(1).Width = 22;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 22;
                worksheet.Column(4).Width = 10;
                worksheet.Column(5).Width = 30;
                worksheet.Column(6).Width = 19;
                worksheet.Column(7).Width = 19;
                worksheet.Column(8).Width = 19;
                worksheet.Column(9).Width = 10;
                worksheet.Column(10).Width = 10;
                worksheet.Column(11).Width = 10;
                worksheet.Column(12).Width = 19;
                worksheet.Column(13).Width = 19;

                worksheet.Row(2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(2, 1).Value = "Department Stores Group";
                worksheet.Cell(2, 2).Value = "Department Stores";
                worksheet.Cell(2, 3).Value = "Department Stores Rank";
                worksheet.Cell(2, 4).Value = "Region";
                worksheet.Cell(2, 5).Value = "Brand Group";
                worksheet.Cell(2, 6).Value = "Brand Type";
                worksheet.Cell(2, 7).Value = "Brand Segment";
                worksheet.Cell(2, 8).Value = "Brand";
                worksheet.Cell(2, 9).Value = "Week";
                worksheet.Cell(2, 10).Value = "Month";
                worksheet.Cell(2, 11).Value = "Year";
                worksheet.Cell(2, 12).Value = "Amount";
                worksheet.Cell(2, 13).Value = "Whole";

                worksheet.Cell(2, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2, 11).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(2, 13).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 13)).Style.Fill.BackgroundColor = headXL;

                #endregion

                int rowData = 3;
                foreach (var itemData in listData)
                {
                    worksheet.Row(rowData).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(rowData, 1).Value = itemData.StoreG_Name;
                    worksheet.Cell(rowData, 2).Value = itemData.Store_Name;
                    worksheet.Cell(rowData, 3).SetValue(Convert.ToString($"{itemData.Store_Rank}"));
                    worksheet.Cell(rowData, 4).Value = itemData.Region_Name;
                    worksheet.Cell(rowData, 5).Value = itemData.BrandG_Name;
                    worksheet.Cell(rowData, 6).Value = itemData.Brand_Type_Name;
                    worksheet.Cell(rowData, 7).Value = itemData.Brand_Segment_Name;
                    worksheet.Cell(rowData, 8).Value = itemData.Brand_Name;
                    worksheet.Cell(rowData, 9).Value = itemData.Sales_Week;
                    worksheet.Cell(rowData, 10).Value = itemData.Sales_Month;
                    worksheet.Cell(rowData, 11).Value = itemData.Sales_Year;
                    worksheet.Cell(rowData, 12).SetValue(string.Format("{0:#,0}", itemData.Amount_Sales));
                    worksheet.Cell(rowData, 13).SetValue(string.Format("{0:#,0}", itemData.Whole_Sales));

                    worksheet.Cell(rowData, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 11).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(rowData, 13).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    rowData++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string htmlBody = string.Empty;

                    if (request.preview)
                    {
                        Workbook workbookC = new Workbook();
                        workbookC.LoadFromStream(stream);
                        Worksheet sheet = workbookC.Worksheets[0];

                        string fileSave = $"Report5{Guid.NewGuid()}";

                        sheet.SaveToHtml(fileSave);

                        string excelHtmlPath = Path.GetFullPath(Path.Combine(fileSave));
                        using (StreamReader reader = File.OpenText(excelHtmlPath))
                        {
                            htmlBody = reader.ReadToEnd();
                        }

                        var regex = new Regex(@"<[hH][2][^>]*>[^<]*</[hH][2]\s*>", RegexOptions.Compiled | RegexOptions.Multiline);
                        htmlBody = regex.Replace(htmlBody, "");

                        File.Delete(excelHtmlPath);
                    }

                    return (content, htmlBody);
                }
            }
        }

        private List<PeriodTime> GetPeriodTime(string startMonth, string startYear, string endMonth, string endYear)
        {
            List<PeriodTime> periodList = new List<PeriodTime>();

            if (endMonth != null && endYear != null)
            {
                int yearStart = int.Parse(startYear);
                int monthStart = int.Parse(startMonth);
                int yearEnd = int.Parse(endYear);
                int monthEnd = int.Parse(endMonth);

                if (yearStart != yearEnd)
                {
                    int diffYear = yearEnd - yearStart;

                    for (int j = 0; j <= diffYear; j++)
                    {
                        if (yearStart != yearEnd)
                        {
                            for (int i = monthStart - 1; i < 12; i++)
                            {
                                var monthText = (i + 1).ToString();

                                PeriodTime time = new PeriodTime
                                {
                                    year = startYear,
                                    month = monthText.Length == 1 ? $"0{monthText}" : monthText,
                                    yearDisplay = startYear,
                                    monthDisplay = monthList[i]
                                };

                                periodList.Add(time);
                            }

                            monthStart = 1;
                            yearStart++;
                        }
                        else
                        {
                            for (int i = 0; i < monthEnd; i++)
                            {
                                var monthText = (i + 1).ToString();

                                PeriodTime time = new PeriodTime
                                {
                                    year = endYear,
                                    month = monthText.Length == 1 ? $"0{monthText}" : monthText,
                                    yearDisplay = endYear,
                                    monthDisplay = monthList[i]
                                };

                                periodList.Add(time);
                            }
                        }

                    }

                }
                else
                {
                    for (int i = monthStart - 1; i < monthEnd; i++)
                    {
                        var monthText = (i + 1).ToString();

                        PeriodTime time = new PeriodTime
                        {
                            year = startYear,
                            month = monthText.Length == 1 ? $"0{monthText}" : monthText,
                            yearDisplay = startYear,
                            monthDisplay = monthList[i]
                        };

                        periodList.Add(time);
                    }
                }
            }
            else
            {
                int monthStart = int.Parse(startMonth);

                PeriodTime time = new PeriodTime
                {
                    year = startYear,
                    month = startMonth,
                    yearDisplay = startYear,
                    monthDisplay = monthList[monthStart - 1]
                };

                periodList.Add(time);
            }


            return periodList;
        }
    }

    public class GroupStoreRanking
    {
        public Guid storeID { get; set; }
        public string storeName { get; set; }
        public decimal sumStore { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public List<Brand_Ranking> brandDetail { get; set; }
    }

    public class GroupBrandRanking
    {
        public Guid brandID { get; set; }
        public string brandName { get; set; }
        public decimal sumBrand { get; set; }
        public string color { get; set; }
        public List<Brand_Ranking> storeDetail { get; set; }
    }

    public class PeriodTime
    {
        public string year { get; set; }
        public string month { get; set; }
        public string yearDisplay { get; set; }
        public string monthDisplay { get; set; }
    }
}
