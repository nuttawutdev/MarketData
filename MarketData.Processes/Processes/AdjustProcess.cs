using MarketData.Helper;
using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Request.Adjust;
using MarketData.Model.Response.AdjustData;
using MarketData.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

                var onlyBrandLorel = repository.masterData.GetBrandListLoreal(c => allBrandByCounter.Contains(c.Brand_ID)).Where(e => e.universe == request.universe);
                var allApproveKeyInData = repository.approve.GetApproveKeyInData();

                var approveKeyInDataFilter = allApproveKeyInData.Where(
                    c => c.year == request.year && c.month == request.month
                    && c.week == request.week && c.universe == request.universe);

                var adjustStatus = repository.masterData.GetAdjustStatusList();
                var adjustStatusPending = repository.masterData.GetAdjustStatusBy(c => c.Status_Name == "Pending");
                var listAdjustData = repository.adjust.GetAdjustDatalBy(
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
                        statusID = adjustStatusData.ID;
                        statusName = adjustStatus.FirstOrDefault(c => c.ID == adjustStatusData.ID).Status_Name;
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

                    foreach (var itemBrandLoreal in onlyBrandLorel)
                    {
                        var statusBrand = approveKeyInDataFilter.Where(
                            e => e.brandID == itemBrandLoreal.brandID
                            && e.departmentStoreID == itemDepartment.departmentStoreID
                            && e.distributionChannelID == itemDepartment.distributionChannelID
                            && e.retailerGroupID == itemDepartment.retailerGroupID)
                            .OrderByDescending(c => c.dateApprove).FirstOrDefault();

                        var brandShortName = !string.IsNullOrWhiteSpace(itemBrandLoreal.brandShortName) ? itemBrandLoreal.brandShortName : itemBrandLoreal.brandName;
                        var approveStatus = statusBrand?.statusName;

                        adjustData.brandStatus.Add(brandShortName, approveStatus);
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

                var baKeyInData = repository.baKeyIn.GetBAKeyInBy(c => c.ID != Guid.Empty);

                var olldYearList = baKeyInData.Where(e => e.Year != currentYear).GroupBy(c => c.Year).Select(s => s.Key);

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
                bool isSubmitted = false;
                var adjustStatusSubmit = repository.masterData.GetAdjustStatusBy(c => c.Status_Name == "Submit");

                var adjustData = repository.adjust.FindAdjustDatalBy(c => c.ID == adjustDataID);
                var adjustDetail = repository.adjust.GetAdjustDataDetaillBy(t => t.AdjustData_ID == adjustData.ID);

                if (adjustData.Status_ID == adjustStatusSubmit.ID)
                {
                    isSubmitted = true;
                }

                var counterList = repository.masterData.GetCounterListBy(
                       e => e.Distribution_Channel_ID == adjustData.DistributionChannel_ID
                       && e.Department_Store_ID == adjustData.DepartmentStore_ID
                       && e.Active_Flag && e.Delete_Flag != true);

                if (adjustData.Week != "4")
                {
                    // Filter Brand Type Fragrances
                    List<TMCounter> listCounterFilterFragrances = new List<TMCounter>();

                    var brandIDCounter = counterList.GroupBy(c => c.Brand_ID).Select(e => e.Key);
                    var brandDataList = repository.masterData.GetBrandListBy(c => brandIDCounter.Contains(c.Brand_ID) && c.Active_Flag);
                    var brandTypeList = repository.masterData.GetBrandTypeList().Where(e => e.Active_Flag);

                    foreach (var itemCounter in counterList)
                    {
                        var brandData = brandDataList.FirstOrDefault(c => c.Brand_ID == itemCounter.Brand_ID);
                        var brandTypeData = brandTypeList.FirstOrDefault(c => c.Brand_Type_ID == brandData.Brand_Type_ID);

                        if (brandTypeData?.Brand_Type_Name != "Fragrances")
                        {
                            listCounterFilterFragrances.Add(itemCounter);
                        }
                    }

                    counterList = listCounterFilterFragrances;
                }

                #region Get Brand Data
                var allBrandByCounter = counterList.GroupBy(e => e.Brand_ID).Select(c => c.Key);
                var allBrandByCounterListData = repository.masterData.GetBrandListBy(c => allBrandByCounter.Contains(c.Brand_ID) && c.Universe == adjustData.Universe);
                var onlyBrandLorel = repository.masterData.GetBrandListLoreal(c => allBrandByCounter.Contains(c.Brand_ID)).Where(e => e.universe == adjustData.Universe);
                var onlyBrandLorealID = onlyBrandLorel.Select(e => e.brandID);
                #endregion

                #region Get BAKeyIn Data
                var keyInStatusApprove = repository.masterData.GetKeyInStatusBy(e => e.Status_Name == "Approve");

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

                var baKeyInDataApprovePreviousYear = repository.baKeyIn.GetBAKeyInBy(
                    c => c.Year == previousYear
                    && c.Month == adjustData.Month
                    && c.Week == "4"
                    && c.DistributionChannel_ID == adjustData.DistributionChannel_ID
                    && c.DepartmentStore_ID == adjustData.DepartmentStore_ID
                    && c.Universe == adjustData.Universe
                    && c.KeyIn_Status_ID == keyInStatusApprove.ID
                    && onlyBrandLorealID.Contains(c.Brand_ID));

                var baKeyInIDApprove = baKeyInDataApprove.Select(t => t.ID);
                var baKeyInIDApprovePreviousYear = baKeyInDataApprovePreviousYear.Select(t => t.ID);

                var baKeyInDetailApprove = repository.baKeyIn.GetBAKeyInDetailListData(p => baKeyInIDApprove.Contains(p.BAKeyIn_ID));
                var baKeyInDetailApprovePreviousYear = repository.baKeyIn.GetBAKeyInDetailListData(p => baKeyInIDApprovePreviousYear.Contains(p.BAKeyIn_ID));
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
                    decimal? adjustAmountSale = null;
                    decimal? adjustWholeSale = null;
                    decimal? sk = null;
                    decimal? mu = null;
                    decimal? fg = null;
                    decimal? ot = null;
                    string remark = null;

                    var keyInDataPreviousYear = baKeyInDetailApprovePreviousYear.Where(c => c.Brand_ID == itemBrandInDepartment.Brand_ID).OrderByDescending(e => e.Amount_Sales).FirstOrDefault();
                    if (keyInDataPreviousYear != null)
                    {
                        // Or ค่าจากการ Adjust ถาม ลูกค้า
                        amountPreviousYear = keyInDataPreviousYear.Amount_Sales;
                    }

                    var adminKeyInData = adminKeyInDetailData.FirstOrDefault(a => a.Brand_ID == itemBrandInDepartment.Brand_ID);
                    var baKeyInBrand = baKeyInDetailApprove.Where(b => b.Brand_ID == itemBrandInDepartment.Brand_ID);
                    var adjustBrandData = adjustDetail.FirstOrDefault(e => e.Brand_ID == itemBrandInDepartment.Brand_ID);

                    if (!isSubmitted)
                    {
                        //if (adjustBrandData != null)
                        //{
                        //    adminAmountSale = adjustBrandData.Admin_AmountSale;
                        //    adjustAmountSale = adjustBrandData.Adjust_AmountSale;
                        //    adjustWholeSale = adjustBrandData.Adjust_WholeSale;
                        //    sk = adjustBrandData.SK;
                        //    mu = adjustBrandData.MU;
                        //    fg = adjustBrandData.FG;
                        //    ot = adjustBrandData.OT;
                        //    remark = adjustBrandData.Remark;
                        //}
                        // Brand Loreal ค่าว่างไหมถ้า Admin ยังไม่กรอก
                        // เอาค่าของ Admin หรือค่าที่เคย Adjust
                        if (adminKeyInData != null)
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
                        // เอาค่าที่มากสุด หรือค่าที่เคย Adjust
                        else if (baKeyInBrand.Any())
                        {
                            if (baKeyInBrand.Any(c => c.Amount_Sales.HasValue) && baKeyInBrand.Any(c => c.Whole_Sales.HasValue))
                            {
                                var mostAmountSale = baKeyInBrand.Where(e => e.Amount_Sales.HasValue).OrderByDescending(c => c.Amount_Sales).FirstOrDefault().Amount_Sales;
                                var mostWholeSale = baKeyInBrand.Where(e => e.Whole_Sales.HasValue).OrderByDescending(c => c.Whole_Sales).FirstOrDefault().Amount_Sales;

                                adjustAmountSale = mostAmountSale;
                                adjustWholeSale = mostWholeSale;

                                foreach (var itemBrandLoreal in onlyBrandLorel.OrderBy(c => c.lorealBrandRank))
                                {
                                    var brandLorealKeyIn = baKeyInDataApprove.FirstOrDefault(t => t.Brand_ID == itemBrandLoreal.brandID);
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
                            else
                            {
                                foreach (var itemBrandLoreal in onlyBrandLorel.OrderBy(c => c.lorealBrandRank))
                                {
                                    var brandLorealKeyIn = baKeyInDataApprove.FirstOrDefault(t => t.Brand_ID == itemBrandLoreal.brandID);
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
                    else
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
                    }

                    AdjustDetailData adjustDetailData = new AdjustDetailData
                    {
                        brandID = itemBrandInDepartment.Brand_ID,
                        brandName = itemBrandInDepartment.Brand_Name,
                        amountPreviousYear = amountPreviousYear,
                        brandKeyInAmount = new Dictionary<string, decimal?>(),
                        brandKeyInRank = new Dictionary<string, string>(),
                        adminAmountSale = adminAmountSale,
                        adjustAmountSale = adjustAmountSale,
                        adjustWholeSale = adjustWholeSale,
                        sk = sk,
                        mu = mu,
                        fg = fg,
                        ot = ot,
                        remark = remark
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

                int rankAdjust = 1;

                foreach (var itemAdjustData in listAdjustDetailData.OrderByDescending(e => e.adjustAmountSale))
                {
                    itemAdjustData.rank = rankAdjust;
                    if (itemAdjustData.amountPreviousYear.HasValue)
                    {
                        // ((adjustAmountSale - amountPreviousYear) / amountPreviousYear) X 100
                        itemAdjustData.percentGrowth = ((itemAdjustData.adjustAmountSale.GetValueOrDefault() - itemAdjustData.amountPreviousYear.GetValueOrDefault()) / itemAdjustData.amountPreviousYear.GetValueOrDefault()) * 100;
                    }

                    rankAdjust += 1;
                }

                var departmentData = repository.masterData.FindDepartmentStoreBy(c => c.Department_Store_ID == adjustData.DepartmentStore_ID);
                var retailerGroup = repository.masterData.FindRetailerGroupBy(e => e.Retailer_Group_ID == departmentData.Retailer_Group_ID);
                var channelData = repository.masterData.FindDistributionChannelBy(c => c.Distribution_Channel_ID == adjustData.DistributionChannel_ID);

                List<string> brandColumn = new List<string>();

                foreach (var itemBrandLoreal in onlyBrandLorel)
                {
                    string brandName = !string.IsNullOrWhiteSpace(itemBrandLoreal.brandShortName) ? itemBrandLoreal.brandShortName : itemBrandLoreal.brandName;

                    brandColumn.Add($"{brandName}-Amt.Sales");
                    brandColumn.Add($"{brandName}-Rank");
                }

                var adjustStatus = repository.masterData.GetAdjustStatusBy(e => e.ID == adjustData.Status_ID);

                response.status = adjustStatus.Status_Name;
                response.brandDataColumn = brandColumn;
                response.year = adjustData.Year;
                response.month = Enum.GetName(typeof(MonthEnum), Int32.Parse(adjustData.Month));
                response.week = adjustData.Week;
                response.channel = channelData.Distribution_Channel_Name;
                response.retailerGroup = retailerGroup.Retailer_Group_Name;
                response.departmentStore = departmentData.Department_Store_Name;
                response.universe = adjustData.Universe;
                response.data = listAdjustDetailData;
            }
            catch (Exception ex)
            {
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public async Task<SaveAdjustDetailResponse> CreateAdjustData(GetAdjustDetailRequest request)
        {
            SaveAdjustDetailResponse response = new SaveAdjustDetailResponse();

            try
            {
                var adjustData = repository.adjust.FindAdjustDatalBy(
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
    }
}
