using ClosedXML.Excel;
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

                    if (brandRankingData.Any())
                    {
                        var brandRankingStartYear = brandRankingData.Where(w => w.Sales_Year == request.startYear);
                        var groupStoreStartYear = brandRankingStartYear.GroupBy(c => c.Store_Id).Select(e => new GroupBrandRanking
                        {
                            storeID = e.Key,
                            sumStore = e.Sum(d => d.Amount_Sales.GetValueOrDefault()),
                            brandDetail = e.OrderByDescending(s=>s.Amount_Sales).ToList()
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

                        var groupStoreCompareYear = brandRankingCompareYear
                            .GroupBy(c => c.Store_Id).Select(e => new GroupBrandRanking
                            {
                                storeID = e.Key,
                                sumStore = e.Sum(d => d.Amount_Sales.GetValueOrDefault()),
                                brandDetail = e.OrderByDescending(s => s.Amount_Sales).ToList()
                            }).OrderByDescending(s => s.sumStore).ToList();
                    }
                    else
                    {

                    }

                }
                // YTD
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        private byte[] GenerateReportStoreMarketShareZone()
        {
            using (var workbook = new XLWorkbook())
            {
                int currentRow = 1;
                Color yellowHead = Color.FromArgb(250, 250, 181);
                XLColor yellowXL = XLColor.FromArgb(yellowHead.A, yellowHead.R, yellowHead.G, yellowHead.B);

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
        public decimal sumStore { get; set; }
        public List<Brand_Ranking> brandDetail { get; set; }
    }
}
