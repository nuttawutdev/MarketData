﻿using ClosedXML.Excel;
using MarketData.Model.Entiry;
using MarketData.Model.Request.Report;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace MarketData.Processes.Processes
{
    public class ReportProcess
    {
        private readonly Repository repository;

        public ReportProcess(Repository repository)
        {
            this.repository = repository;
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

                    if (brandRankingData.Any())
                    {
                        var brandRankingStartYear = brandRankingData.Where(w => w.Sales_Year == request.startYear);
                        var groupStoreStartYear = brandRankingStartYear.GroupBy(
                        x => new
                        {
                            x.Store_Id,
                            x.Department_Store_Name
                        })
                        .Select(e => new GroupBrandRanking
                        {
                            storeName = e.Key.Department_Store_Name,
                            storeID = e.Key.Store_Id,
                            sumStore = e.Sum(d => d.Amount_Sales.GetValueOrDefault()),
                            brandDetail = e.OrderByDescending(s => s.Amount_Sales).ToList()
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

                        var groupStoreCompareYear = brandRankingCompareYear.GroupBy(
                            x => new
                            {
                                x.Store_Id,
                                x.Department_Store_Name
                            })
                            .Select(e => new GroupBrandRanking
                            {
                                storeName = e.Key.Department_Store_Name,
                                storeID = e.Key.Store_Id,
                                sumStore = e.Sum(d => d.Amount_Sales.GetValueOrDefault()),
                                brandDetail = e.OrderByDescending(s => s.Amount_Sales).ToList()
                            }).OrderByDescending(s => s.sumStore).ToList();

                        var groupStoreCompareOldYear = brandRankingDataOlpCompare.Where(c=> storeFilter.Contains(c.Store_Id)).GroupBy(
                          x => new
                          {
                              x.Store_Id,
                              x.Department_Store_Name
                          })
                          .Select(e => new GroupBrandRanking
                          {
                              storeName = e.Key.Department_Store_Name,
                              storeID = e.Key.Store_Id,
                              sumStore = e.Sum(d => d.Amount_Sales.GetValueOrDefault()),
                              brandDetail = e.OrderByDescending(s => s.Amount_Sales).ToList()
                          }).OrderByDescending(s => s.sumStore).ToList();

                        return GenerateReportStoreMarketShareZone(request, groupStoreStartYear, groupStoreCompareYear, groupStoreCompareOldYear);
                    }
                    else
                    {

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
                            sumStore = e.Sum(d => d.Amount_Sales.GetValueOrDefault()),
                            brandDetail = e.OrderByDescending(s => s.Amount_Sales).ToList()
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

                    var groupStoreCompareYear = brandRankingCompareData
                         .GroupBy(c => c.Department_Store_Name).Select(e => new GroupBrandRanking
                         {
                             storeName = e.Key,
                             sumStore = e.Sum(d => d.Amount_Sales.GetValueOrDefault()),
                             brandDetail = e.OrderByDescending(s => s.Amount_Sales).ToList()
                         }).OrderByDescending(s => s.sumStore).ToList();
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
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 30)).Value = "DEPARTMENT STORES PANEL - TOP 10";
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 30)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 30)).Merge();
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 30)).Value = "MARKET SHARE AND GROWTH";
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 30)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Range(worksheet.Cell(3, 1), worksheet.Cell(3, 30)).Merge();
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
                worksheet.Cell(6, 5).SetValue(Convert.ToString("2021"));
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

                    // worksheet.Range(worksheet.Cell(6, columnBrand), worksheet.Cell(7, columnBrand + 1)).Value = $"{brandLorealList[i]} If Not In Top {request.brandRankEnd}";
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

                    var percentGrowth = Math.Round(storeCompare != null ? ((itemGroup.sumStore / storeCompare.sumStore) - 1) * 100 : -100, 2);
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
                            var percentGrowthBrand = Math.Round(brandCompare != null ? ((itemBrand.Amount_Sales.GetValueOrDefault() / brandCompare.Amount_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
                            worksheet.Cell(rowData + 1, columnBrandDetail + 1).SetValue($"{percentGrowthBrand}%");
                        }
                        else
                        {
                            worksheet.Cell(rowData + 1, columnBrandDetail + 1).SetValue($"-100%");
                        }

                        worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Merge();
                        worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).SetValue(itemBrand.Brand_Name);

                        var percentShareBrand = Math.Round((itemBrand.Amount_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2);
                        worksheet.Cell(rowData + 1, columnBrandDetail).SetValue($"{percentShareBrand}%");
                        columnBrandDetail = columnBrandDetail + 2;

                    }

                    var listBrandTopSelectName = listBrandSelect.Select(c => c.Brand_Name);
                    columnBrandDetail++;
                    
                    foreach(var itemBrandLoreal in brandLorealList)
                    {
                        for(int i = 0; i < itemGroup.brandDetail.Count(); i++)
                        {
                            var brandNotTopDetail = itemGroup.brandDetail[i];
                            if (brandNotTopDetail.Brand_Name == itemBrandLoreal 
                                && !listBrandTopSelectName.Contains(brandNotTopDetail.Brand_Name))
                            {
                                worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Value = $"{itemBrandLoreal} [#{i+1}]";          
                                worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                worksheet.Range(worksheet.Cell(rowData, columnBrandDetail), worksheet.Cell(rowData, columnBrandDetail + 1)).Merge();

                                var percentShareBrand = Math.Round((brandNotTopDetail.Amount_Sales.GetValueOrDefault() / itemGroup.sumStore) * 100, 2);
                                worksheet.Cell(rowData + 1, columnBrandDetail).SetValue($"{percentShareBrand}%");

                                if (storeCompare != null)
                                {
                                    var brandCompare = storeCompare.brandDetail.FirstOrDefault(c => c.Brand_ID == brandNotTopDetail.Brand_ID);
                                    var percentGrowthBrand = Math.Round(brandCompare != null ? ((brandNotTopDetail.Amount_Sales.GetValueOrDefault() / brandCompare.Amount_Sales.GetValueOrDefault()) - 1) * 100 : -100, 2);
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
                           sumTotalBrand = e.Sum(d => d.Amount_Sales.GetValueOrDefault())
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
                         sumTotalBrand = e.Sum(d => d.Amount_Sales.GetValueOrDefault()),
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
                        sumTotalBrand = e.Sum(d => d.Amount_Sales.GetValueOrDefault()),
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
                for (int i = 0; i < request.brandRankEnd; i++)
                {
                    var brandDetail = groupBrandDetail[i];
                    var brandCompare = groupBrandDetailCompare.FirstOrDefault(c => c.brandID == brandDetail.brandID);

                    var percentShareBrand = Math.Round((brandDetail.sumTotalBrand / sumAllStore) * 100, 2);
                    worksheet.Cell(rowData + 1, columnBrandTotal).SetValue($"{percentShareBrand}%");

                    if (brandCompare != null)
                    {
                        var percentGrowthBrand = Math.Round(brandCompare != null ? ((brandDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
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
                        var percentGrowthBrand = Math.Round(brandCompare != null ? ((brandDetail.sumTotalBrand / brandCompare.sumTotalBrand) - 1) * 100 : -100, 2);
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
                #endregion


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
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
