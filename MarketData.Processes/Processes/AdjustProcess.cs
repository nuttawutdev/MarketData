using MarketData.Helper;
using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Request.Adjust;
using MarketData.Model.Response;
using MarketData.Model.Response.AdjustData;
using MarketData.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MarketData.Helper.Utility;

namespace MarketData.Processes.Processes
{
    public class AdjustProcess
    {
        private readonly Repository repository;

        public AdjustProcess(Repository repository)
        {
            this.repository = repository;
        }

        public GetAdjustDataListModel GetAdjustDataList(GetAdjustListRequest request)
        {
            GetAdjustDataListModel response = new GetAdjustDataListModel();

            try
            {
                var departmentStoreList = repository.masterData.GetDepartmentStoreList().Where(e => e.active);
                var departmentStoreByFilter = departmentStoreList.Where(
                    c => (!request.departmentStoreID.HasValue || (request.departmentStoreID.HasValue && request.departmentStoreID == c.departmentStoreID))
                    && (!request.retailerGroupID.HasValue || (request.retailerGroupID.HasValue && request.retailerGroupID == c.retailerGroupID))
                    && (!request.distributionChannelID.HasValue || (request.distributionChannelID.HasValue && request.distributionChannelID == c.distributionChannelID)));

                var departmentSoteIDList = departmentStoreByFilter.Select(c => c.departmentStoreID);
                var counterByDepartmentStoreList = repository.masterData.GetCounterListBy(e =>
                    departmentSoteIDList.Contains(e.Department_Store_ID)
                    && e.Active_Flag && e.Delete_Flag != true
                    && (!request.distributionChannelID.HasValue || (request.distributionChannelID.HasValue && request.distributionChannelID == e.Distribution_Channel_ID)));

                var allBrandByCounter = counterByDepartmentStoreList.GroupBy(e => e.Brand_ID).Select(c => c.Key);

                var onlyBrandLorel = repository.masterData.GetBrandListLoreal(c => allBrandByCounter.Contains(c.Brand_ID)).Where(e => e.universe == request.universe && e.active);
                var allApproveKeyInData = repository.approve.GetApproveKeyInData();

                var approveKeyInDataFilter = allApproveKeyInData.Where(
                    c => c.year == request.year && c.month == request.month
                    && c.week == request.week && c.universe == request.universe);

                var adjustStatus = repository.masterData.GetAdjustStatusList();
                var adjustStatusPending = repository.masterData.GetAdjustStatusBy(c => c.Status_Name == "Pending");
                var listAdjustData = repository.adjust.GetAdjustDataBy(
                    c => c.Year == request.year
                    && c.Month == request.month
                    && c.Week == request.week
                    && c.Universe == request.universe);

                List<AdjustData> adjustDataList = new List<AdjustData>();

                foreach (var itemDepartment in departmentStoreByFilter)
                {
                    var adjustStatusData = listAdjustData.FirstOrDefault(
                        c => c.DistributionChannel_ID == itemDepartment.distributionChannelID
                        && c.RetailerGroup_ID == itemDepartment.retailerGroupID
                        && c.DepartmentStore_ID == itemDepartment.departmentStoreID);

                    string statusName = string.Empty;
                    Guid statusID;

                    if (adjustStatusData != null)
                    {
                        statusID = adjustStatusData.Status_ID;
                        statusName = adjustStatus.FirstOrDefault(c => c.ID == adjustStatusData.Status_ID).Status_Name;
                    }
                    else
                    {
                        statusID = adjustStatusPending.ID;
                        statusName = adjustStatusPending.Status_Name;
                    }

                    AdjustData adjustData = new AdjustData
                    {
                        retailerGroupID = itemDepartment.retailerGroupID,
                        retailerGroupName = itemDepartment.retailerGroupName,
                        week = request.week,
                        month = request.month,
                        year = request.year,
                        departmentStoreID = itemDepartment.departmentStoreID,
                        departmentStoreName = itemDepartment.departmentStoreName,
                        distributionChannelID = itemDepartment.distributionChannelID,
                        distributionChannelName = itemDepartment.distributionChannelName,
                        statusID = statusID,
                        statusName = statusName,
                        brandStatus = new Dictionary<string, string>()
                    };

                    onlyBrandLorel = onlyBrandLorel.Where(c => c.brandTypeName != "Fragrances");

                    foreach (var itemBrandLoreal in onlyBrandLorel)
                    {
                        var brandShortName = !string.IsNullOrWhiteSpace(itemBrandLoreal.brandShortName) ? itemBrandLoreal.brandShortName : itemBrandLoreal.brandName;

                        string statusBrand = string.Empty;

                        var existCounter = counterByDepartmentStoreList.FirstOrDefault(
                           c => c.Distribution_Channel_ID == itemDepartment.distributionChannelID
                           && c.Department_Store_ID == itemDepartment.departmentStoreID
                           && c.Brand_ID == itemBrandLoreal.brandID);

                        if (existCounter != null)
                        {
                            var approveStatus = approveKeyInDataFilter.Where(
                          e => e.brandID == itemBrandLoreal.brandID
                          && e.departmentStoreID == itemDepartment.departmentStoreID
                          && e.distributionChannelID == itemDepartment.distributionChannelID
                          && e.retailerGroupID == itemDepartment.retailerGroupID)
                          .OrderByDescending(c => c.dateApprove).FirstOrDefault();

                            statusBrand = approveStatus?.statusName;
                        }
                        else
                        {
                            statusBrand = "none";
                        }

                        adjustData.brandStatus.Add(brandShortName, statusBrand);
                    }

                    adjustDataList.Add(adjustData);
                }

                if (request.statusID.HasValue)
                {
                    adjustDataList = adjustDataList.Where(c => c.statusID == request.statusID).ToList();

                }

                response.data = adjustDataList;
                response.columnList = onlyBrandLorel.Select(c => !string.IsNullOrWhiteSpace(c.brandShortName) ? c.brandShortName : c.brandName).ToList();
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public GetAdjustOptionResponse GetAdjustOption()
        {
            GetAdjustOptionResponse response = new GetAdjustOptionResponse();

            try
            {
                var getDepartmentStoreResponse = repository.masterData.GetDepartmentStoreListBy(c => c.Active_Flag);
                var getRetailerResponse = repository.masterData.GetRetailerGroupList().Where(c => c.Active_Flag);
                var getBrandResponse = repository.masterData.GetBrandListBy(c => c.Active_Flag);
                var getChannelResponse = repository.masterData.GetDistributionChannelList().Where(c => c.Active_Flag);
                var keyInStatusApprove = repository.masterData.GetKeyInStatusBy(c => c.Status_Name == "Approve");
                var allAdjustData = repository.adjust.GetAdjustDataBy(c => c.ID != Guid.Empty);

                response.channel = getChannelResponse.Select(c => new DistributionChannelData
                {
                    distributionChannelID = c.Distribution_Channel_ID,
                    distributionChannelName = c.Distribution_Channel_Name
                }).ToList();
                response.departmentStore = getDepartmentStoreResponse.Select(c => new DepartmentStoreData
                {
                    departmentStoreID = c.Department_Store_ID,
                    departmentStoreName = c.Department_Store_Name,
                    distributionChannelID = c.Distribution_Channel_ID,
                    retailerGroupID = c.Retailer_Group_ID
                }).OrderBy(r => r.departmentStoreName).ToList();
                response.brand = getBrandResponse.Select(c => new BrandData
                {
                    brandID = c.Brand_ID,
                    brandName = c.Brand_Name
                }).OrderBy(r => r.brandName).ToList();
                response.retailerGroup = getRetailerResponse.Select(c => new RetailerGroupData
                {
                    retailerGroupID = c.Retailer_Group_ID,
                    retailerGroupName = c.Retailer_Group_Name
                }).OrderBy(r => r.retailerGroupName).ToList();

                List<string> yearList = new List<string>();
                string currentYear = Utility.GetDateNowThai().Year.ToString();

                yearList.Add(currentYear);

                var keyInApproveData = repository.baKeyIn.GetBAKeyInBy(c => c.KeyIn_Status_ID == keyInStatusApprove.ID);

                var olldYearListApprove = keyInApproveData.Where(e => e.Year != currentYear).GroupBy(c => c.Year).Select(s => s.Key).ToList();
                var oldYearListAdjust = allAdjustData.Where(e => e.Year != currentYear).GroupBy(c => c.Year).Select(s => s.Key).ToList();
                olldYearListApprove.AddRange(oldYearListAdjust);

                var olldYearList = olldYearListApprove.GroupBy(c => c).SelectMany(e => e).OrderByDescending(t => t);
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

        public GetAdjustDetailResponse GetAdjustDataDetail(Guid adjustDataID)
        {
            GetAdjustDetailResponse response = new GetAdjustDetailResponse();

            try
            {
                bool isAdjusted = false;
                var adjustStatusAdjusted = repository.masterData.GetAdjustStatusBy(c => c.Status_Name == "Adjusted");

                var adjustData = repository.adjust.FindAdjustDataBy(c => c.ID == adjustDataID);
                var adjustDetail = repository.adjust.GetAdjustDataDetaillBy(t => t.AdjustData_ID == adjustData.ID);

                if (adjustData.Status_ID == adjustStatusAdjusted.ID)
                {
                    isAdjusted = true;
                }

                var counterList = repository.masterData.GetCounterListBy(
                       e => e.Distribution_Channel_ID == adjustData.DistributionChannel_ID
                       && e.Department_Store_ID == adjustData.DepartmentStore_ID
                       && e.Active_Flag && e.Delete_Flag != true);

                var allBrandByCounterData = counterList.GroupBy(e => e.Brand_ID).Select(c => c.Key);
                var onlyBrandLorel = repository.masterData.GetBrandListLoreal(c => allBrandByCounterData.Contains(c.Brand_ID)).Where(e => e.universe == adjustData.Universe && e.active);
                var onlyBrandLorealID = onlyBrandLorel.Select(e => e.brandID);

                if (adjustData.Week != "4")
                {
                    // Filter Brand Type Fragrances
                    List<TMCounter> listCounterFilterFragrances = new List<TMCounter>();

                    var brandIDCounter = counterList.GroupBy(c => c.Brand_ID).Select(e => e.Key);
                    var brandDataList = repository.masterData.GetBrandListBy(c => brandIDCounter.Contains(c.Brand_ID) && c.Universe == adjustData.Universe);
                    var brandTypeList = repository.masterData.GetBrandTypeList().Where(e => e.Active_Flag);

                    foreach (var itemCounter in counterList)
                    {
                        var brandData = brandDataList.FirstOrDefault(c => c.Brand_ID == itemCounter.Brand_ID);

                        if (brandData != null)
                        {
                            var brandTypeData = brandTypeList.FirstOrDefault(c => c.Brand_Type_ID == brandData.Brand_Type_ID);

                            if (brandTypeData?.Brand_Type_Name != "Fragrances" || itemCounter.Alway_Show_Current_Year == true)
                            {
                                listCounterFilterFragrances.Add(itemCounter);
                            }
                        }
                    }

                    counterList = listCounterFilterFragrances;
                }

                #region Get Brand Data

                var allBrandByCounter = counterList.GroupBy(e => e.Brand_ID).Select(c => c.Key);
                var allBrandByCounterListData = repository.masterData.GetBrandListBy(c => allBrandByCounter.Contains(c.Brand_ID) && c.Universe == adjustData.Universe);

                #endregion

                #region Get BAKeyIn Data
                var keyInStatusApprove = repository.masterData.GetKeyInStatusBy(e => e.Status_Name == "Approve");

                // BA Key-in ที่ถูก Approve แล้วเฉพาะ Counter ของ Brand Loreal
                var baKeyInDataApprove = repository.baKeyIn.GetBAKeyInBy(
                    c => c.Year == adjustData.Year
                    && c.Month == adjustData.Month
                    && c.Week == adjustData.Week
                    && c.DistributionChannel_ID == adjustData.DistributionChannel_ID
                    && c.DepartmentStore_ID == adjustData.DepartmentStore_ID
                    && c.Universe == adjustData.Universe
                    && c.KeyIn_Status_ID == keyInStatusApprove.ID
                    && onlyBrandLorealID.Contains(c.Brand_ID));

                string previousYear = (int.Parse(adjustData.Year) - 1).ToString();

                // BA Key-in ข้อมูลปีที่แล้ว Week 4 ที่ถูก Approve แล้วเฉพาะ Counter ของ Brand Loreal
                var adjustDataPreviousYearWeek4 = repository.adjust.FindAdjustDataBy(
                    c => c.Year == previousYear
                    && c.Month == adjustData.Month
                    && c.Week == "4"
                    && c.DistributionChannel_ID == adjustData.DistributionChannel_ID
                    && c.DepartmentStore_ID == adjustData.DepartmentStore_ID
                    && c.Universe == adjustData.Universe
                    && c.RetailerGroup_ID == adjustData.RetailerGroup_ID
                    && c.Status_ID == adjustStatusAdjusted.ID);

                var adjustDataPreviousYear = repository.adjust.FindAdjustDataBy(
                   c => c.Year == previousYear
                   && c.Month == adjustData.Month
                   && c.Week == adjustData.Week
                   && c.DistributionChannel_ID == adjustData.DistributionChannel_ID
                   && c.DepartmentStore_ID == adjustData.DepartmentStore_ID
                   && c.Universe == adjustData.Universe
                   && c.RetailerGroup_ID == adjustData.RetailerGroup_ID
                   && c.Status_ID == adjustStatusAdjusted.ID);

                var baKeyInIDApprove = baKeyInDataApprove.Select(t => t.ID);

                var baKeyInDetailApprove = repository.baKeyIn.GetBAKeyInDetailListData(p => baKeyInIDApprove.Contains(p.BAKeyIn_ID));

                List<TTAdjustDataDetail> adjustDetailPreviousYear = new List<TTAdjustDataDetail>();
                List<TTAdjustDataDetail> adjustDetailPreviousYearWeek4 = new List<TTAdjustDataDetail>();
                if (adjustDataPreviousYear != null)
                {
                    adjustDetailPreviousYear = repository.adjust.GetAdjustDataDetaillBy(p => p.AdjustData_ID == adjustDataPreviousYear.ID);
                }

                if (adjustDataPreviousYearWeek4 != null)
                {
                    adjustDetailPreviousYearWeek4 = repository.adjust.GetAdjustDataDetaillBy(p => p.AdjustData_ID == adjustDataPreviousYearWeek4.ID);
                }
                #endregion

                var adminKeyInDetailData = repository.adminKeyIn.GetAdminKeyInDetailBy(
                   c => c.Year == adjustData.Year
                   && c.Month == adjustData.Month
                   && c.Week == adjustData.Week
                   && c.DistributionChannel_ID == adjustData.DistributionChannel_ID
                   && c.DepartmentStore_ID == adjustData.DepartmentStore_ID
                   && c.Universe == adjustData.Universe);

                List<AdjustDetailData> listAdjustDetailData = new List<AdjustDetailData>();

                foreach (var itemBrandInDepartment in allBrandByCounterListData)
                {
                    decimal? adminAmountSale = null;
                    decimal? amountPreviousYear = null;
                    decimal? amountPreviousYearWeek = null;
                    decimal? adjustAmountSale = null;
                    decimal? adjustWholeSale = null;
                    decimal? sk = null;
                    decimal? mu = null;
                    decimal? fg = null;
                    decimal? ot = null;
                    string remark = null;

                    // ค่าที่ผ่านการ Adjust เมื่อปีที่แล้ว Week 4
                    var adjustKeyDataPreviousYearWeek4 = adjustDetailPreviousYearWeek4.Where(
                        c => c.Brand_ID == itemBrandInDepartment.Brand_ID)
                        .OrderByDescending(e => e.Adjust_AmountSale).FirstOrDefault();

                    if (adjustKeyDataPreviousYearWeek4 != null)
                    {
                        amountPreviousYear = adjustKeyDataPreviousYearWeek4.Adjust_AmountSale;
                    }

                    // ค่าที่ผ่านการ Adjust เมื่อปีที่แล้ว Week 4
                    var adjustKeyDataPreviousYear = adjustDetailPreviousYear.Where(
                        c => c.Brand_ID == itemBrandInDepartment.Brand_ID)
                        .OrderByDescending(e => e.Adjust_AmountSale).FirstOrDefault();

                    if (adjustKeyDataPreviousYear != null)
                    {
                        amountPreviousYearWeek = adjustKeyDataPreviousYear.Adjust_AmountSale;
                    }

                    // ค่าที่ Admin กรอก
                    var adminKeyInData = adminKeyInDetailData.FirstOrDefault(a => a.Brand_ID == itemBrandInDepartment.Brand_ID);

                    // ค่าที่ BA ใน Store นั้นกรอกมาและถูก Approve แล้ว
                    var baKeyInBrand = baKeyInDetailApprove.Where(b => b.Brand_ID == itemBrandInDepartment.Brand_ID);

                    // ค่าที่เคยทำการ Save Adjust
                    var adjustBrandData = adjustDetail.FirstOrDefault(e => e.Brand_ID == itemBrandInDepartment.Brand_ID);

                    // ถ้ายังไม่ Submit ยังเอาค่าของ Counter ที่ถูก Approve มาหาค่ามากสุดอยู่ ?
                    if (!isAdjusted)
                    {
                        if (adjustBrandData != null)
                        {
                            adminAmountSale = adjustBrandData.Admin_AmountSale;
                            adjustAmountSale = adjustBrandData.Adjust_AmountSale;
                            adjustWholeSale = adjustBrandData.Adjust_WholeSale;
                            sk = adjustBrandData.SK;
                            mu = adjustBrandData.MU;
                            fg = adjustBrandData.FG;
                            ot = adjustBrandData.OT;
                            remark = adjustBrandData.Remark;
                        }
                        // ถ้า Admin กรอกมาใช้ค่าของ Admin
                        else if (adminKeyInData != null && (adminKeyInData.Amount_Sales.HasValue
                            || adminKeyInData.Whole_Sales.HasValue
                            || adminKeyInData.FG.HasValue
                            || adminKeyInData.MU.HasValue
                            || adminKeyInData.OT.HasValue
                            || adminKeyInData.SK.HasValue
                            || !string.IsNullOrWhiteSpace(adminKeyInData.Remark)))
                        {
                            adminAmountSale = adminKeyInData.Amount_Sales;
                            adjustAmountSale = adminKeyInData.Amount_Sales;
                            adjustWholeSale = adminKeyInData.Whole_Sales;
                            sk = adminKeyInData.SK;
                            mu = adminKeyInData.MU;
                            fg = adminKeyInData.FG;
                            ot = adminKeyInData.OT;
                            remark = adminKeyInData.Remark;
                        }
                        else if (baKeyInBrand.Any())
                        {
                            // มีค่า Amount Sale และ Whole Sale
                            if (baKeyInBrand.Any(c => c.Amount_Sales.HasValue))
                            {
                                // ค่า Amount Sale มากที่สุดของแต่ละ Counter
                                var mostAmountSale = baKeyInBrand.Where(
                                    e => e.Amount_Sales.HasValue)
                                    .OrderByDescending(c => c.Amount_Sales).FirstOrDefault().Amount_Sales;

                                // ค่า Whole Sale มากที่สุดของแต่ละ Counter
                                var mostWholeSale = baKeyInBrand.Where(
                                    e => e.Whole_Sales.HasValue)
                                    .OrderByDescending(c => c.Whole_Sales).FirstOrDefault()?.Whole_Sales;

                                adjustAmountSale = mostAmountSale;
                                adjustWholeSale = mostWholeSale;

                                // หาค่า sk mu fg ot จาก Brand ที่ Rank สูงที่สุดของ Loreal
                                foreach (var itemBrandLoreal in onlyBrandLorel.OrderBy(c => c.lorealBrandRank))
                                {
                                    var brandLorealKeyIn = baKeyInDataApprove.FirstOrDefault(t => t.Brand_ID == itemBrandLoreal.brandID);

                                    if (brandLorealKeyIn != null)
                                    {
                                        var keyInDetailBrandLoreal = baKeyInBrand.FirstOrDefault(r => r.BAKeyIn_ID == brandLorealKeyIn.ID);

                                        if (keyInDetailBrandLoreal.FG.HasValue || keyInDetailBrandLoreal.SK.HasValue
                                            || keyInDetailBrandLoreal.MU.HasValue || keyInDetailBrandLoreal.OT.HasValue)
                                        {
                                            sk = keyInDetailBrandLoreal.SK;
                                            mu = keyInDetailBrandLoreal.MU;
                                            fg = keyInDetailBrandLoreal.FG;
                                            ot = keyInDetailBrandLoreal.OT;
                                            break;
                                        }
                                    }

                                }
                            }
                            else
                            {
                                // หาค่า Remark จาก Brand ที่ Rank สูงที่สุดของ Loreal
                                foreach (var itemBrandLoreal in onlyBrandLorel.OrderBy(c => c.lorealBrandRank))
                                {
                                    var brandLorealKeyIn = baKeyInDataApprove.FirstOrDefault(t => t.Brand_ID == itemBrandLoreal.brandID);

                                    if (brandLorealKeyIn != null)
                                    {
                                        var keyInDetailBrandLoreal = baKeyInBrand.FirstOrDefault(r => r.BAKeyIn_ID == brandLorealKeyIn.ID);

                                        if (!string.IsNullOrWhiteSpace(keyInDetailBrandLoreal.Remark))
                                        {
                                            remark = keyInDetailBrandLoreal.Remark;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // ถ้า Submit ไปแล้วใช้ค่าจากที่เคยทำการ Adjust
                        if (adjustBrandData != null)
                        {
                            adminAmountSale = adjustBrandData.Admin_AmountSale;
                            adjustAmountSale = adjustBrandData.Adjust_AmountSale;
                            adjustWholeSale = adjustBrandData.Adjust_WholeSale;
                            sk = adjustBrandData.SK;
                            mu = adjustBrandData.MU;
                            fg = adjustBrandData.FG;
                            ot = adjustBrandData.OT;
                            remark = adjustBrandData.Remark;
                        }
                    }

                    var counterData = counterList
                        .FirstOrDefault(c => c.Department_Store_ID == adjustData.DepartmentStore_ID
                        && c.Distribution_Channel_ID == adjustData.DistributionChannel_ID
                        && c.Brand_ID == itemBrandInDepartment.Brand_ID);

                    AdjustDetailData adjustDetailData = new AdjustDetailData
                    {
                        brandID = itemBrandInDepartment.Brand_ID,
                        brandColor = itemBrandInDepartment.Brand_Color,
                        year = adjustData.Year,
                        month = adjustData.Month,
                        week = adjustData.Week,
                        brandName = itemBrandInDepartment.Brand_Name,
                        amountPreviousYear = amountPreviousYear,
                        amountPreviousYearWeek = amountPreviousYearWeek,
                        brandKeyInAmount = new Dictionary<string, decimal?>(),
                        brandKeyInRank = new Dictionary<string, string>(),
                        adminAmountSale = adminAmountSale,
                        adjustAmountSale = adjustAmountSale ?? 0,
                        adjustWholeSale = adjustWholeSale ?? 0,
                        sk = sk,
                        mu = mu,
                        fg = fg,
                        ot = ot,
                        remark = remark,
                        counterCreateDate = counterData?.Created_Date,
                        alwayShow = counterData != null ? counterData.Alway_Show_Current_Year.GetValueOrDefault() : false
                    };

                    foreach (var itemBrandLoreal in onlyBrandLorel)
                    {
                        var baBrandKeyIn = baKeyInDataApprove.FirstOrDefault(c => c.Brand_ID == itemBrandLoreal.brandID);
                        var brandName = !string.IsNullOrWhiteSpace(itemBrandLoreal.brandShortName) ? itemBrandLoreal.brandShortName : itemBrandLoreal.brandName;
                        decimal? amountSale = null;
                        string rank = null;

                        if (baBrandKeyIn != null)
                        {
                            var baBrandKeyInDetail = baKeyInDetailApprove.FirstOrDefault(
                              e => e.Brand_ID == itemBrandInDepartment.Brand_ID
                              && e.BAKeyIn_ID == baBrandKeyIn.ID);

                            if (baBrandKeyInDetail != null && baBrandKeyInDetail.Amount_Sales.HasValue && baBrandKeyInDetail.Rank.HasValue)
                            {
                                amountSale = baBrandKeyInDetail.Amount_Sales;
                                rank = baBrandKeyInDetail.Rank.Value.ToString();
                            }
                        }

                        adjustDetailData.brandKeyInAmount.Add(brandName, amountSale);
                        adjustDetailData.brandKeyInRank.Add(brandName, rank);
                    }

                    listAdjustDetailData.Add(adjustDetailData);
                }

                if (adjustData.Year == GetDateNowThai().Year.ToString())
                {
                    listAdjustDetailData = listAdjustDetailData.Where(c => c.amountPreviousYear > 0
                    || c.counterCreateDate.GetValueOrDefault().Year == GetDateNowThai().Year
                    || c.alwayShow).ToList();
                }
                else
                {
                    listAdjustDetailData = listAdjustDetailData.Where(c => c.amountPreviousYear > 0).ToList();
                }

                int rankAdjust = 1;

                foreach (var itemAdjustData in listAdjustDetailData.OrderByDescending(e => e.adjustAmountSale).ThenByDescending(c => c.amountPreviousYear))
                {
                    itemAdjustData.rank = rankAdjust;
                    if (itemAdjustData.amountPreviousYearWeek.HasValue && itemAdjustData.adjustAmountSale.HasValue)
                    {
                        // ((adjustAmountSale - amountPreviousYearWeek) / amountPreviousYearWeek) X 100
                        var adjustAmountSale = itemAdjustData.adjustAmountSale.GetValueOrDefault();
                        var amountPreviousYearWeek = itemAdjustData.amountPreviousYearWeek.GetValueOrDefault();

                        if (amountPreviousYearWeek > 0)
                        {
                            itemAdjustData.percentGrowth = ((adjustAmountSale - amountPreviousYearWeek) / amountPreviousYearWeek) * 100;
                            itemAdjustData.percentGrowth = Math.Round(itemAdjustData.percentGrowth.Value, 2);
                        }
                        else
                        {
                            itemAdjustData.percentGrowth = 0;
                        }

                    }

                    rankAdjust += 1;
                }

                var brandAmountSale = listAdjustDetailData.SelectMany(c => c.brandKeyInAmount).GroupBy(e => e.Key);
                Dictionary<string, decimal?> summaryBrandAmount = new Dictionary<string, decimal?>();

                var listBrandCounterHaveValue = listAdjustDetailData.SelectMany(c => c.brandKeyInAmount).Where(e => e.Value.HasValue).GroupBy(k => k.Key).Select(d => d.Key);

                foreach (var itemAdjustList in listAdjustDetailData)
                {
                    itemAdjustList.brandKeyInAmount = itemAdjustList.brandKeyInAmount.Where(c => listBrandCounterHaveValue.Contains(c.Key)).ToDictionary(c => c.Key, c => c.Value);
                }

                foreach (var itemGroupBrand in brandAmountSale)
                {
                    var summaryAmount = itemGroupBrand.Sum(c => c.Value.GetValueOrDefault());
                    summaryBrandAmount.Add(itemGroupBrand.Key, summaryAmount);
                }

                var departmentData = repository.masterData.FindDepartmentStoreBy(c => c.Department_Store_ID == adjustData.DepartmentStore_ID);
                var retailerGroup = repository.masterData.FindRetailerGroupBy(e => e.Retailer_Group_ID == departmentData.Retailer_Group_ID);
                var channelData = repository.masterData.FindDistributionChannelBy(c => c.Distribution_Channel_ID == adjustData.DistributionChannel_ID);

                List<string> brandColumn = new List<string>();

                foreach (var itemBrandLoreal in listBrandCounterHaveValue)
                {
                    //string brandName = !string.IsNullOrWhiteSpace(itemBrandLoreal.brandShortName) ? itemBrandLoreal.brandShortName : itemBrandLoreal.brandName;

                    brandColumn.Add($"{itemBrandLoreal}-Amt.Sales");
                    brandColumn.Add($"{itemBrandLoreal}-Rank");
                }

                var adjustStatus = repository.masterData.GetAdjustStatusBy(e => e.ID == adjustData.Status_ID);

                response.brandTotalAmount = summaryBrandAmount;
                response.adjustDataID = adjustData.ID;
                response.status = adjustStatus.Status_Name;
                response.brandDataColumn = brandColumn;
                response.year = adjustData.Year;
                response.month = Enum.GetName(typeof(MonthEnum), Int32.Parse(adjustData.Month));
                response.week = adjustData.Week;
                response.channel = channelData.Distribution_Channel_Name;
                response.retailerGroup = retailerGroup.Retailer_Group_Name;
                response.departmentStore = departmentData.Department_Store_Name;
                response.universe = adjustData.Universe;
                response.data = listAdjustDetailData.OrderBy(e => e.rank).ToList();
            }
            catch (Exception ex)
            {
                response.responseError = ex.InnerException?.Message ?? ex.Message;
                string fullPath = "C:\\ERRORLOG";

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath); //Create directory if it doesn't exist
                }

                File.AppendAllText($"{fullPath}\\ERROR.txt",
               $"{ex.InnerException?.Message + "," + ex.Message} {DateTime.Now.ToString()} {Environment.NewLine}");
            }

            return response;
        }

        public async Task<SaveAdjustDetailResponse> CreateAdjustData(GetAdjustDetailRequest request)
        {
            SaveAdjustDetailResponse response = new SaveAdjustDetailResponse();

            try
            {
                var adjustData = repository.adjust.FindAdjustDataBy(
                    c => c.DistributionChannel_ID == request.distributionChannelID
                    && c.DepartmentStore_ID == request.departmentStoreID
                    && c.RetailerGroup_ID == request.retailerGroupID
                    && c.Year == request.year
                    && c.Month == request.month
                    && c.Week == request.week
                    && c.Universe == request.universe);

                if (adjustData != null)
                {
                    response.adjustDataID = adjustData.ID;
                    response.isSuccess = true;
                }
                else
                {
                    var createAdjustDataResponse = await repository.adjust.CreateAdjustData(request);
                    if (createAdjustDataResponse != null)
                    {
                        response.adjustDataID = createAdjustDataResponse.ID;
                        response.isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> SaveAdjustDataDetail(SaveAdjustDataRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                bool updateAdjustDataResult = true;
                var adjustData = repository.adjust.FindAdjustDataBy(c => c.ID == request.adjustDataID);
                var adjustesStatus = repository.masterData.GetAdjustStatusBy(c => c.Status_Name == "Adjusted");

                if (adjustData.Status_ID == adjustesStatus.ID)
                {
                    updateAdjustDataResult = await repository.adjust.UpdateAdjustData(request.adjustDataID, request.userID, adjustesStatus.ID);
                }
                else
                {
                    var inprogressStatus = repository.masterData.GetAdjustStatusBy(c => c.Status_Name == "In-Progress");
                    updateAdjustDataResult = await repository.adjust.UpdateAdjustData(request.adjustDataID, request.userID, inprogressStatus.ID);
                }

                if (updateAdjustDataResult)
                {
                    response = await SaveAdjustData(request);
                }
                else
                {
                    response.isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> SubmitAdjustDataDetail(SaveAdjustDataRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var adjustedStatus = repository.masterData.GetAdjustStatusBy(c => c.Status_Name == "Adjusted");
                var updateAdjustData = await repository.adjust.UpdateAdjustData(request.adjustDataID, request.userID, adjustedStatus.ID);

                if (updateAdjustData)
                {
                    response = await SaveAdjustData(request);
                }
                else
                {
                    response.isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        private async Task<SaveDataResponse> SaveAdjustData(SaveAdjustDataRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var brandList = request.adjustDataDetail.SelectMany(c => c.brandKeyInAmount).GroupBy(g => g.Key).Select(c => c.Key);
                var brandDataList = repository.masterData.GetBrandListBy(e => brandList.Contains(e.Brand_Short_Name) || brandList.Contains(e.Brand_Name));

                await repository.adjust.RemoveAllAdjustDetailByID(request.adjustDataID);
                await repository.adjust.RemoveAllAdjustBrandDetailByID(request.adjustDataID);

                List<TTAdjustDataDetail> listInsertAdjustDataDetail = new List<TTAdjustDataDetail>();
                List<TTAdjustDataBrandDetail> listInsertAdjustDataBrandDetail = new List<TTAdjustDataBrandDetail>();

                foreach (var itemAdjustDetail in request.adjustDataDetail)
                {
                    TTAdjustDataDetail adjustDataItem = new TTAdjustDataDetail
                    {
                        ID = Guid.NewGuid(),
                        AdjustData_ID = request.adjustDataID,
                        Adjust_AmountSale = itemAdjustDetail.adjustAmountSale,
                        Adjust_WholeSale = itemAdjustDetail.adjustWholeSale,
                        Admin_AmountSale = itemAdjustDetail.adminAmountSale,
                        Amount_PreviousYear = itemAdjustDetail.amountPreviousYear,
                        Brand_ID = itemAdjustDetail.brandID,
                        FG = itemAdjustDetail.fg,
                        MU = itemAdjustDetail.mu,
                        OT = itemAdjustDetail.ot,
                        SK = itemAdjustDetail.sk,
                        Rank = itemAdjustDetail.rank,
                        Percent_Growth = itemAdjustDetail.percentGrowth,
                        Remark = itemAdjustDetail.remark
                    };

                    foreach (var brandData in brandDataList)
                    {
                        var amountSale = itemAdjustDetail.brandKeyInAmount.FirstOrDefault(e => e.Key == brandData.Brand_Short_Name || e.Key == brandData.Brand_Name);
                        var rank = itemAdjustDetail.brandKeyInRank.FirstOrDefault(e => e.Key == brandData.Brand_Short_Name || e.Key == brandData.Brand_Name);
                        decimal? amountSaleValue = null;
                        int? rankValue = null;

                        if (amountSale.Value != null)
                        {
                            amountSaleValue = amountSale.Value;
                        }

                        if (rank.Value != null)
                        {
                            rankValue = int.Parse(rank.Value);
                        }
                        TTAdjustDataBrandDetail adjustBrandDetail = new TTAdjustDataBrandDetail
                        {
                            ID = Guid.NewGuid(),
                            BrandCounter_ID = brandData.Brand_ID,
                            AdjustData_ID = request.adjustDataID,
                            Amount_Sale = amountSaleValue,
                            Rank = rankValue,
                            Brand_ID = itemAdjustDetail.brandID
                        };

                        listInsertAdjustDataBrandDetail.Add(adjustBrandDetail);
                    }

                    listInsertAdjustDataDetail.Add(adjustDataItem);
                }

                var saveAdjustDetailResult = await repository.adjust.InsertAdjustDataDetail(listInsertAdjustDataDetail);
                var saveAdjustBrandDetailResult = await repository.adjust.InsertAdjustDataBrandDetail(listInsertAdjustDataBrandDetail);

                if (saveAdjustDetailResult)
                {
                    response.isSuccess = true;
                }
                else
                {
                    response.isSuccess = false;
                }

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }
    }
}
