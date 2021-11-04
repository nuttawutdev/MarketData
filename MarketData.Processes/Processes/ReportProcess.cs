using ClosedXML.Excel;
using MarketData.Model.Entiry;
using MarketData.Model.Request.Report;
using MarketData.Repositories;
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

        public ReportProcess(Repository repository)
        {
            this.repository = repository;
        }
        public string PreviewStoreMarketShareZone(ReportStoreMarketShareZoneRequest request)
        {
            try
            {
                List<Brand_Ranking> brandRankingData;
                // MTD
                if (string.IsNullOrWhiteSpace(request.endWeek))
                {
                    List<GroupBrandRanking> groupBrandStore = new List<GroupBrandRanking>();
                    brandRankingData = repository.report.GetBrandRankingBy(
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

                        List<GroupBrandRanking> groupStoreStartYear = new List<GroupBrandRanking>();
                        groupStoreStartYear = brandRankingStartYear.GroupBy(
                            x => new
                            {
                                x.Store_Id,
                                x.Department_Store_Name
                            })
                        .Select(e => new GroupBrandRanking
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

                        List<GroupBrandRanking> groupStoreCompareYear = new List<GroupBrandRanking>();

                        groupStoreCompareYear = brandRankingCompareYear.GroupBy(
                        x => new
                        {
                            x.Store_Id,
                            x.Department_Store_Name
                        })
                        .Select(e => new GroupBrandRanking
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

                        List<GroupBrandRanking> groupStoreCompareOldYear = new List<GroupBrandRanking>();

                        groupStoreCompareOldYear = brandRankingDataOlpCompare.Where(c => storeFilter.Contains(c.Store_Id)).GroupBy(
                        x => new
                        {
                            x.Store_Id,
                            x.Department_Store_Name
                        })
                        .Select(e => new GroupBrandRanking
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

                        return PreviewReportStoreMarketShareValue(request, groupStoreStartYear, groupStoreCompareYear, groupStoreCompareOldYear);
                    }
                }
              
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public byte[] ExportStoreMarketShareZone(ReportStoreMarketShareZoneRequest request)
        {
            try
            {
                List<Brand_Ranking> brandRankingData;
                // MTD
                if (string.IsNullOrWhiteSpace(request.endWeek))
                {
                    List<GroupBrandRanking> groupBrandStore = new List<GroupBrandRanking>();
                    brandRankingData = repository.report.GetBrandRankingBy(
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

                        List<GroupBrandRanking> groupStoreStartYear = new List<GroupBrandRanking>();
                        groupStoreStartYear = brandRankingStartYear.GroupBy(
                            x => new
                            {
                                x.Store_Id,
                                x.Department_Store_Name
                            })
                        .Select(e => new GroupBrandRanking
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

                        List<GroupBrandRanking> groupStoreCompareYear = new List<GroupBrandRanking>();

                        groupStoreCompareYear = brandRankingCompareYear.GroupBy(
                        x => new
                        {
                            x.Store_Id,
                            x.Department_Store_Name
                        })
                        .Select(e => new GroupBrandRanking
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

                        List<GroupBrandRanking> groupStoreCompareOldYear = new List<GroupBrandRanking>();

                        groupStoreCompareOldYear = brandRankingDataOlpCompare.Where(c => storeFilter.Contains(c.Store_Id)).GroupBy(
                        x => new
                        {
                            x.Store_Id,
                            x.Department_Store_Name
                        })
                        .Select(e => new GroupBrandRanking
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

                        return GenerateReportStoreMarketShareValue(request, groupStoreStartYear, groupStoreCompareYear, groupStoreCompareOldYear);
                    }
                }
                // YTD
                else
                {
                    List<GroupBrandRanking> groupBrandStore = new List<GroupBrandRanking>();
                    int timeFilterStart = int.Parse(request.startYear + request.startMonth + request.startWeek);
                    int timeFilterEnd = int.Parse(request.endYear + request.endMonth + request.endWeek);

                    brandRankingData = repository.report.GetBrandRankingBy(
                       c => (request.brandType == null || c.Brand_Type_ID == request.brandType)
                       && (request.universe == null || c.Universe == request.universe)
                       && (request.departmentStoreList == null
                       || !request.departmentStoreList.Any()
                       || (request.departmentStoreList != null && request.departmentStoreList.Contains(c.Store_Id)))
                       && (c.Time_Keyin >= timeFilterStart && c.Time_Keyin <= timeFilterEnd));

                    var groupStoreStartYear = brandRankingData.GroupBy(
                        x => new
                        {
                            x.Store_Id,
                            x.Department_Store_Name
                        })
                        .Select(e => new GroupBrandRanking
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

                    var groupStoreCompareYear = brandRankingCompareData.GroupBy(
                       x => new
                       {
                           x.Store_Id,
                           x.Department_Store_Name
                       })
                       .Select(e => new GroupBrandRanking
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

                    var groupStoreCompareOldYear = brandRankingDataOlpCompare.Where(c => storeFilter.Contains(c.Store_Id)).GroupBy(
                      x => new
                      {
                          x.Store_Id,
                          x.Department_Store_Name
                      })
                      .Select(e => new GroupBrandRanking
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

                    return GenerateReportStoreMarketShareZone(request, groupStoreStartYear, groupStoreCompareYear, groupStoreCompareOldYear);
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private byte[] GenerateReportStoreMarketShareZone(ReportStoreMarketShareZoneRequest request, List<GroupBrandRanking> listGroup, List<GroupBrandRanking> listGroupCompare, List<GroupBrandRanking> listGroupOldCompare)
        {
            using (var workbook = new XLWorkbook())
            {
                Color yellowHead = Color.FromArgb(250, 250, 181);
                XLColor yellowXL = XLColor.FromArgb(yellowHead.A, yellowHead.R, yellowHead.G, yellowHead.B);

                #region Header
                var worksheet = workbook.Worksheets.Add("Transaction");
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 30)).Merge();
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 30)).Value = $"DEPARTMENT STORES PANEL - TOP {request.brandRankEnd}";
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 30)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 30)).Merge();
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 30)).Value = "MARKET SHARE AND GROWTH";
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 30)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, 30)).Merge();

                string dateQuery = string.Empty;
                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, 30)).SetValue(Convert.ToString("Jul-21"));
                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, 30)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 19;
                worksheet.Column(3).Width = 19;
                worksheet.Column(4).Width = 19;
                worksheet.Column(5).Width = 19;

                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(7, 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(7, 1)).Merge();
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(7, 1)).Value = "DEPARTMENT STORES";

                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(7, 1)).Style.Fill.BackgroundColor = yellowXL;
                worksheet.Cell(4, 2).Style.Fill.BackgroundColor = yellowXL;
                worksheet.Cell(5, 2).Style.Fill.BackgroundColor = yellowXL;
                worksheet.Cell(6, 2).Style.Fill.BackgroundColor = yellowXL;

                worksheet.Cell(4, 2).Value = "RETAIL";
                worksheet.Cell(5, 2).Value = "SALES";
                worksheet.Cell(6, 2).SetValue(Convert.ToString("Jul-20"));
                worksheet.Cell(4, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(5, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(6, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(4, 3).Value = "RETAIL";
                worksheet.Cell(5, 3).Value = "SALES";
                worksheet.Cell(6, 3).SetValue(Convert.ToString("Jul-21"));
                worksheet.Cell(4, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(5, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(6, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(4, 4).Value = "%OF";
                worksheet.Cell(5, 4).Value = "GROWTH*";
                worksheet.Cell(6, 4).SetValue(Convert.ToString("2021"));
                worksheet.Cell(4, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(5, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(6, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(4, 5).Value = "%OF";
                worksheet.Cell(5, 5).Value = "SHARE*";
                worksheet.Cell(6, 5).SetValue(Convert.ToString("2021")); //// Gen
                worksheet.Cell(4, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(5, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(6, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(5, 30)).Merge();
                worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(5, 30)).Value = "RANKING";
                worksheet.Range(worksheet.Cell(4, 6), worksheet.Cell(5, 30)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                #endregion

                int columnBrand = 6;
                for (int i = 1; i <= request.brandRankEnd; i++)
                {
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
                    worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(6, columnBrand + 1)).Value = $"{brandLorealList[i]}";
                    worksheet.Range(worksheet.Cell(7, columnBrand), worksheet.Cell(7, columnBrand + 1)).Value = $"If Not In Top {request.brandRankEnd}";
                    worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(6, columnBrand + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(6, columnBrand + 1)).Merge();
                    worksheet.Range(worksheet.Cell(7, columnBrand), worksheet.Cell(7, columnBrand + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(7, columnBrand), worksheet.Cell(7, columnBrand + 1)).Merge();
                    columnBrand = columnBrand + 2;
                }

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
                    worksheet.Range(worksheet.Cell(rowData, 2), worksheet.Cell(rowData + 1, 2)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Range(worksheet.Cell(rowData, 3), worksheet.Cell(rowData + 1, 3)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Range(worksheet.Cell(rowData, 4), worksheet.Cell(rowData + 1, 4)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Range(worksheet.Cell(rowData, 5), worksheet.Cell(rowData + 1, 5)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 1)).Value = itemGroup.storeName;
                    worksheet.Range(worksheet.Cell(rowData, 2), worksheet.Cell(rowData + 1, 2)).SetValue(storeCompare != null ? string.Format("{0:#,0}", storeCompare.sumStore) : "0");
                    worksheet.Range(worksheet.Cell(rowData, 3), worksheet.Cell(rowData + 1, 3)).SetValue(string.Format("{0:#,0}", itemGroup.sumStore));

                    var percentGrowth = Math.Round(storeCompare != null && storeCompare.sumStore != 0 ? ((itemGroup.sumStore / storeCompare.sumStore) - 1) * 100 : -100, 2);
                    worksheet.Range(worksheet.Cell(rowData, 4), worksheet.Cell(rowData + 1, 4)).SetValue($"{percentGrowth}%");

                    var percentShare = Math.Round((itemGroup.sumStore / sumAllStore) * 100, 2);
                    worksheet.Range(worksheet.Cell(rowData, 5), worksheet.Cell(rowData + 1, 5)).SetValue($"{percentShare}%");

                    int columnBrandDetail = 6;
                    List<Brand_Ranking> listBrandSelect = new List<Brand_Ranking>();

                    if (!request.brandRankStart.HasValue)
                    {
                        request.brandRankStart = 1;
                    }

                    for (int i = request.brandRankStart.GetValueOrDefault() - 1; i <= request.brandRankEnd.GetValueOrDefault() - 1; i++)
                    {
                        if (itemGroup.brandDetail.ElementAtOrDefault(i) != null)
                        {
                            listBrandSelect.Add(itemGroup.brandDetail.ElementAtOrDefault(i));
                        }
                    }

                    foreach (var itemBrand in listBrandSelect)
                    {
                        if (storeCompare != null)
                        {
                            var brandCompare = storeCompare.brandDetail.FirstOrDefault(c => c.Brand_ID == itemBrand.Brand_ID);
                            decimal percentGrowthBrand = -100;
                            if (request.saleType == "Amount")
                            {
                                percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Amount_Sales.GetValueOrDefault() != 0
                                    ? ((itemBrand.Amount_Sales.GetValueOrDefault() / brandCompare.Amount_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            }
                            else if (request.saleType == "Whole")
                            {
                                percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Whole_Sales.GetValueOrDefault() != 0
                                     ? ((itemBrand.Whole_Sales.GetValueOrDefault() / brandCompare.Whole_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            }
                            else if (request.saleType == "Net")
                            {
                                percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Net_Sales.GetValueOrDefault() != 0
                                    ? ((itemBrand.Net_Sales.GetValueOrDefault() / brandCompare.Net_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            }


                            worksheet.Cell(rowData + 1, columnBrandDetail + 1).SetValue($"{percentGrowthBrand}%");
                        }
                        else
                        {
                            worksheet.Cell(rowData + 1, columnBrandDetail + 1).SetValue($"-100%");
                        }

                        worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Merge();
                        worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).SetValue(itemBrand.Brand_Name);

                        decimal percentShareBrand = 0;
                        if (request.saleType == "Amount")
                        {
                            percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Amount_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        }
                        else if (request.saleType == "Whole")
                        {
                            percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Whole_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        }
                        else if (request.saleType == "Net")
                        {
                            percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Net_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        }

                        worksheet.Cell(rowData + 1, columnBrandDetail).SetValue($"{percentShareBrand}%");
                        columnBrandDetail = columnBrandDetail + 2;

                    }

                    var listBrandTopSelectName = listBrandSelect.Select(c => c.Brand_Name);
                    columnBrandDetail++;

                    foreach (var itemBrandLoreal in brandLorealList)
                    {
                        for (int i = 0; i < itemGroup.brandDetail.Count(); i++)
                        {
                            var brandNotTopDetail = itemGroup.brandDetail[i];
                            if (brandNotTopDetail.Brand_Name == itemBrandLoreal
                                && !listBrandTopSelectName.Contains(brandNotTopDetail.Brand_Name))
                            {
                                worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Value = $"{itemBrandLoreal} [#{i + 1}]";
                                worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
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

                                break;
                            }
                        }

                        columnBrandDetail = columnBrandDetail + 2;
                    }

                    rowData = rowData + 2;
                }

                var percentGrowthTotal = Math.Round(((sumAllStore / sumAllStoreCompare) - 1) * 100, 2);
                var allBrandDetail = listGroup.SelectMany(c => c.brandDetail);
                var allBrandDetailCompare = listGroupCompare.SelectMany(c => c.brandDetail);
                var allBrandDetailOldCompare = listGroupOldCompare.SelectMany(c => c.brandDetail);

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
                worksheet.Range(worksheet.Cell(rowData, 1), worksheet.Cell(rowData + 1, 1)).Value = $"TOTAL {request.startYear}";
                worksheet.Range(worksheet.Cell(rowData, 2), worksheet.Cell(rowData + 1, 2)).SetValue(string.Format("{0:#,0}", sumAllStoreCompare));
                worksheet.Range(worksheet.Cell(rowData, 3), worksheet.Cell(rowData + 1, 3)).SetValue(string.Format("{0:#,0}", sumAllStore));
                worksheet.Range(worksheet.Cell(rowData, 4), worksheet.Cell(rowData + 1, 4)).SetValue($"{percentGrowthTotal}%");
                worksheet.Range(worksheet.Cell(rowData, 5), worksheet.Cell(rowData + 1, 5)).SetValue($"100%");

                int columnBrandTotal = 6;
                List<string> brandTotalSelect = new List<string>();
                for (int i = 0; i < request.brandRankEnd; i++)
                {
                    var brandDetail = groupBrandDetail[i];
                    brandTotalSelect.Add(brandDetail.brandName);
                    var brandCompare = groupBrandDetailCompare.FirstOrDefault(c => c.brandID == brandDetail.brandID);

                    var percentShareBrand = Math.Round((brandDetail.sumTotalBrand / sumAllStore) * 100, 2);
                    worksheet.Cell(rowData + 1, columnBrandTotal).SetValue($"{percentShareBrand}%");

                    if (brandCompare != null)
                    {
                        var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumTotalBrand > 0 ? ((brandDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
                        worksheet.Cell(rowData + 1, columnBrandTotal + 1).SetValue($"{percentGrowthBrand}%");
                    }
                    else
                    {
                        worksheet.Cell(rowData + 1, columnBrandTotal + 1).SetValue($"-100%");
                    }


                    worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Merge();
                    worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).SetValue(brandDetail.brandName);

                    columnBrandTotal = columnBrandTotal + 2;
                }

                columnBrandTotal++;

                foreach (var itemBrandLoreal in brandLorealList)
                {
                    for (int i = 0; i < groupBrandDetail.Count(); i++)
                    {
                        var brandNotTopDetail = groupBrandDetail[i];
                        if (brandNotTopDetail.brandName == itemBrandLoreal
                            && !brandTotalSelect.Contains(brandNotTopDetail.brandName))
                        {
                            worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Value = $"{itemBrandLoreal} [#{i + 1}]";
                            worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Range(worksheet.Cell(rowData, columnBrandTotal), worksheet.Cell(rowData, columnBrandTotal + 1)).Merge();

                            var percentShareBrand = Math.Round((brandNotTopDetail.sumTotalBrand / sumAllStore) * 100, 2);
                            worksheet.Cell(rowData + 1, columnBrandTotal).SetValue($"{percentShareBrand}%");

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

                    columnBrandTotal = columnBrandTotal + 2;
                }
                #endregion

                #region Total 2
                worksheet.Range(worksheet.Cell(rowData + 2, 1), worksheet.Cell(rowData + 3, 1)).Merge();
                worksheet.Range(worksheet.Cell(rowData + 2, 2), worksheet.Cell(rowData + 3, 2)).Merge();
                worksheet.Range(worksheet.Cell(rowData + 2, 3), worksheet.Cell(rowData + 3, 3)).Merge();
                worksheet.Range(worksheet.Cell(rowData + 2, 4), worksheet.Cell(rowData + 3, 4)).Merge();
                worksheet.Range(worksheet.Cell(rowData + 2, 5), worksheet.Cell(rowData + 3, 5)).Merge();

                worksheet.Range(worksheet.Cell(rowData + 2, 1), worksheet.Cell(rowData + 3, 1)).Value = $"TOTAL {request.compareYear}";
                worksheet.Range(worksheet.Cell(rowData + 2, 2), worksheet.Cell(rowData + 3, 2)).SetValue(string.Format("{0:#,0}", sumAllStoreCompare));

                int columnBrandTotalCompare = 6;

                for (int i = 0; i < request.brandRankEnd; i++)
                {
                    var brandDetail = groupBrandDetailCompare[i];
                    var brandCompare = groupBrandDetailOldCompare.FirstOrDefault(c => c.brandID == brandDetail.brandID);

                    var percentShareBrand = Math.Round((brandDetail.sumTotalBrand / sumAllStoreCompare) * 100, 2);
                    worksheet.Cell(rowData + 3, columnBrandTotalCompare).SetValue($"{percentShareBrand}%");

                    if (brandCompare != null)
                    {
                        var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumTotalBrand > 0 ? ((brandDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
                        worksheet.Cell(rowData + 3, columnBrandTotalCompare + 1).SetValue($"{percentGrowthBrand}%");
                    }
                    else
                    {
                        worksheet.Cell(rowData + 3, columnBrandTotalCompare + 1).SetValue($"-100%");
                    }


                    worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Merge();
                    worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).SetValue(brandDetail.brandName);
                    columnBrandTotalCompare = columnBrandTotalCompare + 2;
                }

                columnBrandTotalCompare++;

                foreach (var itemBrandLoreal in brandLorealList)
                {
                    for (int i = 0; i < groupBrandDetailCompare.Count(); i++)
                    {
                        var brandNotTopDetail = groupBrandDetailCompare[i];
                        if (brandNotTopDetail.brandName == itemBrandLoreal
                            && !brandTotalSelect.Contains(brandNotTopDetail.brandName))
                        {
                            worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Value = $"{itemBrandLoreal} [#{i + 1}]";
                            worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Range(worksheet.Cell(rowData + 2, columnBrandTotalCompare), worksheet.Cell(rowData + 2, columnBrandTotalCompare + 1)).Merge();

                            var percentShareBrand = Math.Round((brandNotTopDetail.sumTotalBrand / sumAllStoreCompare) * 100, 2);
                            worksheet.Cell(rowData + 3, columnBrandTotalCompare).SetValue($"{percentShareBrand}%");

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

                    columnBrandTotalCompare = columnBrandTotalCompare + 2;
                }
                #endregion


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    Workbook workbookC = new Workbook();
                    workbookC.LoadFromStream(stream);
                    Worksheet sheet = workbookC.Worksheets[0];
                    sheet.SaveToHtml("report.html");

                    string htmlBody = string.Empty;
                    string excelHtmlPath = Path.GetFullPath(Path.Combine("report.html"));
                    using (StreamReader reader = File.OpenText(excelHtmlPath))
                    {
                        htmlBody = reader.ReadToEnd();
                    }

                    var regex = new Regex(@"<[hH][2][^>]*>[^<]*</[hH][2]\s*>", RegexOptions.Compiled | RegexOptions.Multiline);
                    htmlBody = regex.Replace(htmlBody, "");

                    File.Delete(excelHtmlPath);
                    return content;
                }
            }
        }

        private byte[] GenerateReportStoreMarketShareValue(ReportStoreMarketShareZoneRequest request, List<GroupBrandRanking> listGroup, List<GroupBrandRanking> listGroupCompare, List<GroupBrandRanking> listGroupOldCompare)
        {
            using (var workbook = new XLWorkbook())
            {
                Color yellowHead = Color.FromArgb(250, 250, 181);
                XLColor yellowXL = XLColor.FromArgb(yellowHead.A, yellowHead.R, yellowHead.G, yellowHead.B);

                #region Header
                var worksheet = workbook.Worksheets.Add("Transaction");
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 30)).Merge();
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 30)).Value = $"Luxury Products - TOP {request.brandRankEnd}";
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 30)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 30)).Merge();
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 30)).Value = "Brand Ranking Perfomance Key Counters";
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 30)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, 30)).Merge();

                string dateQuery = string.Empty;
                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, 30)).SetValue(Convert.ToString("Jul-21"));
                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, 30)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Column(1).Width = 3;
                worksheet.Column(2).Width = 30;

                worksheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(6, 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(6, 1)).Merge();

                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(6, 2)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(6, 2)).Merge();
                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(6, 2)).Value = "DEPARTMENT STORES";

                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(6, 2)).Style.Fill.BackgroundColor = yellowXL;

                worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 50)).Merge();

                int columnBrand = 3;
                for (int i = 1; i <= request.brandRankEnd; i++)
                {
                    worksheet.Column(columnBrand).Width = 19;
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Merge();
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).SetValue($"#{i}");
                    columnBrand++;
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
                    worksheet.Column(columnBrand).Width = 19;
                    worksheet.Cell(5, columnBrand).Value = $"{brandLorealList[i]}";
                    worksheet.Cell(6, columnBrand).Value = $"If Not In Top {request.brandRankEnd}";
                    worksheet.Cell(5, columnBrand).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(6, columnBrand).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    columnBrand++;

                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Merge();
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).SetValue("%");
                    columnBrand++;
                }
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

                    countStore++;

                    worksheet.Cell(rowData, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(rowData, 2).SetValue(itemGroup.storeName);

                    worksheet.Cell(rowData + 1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(rowData + 1, 2).SetValue(string.Format("{0:#,0}", itemGroup.sumStore));

                    int columnBrandDetail = 3;
                    List<Brand_Ranking> listBrandSelect = new List<Brand_Ranking>();

                    if (!request.brandRankStart.HasValue)
                    {
                        request.brandRankStart = 1;
                    }

                    for (int i = request.brandRankStart.GetValueOrDefault() - 1; i <= request.brandRankEnd.GetValueOrDefault() - 1; i++)
                    {
                        if (itemGroup.brandDetail.ElementAtOrDefault(i) != null)
                        {
                            listBrandSelect.Add(itemGroup.brandDetail.ElementAtOrDefault(i));
                        }
                    }

                    foreach (var itemBrand in listBrandSelect)
                    {

                        var brandCompare = storeCompare.brandDetail.FirstOrDefault(c => c.Brand_ID == itemBrand.Brand_ID);

                        worksheet.Cell(rowData, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(rowData, columnBrandDetail).SetValue(itemBrand.Brand_Name);

                        worksheet.Cell(rowData + 1, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(rowData + 1, columnBrandDetail).SetValue(string.Format("{0:#,0}", itemBrand.Amount_Sales));

                        decimal percentShareBrand = 0;
                        if (request.saleType == "Amount")
                        {
                            percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Amount_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        }
                        else if (request.saleType == "Whole")
                        {
                            percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Whole_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        }
                        else if (request.saleType == "Net")
                        {
                            percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Net_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        }

                        worksheet.Cell(rowData + 2, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(rowData + 2, columnBrandDetail).SetValue($"{percentShareBrand}%");

                        columnBrandDetail++;


                        if (storeCompare != null)
                        {
                            decimal percentGrowthBrand = -100;
                            if (request.saleType == "Amount")
                            {
                                percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Amount_Sales.GetValueOrDefault() != 0
                                    ? ((itemBrand.Amount_Sales.GetValueOrDefault() / brandCompare.Amount_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            }
                            else if (request.saleType == "Whole")
                            {
                                percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Whole_Sales.GetValueOrDefault() != 0
                                     ? ((itemBrand.Whole_Sales.GetValueOrDefault() / brandCompare.Whole_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            }
                            else if (request.saleType == "Net")
                            {
                                percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Net_Sales.GetValueOrDefault() != 0
                                    ? ((itemBrand.Net_Sales.GetValueOrDefault() / brandCompare.Net_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            }

                            worksheet.Cell(rowData, columnBrandDetail).SetValue($"{percentGrowthBrand}%");
                        }
                        else
                        {
                            worksheet.Cell(rowData, columnBrandDetail).SetValue($"-100%");
                        }

                        columnBrandDetail++;
                    }

                    var listBrandTopSelectName = listBrandSelect.Select(c => c.Brand_Name);
                    columnBrandDetail++;

                    foreach (var itemBrandLoreal in brandLorealList)
                    {
                        for (int i = 0; i < itemGroup.brandDetail.Count(); i++)
                        {
                            var brandNotTopDetail = itemGroup.brandDetail[i];
                            if (brandNotTopDetail.Brand_Name == itemBrandLoreal
                                && !listBrandTopSelectName.Contains(brandNotTopDetail.Brand_Name))
                            {
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

                                worksheet.Cell(rowData + 2, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                worksheet.Cell(rowData + 2, columnBrandDetail).SetValue($"{percentShareBrand}%");

                                worksheet.Cell(rowData, columnBrandDetail + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

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

                                    worksheet.Cell(rowData, columnBrandDetail + 1).SetValue($"{percentGrowthBrand}%");
                                }
                                else
                                {
                                    worksheet.Cell(rowData, columnBrandDetail + 1).SetValue($"-100%");
                                }

                                break;
                            }
                        }

                        columnBrandDetail = columnBrandDetail + 2;
                    }

                    rowData = rowData + 3;
                }

                var percentGrowthTotal = Math.Round(((sumAllStore / sumAllStoreCompare) - 1) * 100, 2);
                var allBrandDetail = listGroup.SelectMany(c => c.brandDetail);
                var allBrandDetailCompare = listGroupCompare.SelectMany(c => c.brandDetail);
                var allBrandDetailOldCompare = listGroupOldCompare.SelectMany(c => c.brandDetail);

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
                #region Total 1
                worksheet.Cell(rowData, 2).Value = "Year";
                worksheet.Cell(rowData, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(rowData + 1, 2).Value = $"TOTAL {request.startYear}";
                worksheet.Cell(rowData + 1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(rowData + 2, 2).Value = $"TOTAL {request.compareYear}";
                worksheet.Cell(rowData + 2, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                int columnBrandTotal = 3;
                List<string> brandTotalSelect = new List<string>();
                for (int i = 0; i < request.brandRankEnd; i++)
                {
                    var brandDetail = groupBrandDetail[i];
                    brandTotalSelect.Add(brandDetail.brandName);

                    worksheet.Cell(rowData, columnBrandTotal).Value = brandDetail.brandName;
                    worksheet.Cell(rowData, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(rowData, columnBrandTotal + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    var brandCompare = groupBrandDetailCompare.FirstOrDefault(c => c.brandID == brandDetail.brandID);

                    worksheet.Cell(rowData + 1, columnBrandTotal).SetValue(string.Format("{0:#,0}", brandDetail.sumTotalBrand));
                    worksheet.Cell(rowData + 1, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    worksheet.Cell(rowData + 2, columnBrandTotal).SetValue(string.Format("{0:#,0}", brandCompare.sumTotalBrand));
                    worksheet.Cell(rowData + 2, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    if (brandCompare != null)
                    {
                        var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumTotalBrand > 0 ? ((brandDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
                        worksheet.Cell(rowData, columnBrandTotal + 1).SetValue($"{percentGrowthBrand}%");
                    }
                    else
                    {
                        worksheet.Cell(rowData, columnBrandTotal + 1).SetValue($"-100%");
                    }

                    columnBrandTotal = columnBrandTotal + 2;
                }

                columnBrandTotal++;

                foreach (var itemBrandLoreal in brandLorealList)
                {
                    for (int i = 0; i < groupBrandDetail.Count(); i++)
                    {
                        var brandNotTopDetail = groupBrandDetail[i];
                        if (brandNotTopDetail.brandName == itemBrandLoreal
                            && !brandTotalSelect.Contains(brandNotTopDetail.brandName))
                        {
                            worksheet.Cell(rowData, columnBrandTotal).Value = $"{itemBrandLoreal} [#{i + 1}]";
                            worksheet.Cell(rowData, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Cell(rowData, columnBrandTotal + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            var brandCompare = groupBrandDetailCompare.FirstOrDefault(c => c.brandID == brandNotTopDetail.brandID);

                            worksheet.Cell(rowData + 1, columnBrandTotal).SetValue(string.Format("{0:#,0}", brandNotTopDetail.sumTotalBrand));
                            worksheet.Cell(rowData + 1, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            worksheet.Cell(rowData + 2, columnBrandTotal).SetValue(string.Format("{0:#,0}", brandCompare.sumTotalBrand));
                            worksheet.Cell(rowData + 2, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            if (brandCompare != null)
                            {
                                var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumTotalBrand > 0 ? ((brandNotTopDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
                                worksheet.Cell(rowData, columnBrandTotal + 1).SetValue($"{percentGrowthBrand}%");
                            }
                            else
                            {
                                worksheet.Cell(rowData, columnBrandTotal + 1).SetValue($"-100%");
                            }

                            break;
                        }
                    }

                    columnBrandTotal = columnBrandTotal + 2;
                }
                #endregion

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    Workbook workbookC = new Workbook();
                    workbookC.LoadFromStream(stream);
                    Worksheet sheet = workbookC.Worksheets[0];
                    sheet.SaveToHtml("report2.html");

                    string htmlBody = string.Empty;
                    string excelHtmlPath = Path.GetFullPath(Path.Combine("report2.html"));
                    using (StreamReader reader = File.OpenText(excelHtmlPath))
                    {
                        htmlBody = reader.ReadToEnd();
                    }

                    var regex = new Regex(@"<[hH][2][^>]*>[^<]*</[hH][2]\s*>", RegexOptions.Compiled | RegexOptions.Multiline);
                    htmlBody = regex.Replace(htmlBody, "");

                    File.Delete(excelHtmlPath);

                    return content;
                }
            }
        }

        private string PreviewReportStoreMarketShareValue(ReportStoreMarketShareZoneRequest request, List<GroupBrandRanking> listGroup, List<GroupBrandRanking> listGroupCompare, List<GroupBrandRanking> listGroupOldCompare)
        {
            using (var workbook = new XLWorkbook())
            {
                Color yellowHead = Color.FromArgb(250, 250, 181);
                XLColor yellowXL = XLColor.FromArgb(yellowHead.A, yellowHead.R, yellowHead.G, yellowHead.B);

                #region Header
                var worksheet = workbook.Worksheets.Add("Transaction");
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 30)).Merge();
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 30)).Value = $"Luxury Products - TOP {request.brandRankEnd}";
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 30)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 30)).Merge();
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 30)).Value = "Brand Ranking Perfomance Key Counters";
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 30)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, 30)).Merge();

                string dateQuery = string.Empty;
                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, 30)).SetValue(Convert.ToString("Jul-21"));
                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, 30)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Column(1).Width = 3;
                worksheet.Column(2).Width = 30;

                worksheet.Column(1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(6, 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 1), worksheet.Cell(6, 1)).Merge();

                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(6, 2)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(6, 2)).Merge();
                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(6, 2)).Value = "DEPARTMENT STORES";

                worksheet.Range(worksheet.Cell(4, 2), worksheet.Cell(6, 2)).Style.Fill.BackgroundColor = yellowXL;

                worksheet.Range(worksheet.Cell(4, 3), worksheet.Cell(4, 50)).Merge();

                int columnBrand = 3;
                for (int i = 1; i <= request.brandRankEnd; i++)
                {
                    worksheet.Column(columnBrand).Width = 19;
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Merge();
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).SetValue($"#{i}");
                    columnBrand++;
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
                    worksheet.Column(columnBrand).Width = 19;
                    worksheet.Cell(5, columnBrand).Value = $"{brandLorealList[i]}";
                    worksheet.Cell(6, columnBrand).Value = $"If Not In Top {request.brandRankEnd}";
                    worksheet.Cell(5, columnBrand).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(6, columnBrand).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    columnBrand++;

                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).Merge();
                    worksheet.Range(worksheet.Cell(5, columnBrand), worksheet.Cell(6, columnBrand)).SetValue("%");
                    columnBrand++;
                }
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

                    countStore++;

                    worksheet.Cell(rowData, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(rowData, 2).SetValue(itemGroup.storeName);

                    worksheet.Cell(rowData + 1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(rowData + 1, 2).SetValue(string.Format("{0:#,0}", itemGroup.sumStore));

                    int columnBrandDetail = 3;
                    List<Brand_Ranking> listBrandSelect = new List<Brand_Ranking>();

                    if (!request.brandRankStart.HasValue)
                    {
                        request.brandRankStart = 1;
                    }

                    for (int i = request.brandRankStart.GetValueOrDefault() - 1; i <= request.brandRankEnd.GetValueOrDefault() - 1; i++)
                    {
                        if (itemGroup.brandDetail.ElementAtOrDefault(i) != null)
                        {
                            listBrandSelect.Add(itemGroup.brandDetail.ElementAtOrDefault(i));
                        }
                    }

                    foreach (var itemBrand in listBrandSelect)
                    {

                        var brandCompare = storeCompare.brandDetail.FirstOrDefault(c => c.Brand_ID == itemBrand.Brand_ID);

                        worksheet.Cell(rowData, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(rowData, columnBrandDetail).SetValue(itemBrand.Brand_Name);

                        worksheet.Cell(rowData + 1, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(rowData + 1, columnBrandDetail).SetValue(string.Format("{0:#,0}", itemBrand.Amount_Sales));

                        decimal percentShareBrand = 0;
                        if (request.saleType == "Amount")
                        {
                            percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Amount_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        }
                        else if (request.saleType == "Whole")
                        {
                            percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Whole_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        }
                        else if (request.saleType == "Net")
                        {
                            percentShareBrand = itemGroup.sumStore > 0 ? Math.Round((itemBrand.Net_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2) : 0;
                        }

                        worksheet.Cell(rowData + 2, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(rowData + 2, columnBrandDetail).SetValue($"{percentShareBrand}%");

                        columnBrandDetail++;


                        if (storeCompare != null)
                        {
                            decimal percentGrowthBrand = -100;
                            if (request.saleType == "Amount")
                            {
                                percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Amount_Sales.GetValueOrDefault() != 0
                                    ? ((itemBrand.Amount_Sales.GetValueOrDefault() / brandCompare.Amount_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            }
                            else if (request.saleType == "Whole")
                            {
                                percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Whole_Sales.GetValueOrDefault() != 0
                                     ? ((itemBrand.Whole_Sales.GetValueOrDefault() / brandCompare.Whole_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            }
                            else if (request.saleType == "Net")
                            {
                                percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.Net_Sales.GetValueOrDefault() != 0
                                    ? ((itemBrand.Net_Sales.GetValueOrDefault() / brandCompare.Net_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            }

                            worksheet.Cell(rowData, columnBrandDetail).SetValue($"{percentGrowthBrand}%");
                        }
                        else
                        {
                            worksheet.Cell(rowData, columnBrandDetail).SetValue($"-100%");
                        }

                        columnBrandDetail++;
                    }

                    var listBrandTopSelectName = listBrandSelect.Select(c => c.Brand_Name);
                    columnBrandDetail++;

                    foreach (var itemBrandLoreal in brandLorealList)
                    {
                        for (int i = 0; i < itemGroup.brandDetail.Count(); i++)
                        {
                            var brandNotTopDetail = itemGroup.brandDetail[i];
                            if (brandNotTopDetail.Brand_Name == itemBrandLoreal
                                && !listBrandTopSelectName.Contains(brandNotTopDetail.Brand_Name))
                            {
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

                                worksheet.Cell(rowData + 2, columnBrandDetail).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                worksheet.Cell(rowData + 2, columnBrandDetail).SetValue($"{percentShareBrand}%");

                                worksheet.Cell(rowData, columnBrandDetail + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

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

                                    worksheet.Cell(rowData, columnBrandDetail + 1).SetValue($"{percentGrowthBrand}%");
                                }
                                else
                                {
                                    worksheet.Cell(rowData, columnBrandDetail + 1).SetValue($"-100%");
                                }

                                break;
                            }
                        }

                        columnBrandDetail = columnBrandDetail + 2;
                    }

                    rowData = rowData + 3;
                }

                var percentGrowthTotal = Math.Round(((sumAllStore / sumAllStoreCompare) - 1) * 100, 2);
                var allBrandDetail = listGroup.SelectMany(c => c.brandDetail);
                var allBrandDetailCompare = listGroupCompare.SelectMany(c => c.brandDetail);
                var allBrandDetailOldCompare = listGroupOldCompare.SelectMany(c => c.brandDetail);

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
                #region Total 1
                worksheet.Cell(rowData, 2).Value = "Year";
                worksheet.Cell(rowData, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(rowData + 1, 2).Value = $"TOTAL {request.startYear}";
                worksheet.Cell(rowData + 1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(rowData + 2, 2).Value = $"TOTAL {request.compareYear}";
                worksheet.Cell(rowData + 2, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                int columnBrandTotal = 3;
                List<string> brandTotalSelect = new List<string>();
                for (int i = 0; i < request.brandRankEnd; i++)
                {
                    var brandDetail = groupBrandDetail[i];
                    brandTotalSelect.Add(brandDetail.brandName);

                    worksheet.Cell(rowData, columnBrandTotal).Value = brandDetail.brandName;
                    worksheet.Cell(rowData, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(rowData, columnBrandTotal + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    var brandCompare = groupBrandDetailCompare.FirstOrDefault(c => c.brandID == brandDetail.brandID);

                    worksheet.Cell(rowData + 1, columnBrandTotal).SetValue(string.Format("{0:#,0}", brandDetail.sumTotalBrand));
                    worksheet.Cell(rowData + 1, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    worksheet.Cell(rowData + 2, columnBrandTotal).SetValue(string.Format("{0:#,0}", brandCompare.sumTotalBrand));
                    worksheet.Cell(rowData + 2, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    if (brandCompare != null)
                    {
                        var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumTotalBrand > 0 ? ((brandDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
                        worksheet.Cell(rowData, columnBrandTotal + 1).SetValue($"{percentGrowthBrand}%");
                    }
                    else
                    {
                        worksheet.Cell(rowData, columnBrandTotal + 1).SetValue($"-100%");
                    }

                    columnBrandTotal = columnBrandTotal + 2;
                }

                columnBrandTotal++;

                foreach (var itemBrandLoreal in brandLorealList)
                {
                    for (int i = 0; i < groupBrandDetail.Count(); i++)
                    {
                        var brandNotTopDetail = groupBrandDetail[i];
                        if (brandNotTopDetail.brandName == itemBrandLoreal
                            && !brandTotalSelect.Contains(brandNotTopDetail.brandName))
                        {
                            worksheet.Cell(rowData, columnBrandTotal).Value = $"{itemBrandLoreal} [#{i + 1}]";
                            worksheet.Cell(rowData, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Cell(rowData, columnBrandTotal + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            var brandCompare = groupBrandDetailCompare.FirstOrDefault(c => c.brandID == brandNotTopDetail.brandID);

                            worksheet.Cell(rowData + 1, columnBrandTotal).SetValue(string.Format("{0:#,0}", brandNotTopDetail.sumTotalBrand));
                            worksheet.Cell(rowData + 1, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            worksheet.Cell(rowData + 2, columnBrandTotal).SetValue(string.Format("{0:#,0}", brandCompare.sumTotalBrand));
                            worksheet.Cell(rowData + 2, columnBrandTotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            if (brandCompare != null)
                            {
                                var percentGrowthBrand = Math.Round(brandCompare != null && brandCompare.sumTotalBrand > 0 ? ((brandNotTopDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
                                worksheet.Cell(rowData, columnBrandTotal + 1).SetValue($"{percentGrowthBrand}%");
                            }
                            else
                            {
                                worksheet.Cell(rowData, columnBrandTotal + 1).SetValue($"-100%");
                            }

                            break;
                        }
                    }

                    columnBrandTotal = columnBrandTotal + 2;
                }
                #endregion

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    Workbook workbookC = new Workbook();
                    workbookC.LoadFromStream(stream);
                    Worksheet sheet = workbookC.Worksheets[0];
                    sheet.SaveToHtml("report2.html");

                    string htmlBody = string.Empty;
                    string excelHtmlPath = Path.GetFullPath(Path.Combine("report2.html"));
                    using (StreamReader reader = File.OpenText(excelHtmlPath))
                    {
                        htmlBody = reader.ReadToEnd();
                    }

                    var regex = new Regex(@"<[hH][2][^>]*>[^<]*</[hH][2]\s*>", RegexOptions.Compiled | RegexOptions.Multiline);
                    htmlBody = regex.Replace(htmlBody, "");

                    File.Delete(excelHtmlPath);

                    return htmlBody;
                }
            }
        }
    }

    public class GroupBrandRanking
    {
        public Guid storeID { get; set; }
        public string storeName { get; set; }
        public decimal sumStore { get; set; }
        public List<Brand_Ranking> brandDetail { get; set; }
    }
}
