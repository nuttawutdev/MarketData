using AspNetCore.Reporting;
using ClosedXML.Excel;
using MarketData.Helper;
using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Request.Approve;
using MarketData.Model.Response;
using MarketData.Model.Response.Approve;
using MarketData.Model.Response.KeyIn;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MarketData.Helper.Utility;

namespace MarketData.Processes.Processes
{
    public class ApproveProcess
    {
        private readonly Repository repository;

        public ApproveProcess(Repository repository)
        {
            this.repository = repository;
        }

        public GetApproveKeyInListResponse GetApproveKeyInList()
        {
            GetApproveKeyInListResponse response = new GetApproveKeyInListResponse();

            try
            {
                var approveKeyInData = repository.approve.GetApproveKeyInData();

                if (approveKeyInData.Any())
                {
                    response.data = approveKeyInData.OrderByDescending(c => c.year).ThenByDescending(c => c.month).ThenByDescending(c => c.week).ThenByDescending(c => c.dateApprove).ToList(); ;
                }
                else
                {
                    response.data = new List<ApproveKeyInData>();
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public GetApproveKeyInOptionResponse GetApproveKeyInOption()
        {
            GetApproveKeyInOptionResponse response = new GetApproveKeyInOptionResponse();

            try
            {
                var getDepartmentStoreResponse = repository.masterData.GetDepartmentStoreList().Where(c => c.active);
                var getRetailerResponse = repository.masterData.GetRetailerGroupList().Where(c => c.Active_Flag);
                var getBrandResponse = repository.masterData.GetBrandList().Where(c => c.active);
                var getChannelResponse = repository.masterData.GetDistributionChannelList().Where(c => c.Active_Flag);
                response.channel = getChannelResponse.Select(c => new DistributionChannelData
                {
                    distributionChannelID = c.Distribution_Channel_ID,
                    distributionChannelName = c.Distribution_Channel_Name
                }).ToList();
                response.departmentStore = getDepartmentStoreResponse.ToList();
                response.brand = getBrandResponse.ToList();
                response.retailerGroup = getRetailerResponse.Select(c => new RetailerGroupData
                {
                    retailerGroupID = c.Retailer_Group_ID,
                    retailerGroupName = c.Retailer_Group_Name
                }).ToList();

                List<string> yearList = new List<string>();
                string currentYear = Utility.GetDateNowThai().Year.ToString();

                yearList.Add(currentYear);

                var approveKeyInData = repository.approve.GetApproveKeyInBy(c => c.ID != Guid.Empty);

                var baKeyInListID = approveKeyInData.Select(c => c.BAKeyIn_ID);
                var baKeyInDataApprove = repository.baKeyIn.GetBAKeyInBy(e => baKeyInListID.Contains(e.ID)).Where(c => c.Year != currentYear);
                var olldYearList = baKeyInDataApprove.Select(r => r.Year);

                if (olldYearList.Any())
                {
                    yearList.AddRange(olldYearList);
                }

                response.year = yearList;
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public GetApproveKeyInDetailResponse GetApproveKeyInDetail(Guid approveKeyInID)
        {
            GetApproveKeyInDetailResponse response = new GetApproveKeyInDetailResponse();

            try
            {
                var keyInData = repository.approve.FindApproveKeyInBy(c => c.ID == approveKeyInID);

                var approveData = repository.approve.FindApproveKeyInBy(c => c.ID == approveKeyInID);
                var BAKeyInData = repository.baKeyIn.FindBAKeyInBy(c => c.ID == keyInData.BAKeyIn_ID);

                List<BAKeyInDetailData> baKeyInList = new List<BAKeyInDetailData>();

                baKeyInList = repository.approve.GetApproveKeyInDetailBy(c => c.ApproveKeyIn_ID == approveKeyInID);

                if (!baKeyInList.Any())
                {
                    baKeyInList = repository.baKeyIn.GetBAKeyInDetailBy(c => c.BAKeyIn_ID == BAKeyInData.ID);

                    baKeyInList = baKeyInList
                      .GroupBy(c => new { c.brandID, c.departmentStoreID, c.channelID, c.amountSale })
                      .Select(g => g.FirstOrDefault())
                      .ToList();

                    baKeyInList = GroupBAKeyInDetailData(baKeyInList);
                }

                string previousYear = (Int32.Parse(BAKeyInData.Year) - 1).ToString();

                var adjustDataPreviousYearWeek4 = repository.adjust.FindAdjustDataBy(
                       c => c.Year == previousYear
                       && c.Month == BAKeyInData.Month
                       && c.Week == "4"
                       && c.DistributionChannel_ID == BAKeyInData.DistributionChannel_ID
                       && c.DepartmentStore_ID == BAKeyInData.DepartmentStore_ID
                       && c.Universe == BAKeyInData.Universe
                       && c.RetailerGroup_ID == BAKeyInData.RetailerGroup_ID);



                if (adjustDataPreviousYearWeek4 != null)
                {
                    var adjustDetailPreviousYearList = repository.adjust.GetAdjustDataDetaillBy(c => c.AdjustData_ID == adjustDataPreviousYearWeek4.ID);

                    foreach (var itemBADetail in baKeyInList)
                    {
                        var adjustDataPreviousYear = adjustDetailPreviousYearList
                            .Where(p => p.Brand_ID == itemBADetail.brandID).OrderByDescending(e => e.Adjust_AmountSale).ToList();

                        if (adjustDataPreviousYear != null && adjustDataPreviousYear.Any())
                        {
                            itemBADetail.amountSalePreviousYear = adjustDataPreviousYear.Sum(c => c.Adjust_AmountSale);
                        }
                    }
                }

                var brandBAData = repository.masterData.FindBrandBy(c => c.Brand_ID == BAKeyInData.Brand_ID);
                var departmentStoreData = repository.masterData.FindDepartmentStoreBy(c => c.Department_Store_ID == BAKeyInData.DepartmentStore_ID);
                var retailerGroupData = repository.masterData.FindRetailerGroupBy(c => c.Retailer_Group_ID == departmentStoreData.Retailer_Group_ID);
                var channelBAData = repository.masterData.FindDistributionChannelBy(c => c.Distribution_Channel_ID == departmentStoreData.Distribution_Channel_ID);

                response.baRemark = approveData.BA_Remark;
                response.remark = approveData.Remark;
                response.universe = BAKeyInData.Universe;
                response.status = repository.masterData.GetApproveKeyInStatusBy(c => c.ID == approveData.Status_ID)?.Status_Name;
                response.brand = brandBAData?.Brand_Name;
                response.departmentStore = departmentStoreData?.Department_Store_Name;
                response.retailerGroup = retailerGroupData?.Retailer_Group_Name;
                response.channel = channelBAData?.Distribution_Channel_Name;
                response.year = BAKeyInData.Year;
                response.month = Enum.GetName(typeof(MonthEnum), Int32.Parse(BAKeyInData.Month));
                response.week = BAKeyInData.Week;

                if (BAKeyInData.Year == GetDateNowThai().Year.ToString())
                {
                    response.data = baKeyInList
                       .Where(e => e.amountSalePreviousYear > 0
                       || e.counterCreateDate.GetValueOrDefault().Year == GetDateNowThai().Year
                       || e.alwayShow)
                       .OrderBy(c => c.brandName).ToList();
                }
                else
                {
                    response.data = baKeyInList
                    .Where(e => e.amountSalePreviousYear > 0)
                    .OrderBy(c => c.brandName).ToList();
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> ApproveKeyInData(ApproveKeyInDataRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var approveStatus = repository.masterData.GetApproveKeyInStatusBy(c => c.Status_Name == "Approve");
                var approveData = repository.approve.FindApproveKeyInBy(c => c.ID == request.approveKeyInID);

                var updateBAKeyInResult = await repository.baKeyIn.ApproveBAKeyIn(approveData.BAKeyIn_ID, request.userID);

                if (updateBAKeyInResult)
                {
                    approveData.Status_ID = approveStatus.ID;
                    approveData.Remark = request.remark;
                    approveData.Action_By = request.userID;
                    approveData.BA_Remark = request.baRemark;
                    approveData.Action_Date = Utility.GetDateNowThai();

                    var updateApproveResult = await repository.approve.UpdateApproveKeyInData(approveData);

                    if (updateApproveResult)
                    {
                        var baKeyInDetail = repository.baKeyIn.GetBAKeyInDetailListData(c => c.BAKeyIn_ID == approveData.BAKeyIn_ID);
                        List<TTApproveKeyInDetail> approveKeyInDetail = baKeyInDetail.Select(c => new TTApproveKeyInDetail
                        {
                            ID = Guid.NewGuid(),
                            DepartmentStore_ID = c.DepartmentStore_ID,
                            DistributionChannel_ID = c.DistributionChannel_ID,
                            FG = c.FG,
                            MU = c.MU,
                            OT = c.OT,
                            SK = c.SK,
                            Amount_Sales = c.Amount_Sales,
                            ApproveKeyIn_ID = request.approveKeyInID,
                            Brand_ID = c.Brand_ID,
                            Counter_ID = c.Counter_ID,
                            Month = c.Month,
                            Year = c.Year,
                            Week = c.Week,
                            Rank = c.Rank,
                            Remark = c.Remark,
                            Whole_Sales = c.Whole_Sales,
                            Created_By = request.userID,
                            Created_Date = Utility.GetDateNowThai()
                        }).ToList();

                        var insertApproveDetailResult = await repository.approve.InsertApproveKeyInDetail(approveKeyInDetail);

                        if (insertApproveDetailResult)
                        {
                            var baKeyInData = repository.baKeyIn.FindBAKeyInBy(c => c.ID == approveData.BAKeyIn_ID);
                            var adjustData = repository.adjust.FindAdjustDataBy(
                                c => c.Year == baKeyInData.Year
                                && c.Month == baKeyInData.Month
                                && c.Week == baKeyInData.Week
                                && c.DistributionChannel_ID == baKeyInData.DistributionChannel_ID
                                && c.DepartmentStore_ID == baKeyInData.DepartmentStore_ID
                                && c.RetailerGroup_ID == baKeyInData.RetailerGroup_ID
                                && c.Universe == baKeyInData.Universe);

                            var adjustStatusAdjusted = repository.masterData.GetAdjustStatusBy(e => e.Status_Name == "Adjusted");

                            if (adjustData != null && adjustData.Status_ID != adjustStatusAdjusted?.ID)
                            {
                                await repository.adjust.RemoveAllAdjustDetailByID(adjustData.ID);
                                await repository.adjust.RemoveAllAdjustBrandDetailByID(adjustData.ID);
                            }

                            response.isSuccess = true;
                        }
                    }
                    else
                    {
                        response.isSuccess = false;
                    }
                }
                else
                {
                    response.isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> RejectKeyInData(RejectKeyInDataRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var rejectStatus = repository.masterData.GetApproveKeyInStatusBy(c => c.Status_Name == "Reject");
                var approveData = repository.approve.FindApproveKeyInBy(c => c.ID == request.approveKeyInID);

                var updateBAKeyInResult = await repository.baKeyIn.RejectBAKeyIn(approveData.BAKeyIn_ID, request.userID);

                if (updateBAKeyInResult)
                {
                    approveData.Status_ID = rejectStatus.ID;
                    approveData.Remark = request.remark;
                    approveData.Action_By = request.userID;
                    approveData.Action_Date = Utility.GetDateNowThai();
                    approveData.BA_Remark = request.baRemark;

                    var updateApproveResult = await repository.approve.UpdateApproveKeyInData(approveData);

                    if (updateApproveResult)
                    {
                        var approveDetail = repository.approve.GetApproveKeyInDetail(c => c.ApproveKeyIn_ID == approveData.ID);

                        if (approveDetail.Any())
                        {
                            response.isSuccess = true;
                        }
                        else
                        {
                            var baKeyInDetail = repository.baKeyIn.GetBAKeyInDetailListData(c => c.BAKeyIn_ID == approveData.BAKeyIn_ID);

                            List<TTApproveKeyInDetail> approveKeyInDetail = baKeyInDetail.Select(c => new TTApproveKeyInDetail
                            {
                                ID = Guid.NewGuid(),
                                DepartmentStore_ID = c.DepartmentStore_ID,
                                DistributionChannel_ID = c.DistributionChannel_ID,
                                FG = c.FG,
                                MU = c.MU,
                                OT = c.OT,
                                SK = c.SK,
                                Amount_Sales = c.Amount_Sales,
                                ApproveKeyIn_ID = request.approveKeyInID,
                                Brand_ID = c.Brand_ID,
                                Counter_ID = c.Counter_ID,
                                Month = c.Month,
                                Year = c.Year,
                                Week = c.Week,
                                Rank = c.Rank,
                                Remark = c.Remark,
                                Whole_Sales = c.Whole_Sales,
                                Created_By = request.userID,
                                Created_Date = Utility.GetDateNowThai()
                            }).ToList();


                            response.isSuccess = await repository.approve.InsertApproveKeyInDetail(approveKeyInDetail);
                        }

                    }
                    else
                    {
                        response.isSuccess = false;
                    }
                }
                else
                {
                    response.isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public byte[] Export()
        {
            try
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

                    //worksheet.Cell(currentRow, 1).Value = "วันที่";
                    //worksheet.Cell(currentRow, 2).Value = "Action";
                    //worksheet.Cell(currentRow, 3).Value = "Employee Name";
                    //worksheet.Cell(currentRow, 4).Value = "Product Heat Number";
                    //worksheet.Cell(currentRow, 5).Value = "Cert Number";
                    //worksheet.Cell(currentRow, 6).Value = "Product Name";
                    //worksheet.Cell(currentRow, 7).Value = "Product Type";
                    //worksheet.Cell(currentRow, 8).Value = "Product Group";
                    //worksheet.Cell(currentRow, 9).Value = "เส้นผ่านศูนย์กลางเหล็กกลม (mm)";
                    //worksheet.Cell(currentRow, 10).Value = "ความยาวเหล็กกลม (mm)";
                    //worksheet.Cell(currentRow, 11).Value = "ความหนาเหล็กแบน (mm)";
                    //worksheet.Cell(currentRow, 12).Value = "ความกว้าง (mm)";
                    //worksheet.Cell(currentRow, 13).Value = "ขนาดเหล็กที่ตัด (mm)";
                    //worksheet.Cell(currentRow, 14).Value = "ขนาดเหล็กที่เหลือ (mm)";
                    //worksheet.Cell(currentRow, 15).Value = "ความยาวเหล็ก (mm)";
                    //worksheet.Cell(currentRow, 16).Value = "จำนวน";
                    //worksheet.Cell(currentRow, 17).Value = "น้ำหนักจริง (kg)";
                    //worksheet.Cell(currentRow, 18).Value = "น้ำหนักต่อชิ้น (kg)";
                    //worksheet.Cell(currentRow, 19).Value = "น้ำหนัก (kg)";
                    //worksheet.Cell(currentRow, 20).Value = "ราคาต่อกิโลกรัม";
                    //worksheet.Cell(currentRow, 21).Value = "ราคาต่อชิ้น";
                    //worksheet.Cell(currentRow, 22).Value = "x_asis";
                    //worksheet.Cell(currentRow, 23).Value = "y_asis";
                    //worksheet.Cell(currentRow, 24).Value = "z_asis";
                    //worksheet.Cell(currentRow, 25).Value = "คลังสินค้า";
                    //worksheet.Cell(currentRow, 26).Value = "โซนวางสินค้า";
                    //worksheet.Cell(currentRow, 27).Value = "ล็อค";
                    //worksheet.Cell(currentRow, 28).Value = "แถว";

                    //foreach (var itemTransaction in productHistoryTransaction)
                    //{
                    //    string actionName = string.Empty;

                    //    if (itemTransaction.action == "C")
                    //    {
                    //        actionName = "Insert";
                    //    }
                    //    else if (itemTransaction.action == "U")
                    //    {
                    //        actionName = "Update";
                    //    }
                    //    else if (itemTransaction.action == "D")
                    //    {
                    //        actionName = "Delete";
                    //    }

                    //    currentRow++;
                    //    worksheet.Cell(currentRow, 1).Value = itemTransaction.createDate.ToString("dd/MM/yyyy HH:mm:ss");
                    //    worksheet.Cell(currentRow, 2).Value = actionName;
                    //    worksheet.Cell(currentRow, 3).Value = itemTransaction.createBy;
                    //    worksheet.Cell(currentRow, 4).Value = !string.IsNullOrWhiteSpace(itemTransaction.heatNumber) ? itemTransaction.heatNumber : "-";
                    //    worksheet.Cell(currentRow, 5).Value = !string.IsNullOrWhiteSpace(itemTransaction.certificateNumber) ? itemTransaction.certificateNumber : "-";
                    //    worksheet.Cell(currentRow, 6).Value = !string.IsNullOrWhiteSpace(itemTransaction.productName) ? itemTransaction.productName : "-";
                    //    worksheet.Cell(currentRow, 7).Value = !string.IsNullOrWhiteSpace(itemTransaction.productTypeName) ? itemTransaction.productTypeName : "-";
                    //    worksheet.Cell(currentRow, 8).Value = !string.IsNullOrWhiteSpace(itemTransaction.productGroupName) ? itemTransaction.productGroupName : "-";
                    //    worksheet.Cell(currentRow, 9).Value = itemTransaction.dia.HasValue ? itemTransaction.dia : 0;
                    //    worksheet.Cell(currentRow, 10).Value = itemTransaction.diaLenght.HasValue ? itemTransaction.diaLenght : 0;
                    //    worksheet.Cell(currentRow, 11).Value = itemTransaction.thickness.HasValue ? itemTransaction.thickness : 0;
                    //    worksheet.Cell(currentRow, 12).Value = itemTransaction.width.HasValue ? itemTransaction.width : 0;
                    //    worksheet.Cell(currentRow, 13).Value = itemTransaction.steelLengthCutting.HasValue ? itemTransaction.steelLengthCutting : 0;
                    //    worksheet.Cell(currentRow, 14).Value = itemTransaction.steelLengthLeft.HasValue ? itemTransaction.steelLengthLeft : 0;
                    //    worksheet.Cell(currentRow, 15).Value = itemTransaction.steelLength.HasValue ? itemTransaction.steelLength : 0;
                    //    worksheet.Cell(currentRow, 16).Value = itemTransaction.qty.HasValue && itemTransaction.qty > 0 ? itemTransaction.qty : 0;
                    //    worksheet.Cell(currentRow, 17).Value = itemTransaction.actualWeight.HasValue ? itemTransaction.actualWeight : 0;
                    //    worksheet.Cell(currentRow, 18).Value = itemTransaction.kg_pc.HasValue ? itemTransaction.kg_pc : 0;
                    //    worksheet.Cell(currentRow, 19).Value = itemTransaction.weight.HasValue ? itemTransaction.width : 0;
                    //    worksheet.Cell(currentRow, 20).Value = itemTransaction.price_kg.HasValue ? itemTransaction.price_kg : 0;
                    //    worksheet.Cell(currentRow, 21).Value = itemTransaction.price_pc.HasValue ? itemTransaction.price_pc : 0;
                    //    worksheet.Cell(currentRow, 22).Value = itemTransaction.x_asis.HasValue ? itemTransaction.x_asis : 0;
                    //    worksheet.Cell(currentRow, 23).Value = itemTransaction.y_asis.HasValue ? itemTransaction.y_asis : 0;
                    //    worksheet.Cell(currentRow, 24).Value = itemTransaction.z_asis.HasValue ? itemTransaction.z_asis : 0;
                    //    worksheet.Cell(currentRow, 25).Value = !string.IsNullOrWhiteSpace(itemTransaction.warehouseName) ? itemTransaction.warehouseName : "-";
                    //    worksheet.Cell(currentRow, 26).Value = !string.IsNullOrWhiteSpace(itemTransaction.zone) ? itemTransaction.zone : "-";
                    //    worksheet.Cell(currentRow, 27).Value = !string.IsNullOrWhiteSpace(itemTransaction.column) ? itemTransaction.column : "-";
                    //    worksheet.Cell(currentRow, 28).Value = !string.IsNullOrWhiteSpace(itemTransaction.row) ? itemTransaction.row : "-";

                    //}


                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return content;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private List<BAKeyInDetailData> GroupBAKeyInDetailData(List<BAKeyInDetailData> listBAKeyInDetail)
        {

            List<BAKeyInDetailData> BAKeyInDetailList = new List<BAKeyInDetailData>();

            foreach (var itemBaKeyin in listBAKeyInDetail.OrderByDescending(c => c.amountSale))
            {
                if (BAKeyInDetailList.FirstOrDefault(c => c.brandID == itemBaKeyin.brandID) == null)
                {
                    var itemDuplicate = listBAKeyInDetail.FirstOrDefault(d => d.brandID == itemBaKeyin.brandID && d.ID != itemBaKeyin.ID);

                    if (itemDuplicate != null)
                    {
                        if (itemBaKeyin.amountSale.HasValue || itemDuplicate.amountSale.HasValue)
                        {
                            itemBaKeyin.amountSale = itemBaKeyin.amountSale.GetValueOrDefault() + itemDuplicate.amountSale.GetValueOrDefault();
                        }

                        if (itemBaKeyin.wholeSale.HasValue || itemDuplicate.wholeSale.HasValue)
                        {
                            itemBaKeyin.wholeSale = itemBaKeyin.wholeSale.GetValueOrDefault() + itemDuplicate.wholeSale.GetValueOrDefault();
                        }

                        itemBaKeyin.remark = !itemBaKeyin.amountSale.HasValue ? !string.IsNullOrWhiteSpace(itemBaKeyin.remark) ? itemBaKeyin.remark : itemDuplicate.remark : null;
                        BAKeyInDetailList.Add(itemBaKeyin);
                    }
                    else
                    {
                        BAKeyInDetailList.Add(itemBaKeyin);
                    }
                }
            }

            return BAKeyInDetailList;
        }
    }
}
