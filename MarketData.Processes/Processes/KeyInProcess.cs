using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Request.KeyIn;
using MarketData.Model.Response;
using MarketData.Model.Response.KeyIn;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MarketData.Helper.Utility;

namespace MarketData.Processes.Processes
{
    public class KeyInProcess
    {
        private readonly Repository repository;

        public KeyInProcess(Repository repository)
        {
            this.repository = repository;
        }

        public GetBAKeyInListResponse GetBAKeyInList(Guid userID)
        {
            GetBAKeyInListResponse response = new GetBAKeyInListResponse();

            try
            {
                var userCounterData = repository.baKeyIn.GetUserCounter(userID);
                List<BAKeyInData> baKeyInData = new List<BAKeyInData>();

                foreach (var itemUserCounter in userCounterData)
                {
                    var baKeyInByCounter = repository.baKeyIn.GetBAKeyInByCounter(itemUserCounter.DepartmentStore_ID, itemUserCounter.Brand_ID, itemUserCounter.DistributionChannel_ID);
                    baKeyInData.AddRange(baKeyInByCounter);
                }

                response.data = baKeyInData;

            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public GetBAKeyInOptionResponse GetBAKeyInOption(Guid userID)
        {
            GetBAKeyInOptionResponse response = new GetBAKeyInOptionResponse();

            try
            {
                var userCounterData = repository.baKeyIn.GetUserCounter(userID);
                List<DepartmentStoreData> departmentStoreBA = new List<DepartmentStoreData>();
                List<RetailerGroupData> retailerGroupBA = new List<RetailerGroupData>();
                List<DistributionChannelData> channelBA = new List<DistributionChannelData>();
                List<BrandData> brandBA = new List<BrandData>();

                var groupByDepartmentStore = userCounterData.GroupBy(s => s.DepartmentStore_ID);
                var groupByChannel = userCounterData.GroupBy(s => s.DistributionChannel_ID);
                var groupByBrand = userCounterData.GroupBy(s => s.Brand_ID);

                foreach (var itemGroupStore in groupByDepartmentStore)
                {
                    var departmentData = repository.masterData.FindDepartmentStoreBy(c => c.Department_Store_ID == itemGroupStore.Key);

                    DepartmentStoreData depaermentBA = new DepartmentStoreData
                    {
                        departmentStoreID = departmentData.Department_Store_ID,
                        departmentStoreName = departmentData.Department_Store_Name,
                        retailerGroupID = departmentData.Retailer_Group_ID,
                        distributionChannelID = departmentData.Department_Store_ID,
                    };

                    departmentStoreBA.Add(depaermentBA);
                }

                foreach (var itemGroupChannel in groupByChannel)
                {
                    var channelData = repository.masterData.FindDistributionChannelBy(c => c.Distribution_Channel_ID == itemGroupChannel.Key);

                    DistributionChannelData channel = new DistributionChannelData
                    {
                        distributionChannelID = channelData.Distribution_Channel_ID,
                        distributionChannelName = channelData.Distribution_Channel_Name
                    };

                    channelBA.Add(channel);
                }

                foreach (var itemGroupBrand in groupByBrand)
                {
                    var brandData = repository.masterData.FindBrandBy(c => c.Brand_ID == itemGroupBrand.Key);

                    BrandData brand = new BrandData
                    {
                        brandID = brandData.Brand_ID,
                        brandName = brandData.Brand_Name,
                    };

                    brandBA.Add(brand);
                }

                var groupByRetailer = departmentStoreBA.GroupBy(g => g.retailerGroupID);

                foreach (var itemRetailer in groupByRetailer)
                {
                    var retailerData = repository.masterData.FindRetailerGroupBy(c => c.Retailer_Group_ID == itemRetailer.Key);

                    RetailerGroupData retailer = new RetailerGroupData
                    {
                        retailerGroupID = retailerData.Retailer_Group_ID,
                        retailerGroupName = retailerData.Retailer_Group_Name,
                    };

                    retailerGroupBA.Add(retailer);
                }

                List<TTBAKeyIn> listBAKeyIn = new List<TTBAKeyIn>();

                foreach (var itemUserCounter in userCounterData)
                {
                    var baKeyInByCounter = repository.baKeyIn.GetBAKeyInBy(c => c.DepartmentStore_ID == itemUserCounter.DepartmentStore_ID
                    && c.DistributionChannel_ID == itemUserCounter.DistributionChannel_ID && c.Brand_ID == itemUserCounter.Brand_ID);

                    listBAKeyIn.AddRange(baKeyInByCounter);
                }

                response.year.Add(DateTime.Now.Year.ToString());

                if (listBAKeyIn.Any())
                {
                    var groupByYear = listBAKeyIn.GroupBy(e => e.Year).Where(c => c.Key != DateTime.Now.Year.ToString());

                    foreach (var itemGroupYear in groupByYear)
                    {
                        response.year.Add(itemGroupYear.Key);
                    }
                }

                response.channel = channelBA;
                response.departmentStore = departmentStoreBA;
                response.brand = brandBA;
                response.retailerGroup = retailerGroupBA;
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public async Task<CreateBAKeyInDetailResponse> CreateBAKeyInDetail(CreateBAKeyInRequest request)
        {
            DateTime dateNow = DateTime.Now;
            CreateBAKeyInDetailResponse response = new CreateBAKeyInDetailResponse();
            var keyInStatusNew = repository.masterData.GetKeyInStatusBy(c => c.Status_Name == "New");

            try
            {
                var userCounterData = repository.baKeyIn.GetUserCounter(request.userID.GetValueOrDefault());

                var userCounterValidate = userCounterData.Where(c =>
                    c.DepartmentStore_ID == request.departmentStoreID
                    && c.Brand_ID == request.brandID
                    && c.DistributionChannel_ID == request.distributionChannelID).Any();

                if (userCounterValidate)
                {
                    var baKeyInData = repository.baKeyIn.FindBAKeyInBy(
                                c => c.DepartmentStore_ID == request.departmentStoreID
                                && c.DistributionChannel_ID == request.distributionChannelID
                                && c.Brand_ID == request.brandID
                                && c.RetailerGroup_ID == request.retailerGroupID
                                && c.Year == request.year
                                && c.Month == request.month
                                && c.Week == request.week);

                    if (baKeyInData == null)
                    {
                        var createBAKeyInResponse = repository.baKeyIn.CreateBAKeyIn(request);

                        if (createBAKeyInResponse != null)
                        {
                            (bool createDetailResult, List<TTBAKeyInDetail> listDetail) = await CreateBAKeyInDetail(request, createBAKeyInResponse.ID);

                            if (createDetailResult)
                            {
                                response.baKeyInID = createBAKeyInResponse.ID;
                                response.isSuccess = true;
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
                    else
                    {
                        var BAKeyInDetailList = repository.baKeyIn.GetBAKeyInDetailBy(c => c.BAKeyIn_ID == baKeyInData.ID);

                        if (!BAKeyInDetailList.Any())
                        {
                            (bool createDetailResult, List<TTBAKeyInDetail> listDetail) = await CreateBAKeyInDetail(request, baKeyInData.ID);

                            if (createDetailResult)
                            {
                                var listBrandIDInCounter = listDetail.Select(r => r.Brand_ID);
                                var brandListData = repository.masterData.GetBrandListBy(e => listBrandIDInCounter.Contains(e.Brand_ID));

                                response.baKeyInID = baKeyInData.ID;
                                response.isSuccess = true;
                            }
                            else
                            {
                                response.isSuccess = false;
                            }
                        }
                        else
                        {
                            var counterList = repository.masterData.GetCounterListBy(
                                            e => e.Distribution_Channel_ID == request.distributionChannelID
                                            && e.Department_Store_ID == request.departmentStoreID
                                            && e.Active_Flag && e.Delete_Flag != true);

                            try
                            {
                                // New Counter
                                if (counterList.Count > BAKeyInDetailList.Count)
                                {
                                    var existBrandList = BAKeyInDetailList.Select(c => c.brandID);
                                    var newCounter = counterList.Where(e => !existBrandList.Contains(e.Brand_ID));

                                    if (request.week != "4")
                                    {
                                        List<TMCounter> listCounterFilterFragrances = new List<TMCounter>();

                                        foreach (var itemCounter in newCounter)
                                        {
                                            var brandData = repository.masterData.FindBrandBy(c => c.Brand_ID == itemCounter.Brand_ID);
                                            var brandTypeData = repository.masterData.FindBrandTypeBy(c => c.Brand_Type_ID == brandData.Brand_Type_ID);

                                            if (brandTypeData?.Brand_Type_Name != "Fragrances")
                                            {
                                                listCounterFilterFragrances.Add(itemCounter);
                                            }
                                        }

                                        newCounter = listCounterFilterFragrances;
                                    }

                                    List<TTBAKeyInDetail> listBAKeyInDetail = newCounter.Select(c => new TTBAKeyInDetail
                                    {
                                        ID = Guid.NewGuid(),
                                        BAKeyIn_ID = baKeyInData.ID,
                                        DepartmentStore_ID = request.departmentStoreID,
                                        DistributionChannel_ID = request.distributionChannelID,
                                        Brand_ID = c.Brand_ID,
                                        Year = request.year,
                                        Month = request.month,
                                        Week = request.week,
                                        Created_By = request.userID.GetValueOrDefault(),
                                        Created_Date = dateNow,
                                        Counter_ID = c.Counter_ID
                                    }).ToList();

                                    await repository.baKeyIn.CreateBAKeyInDetail(listBAKeyInDetail);
                                }
                            }
                            finally
                            {
                                response.baKeyInID = baKeyInData.ID;
                                response.isSuccess = true;
                            }
                        }
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.responseError = "ข้อมูลไม่ถูกต้อง";
                }

            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public async Task<BAKeyInDetailResponse> GetBAKeyInDetail(Guid baKeyInID)
        {
            BAKeyInDetailResponse response = new BAKeyInDetailResponse();

            try
            {
                var BAKeyInData = repository.baKeyIn.FindBAKeyInBy(c => c.ID == baKeyInID);
                var BAKeyInDetailList = repository.baKeyIn.GetBAKeyInDetailBy(c => c.BAKeyIn_ID == baKeyInID);

                var counterList = repository.masterData.GetCounterListBy(
                        e => e.Distribution_Channel_ID == BAKeyInData.DistributionChannel_ID
                        && e.Department_Store_ID == BAKeyInData.DepartmentStore_ID
                        && e.Active_Flag && e.Delete_Flag != true);


                // New Counter
                if (counterList.Count > BAKeyInDetailList.Count)
                {
                    var existBrandList = BAKeyInDetailList.Select(c => c.brandID);
                    var newCounter = counterList.Where(e => !existBrandList.Contains(e.Brand_ID));


                    if (BAKeyInData.Week != "4")
                    {
                        // Filter Brand Type Fragrances
                        List<TMCounter> listCounterFilterFragrances = new List<TMCounter>();

                        foreach (var itemCounter in newCounter)
                        {
                            var brandData = repository.masterData.FindBrandBy(c => c.Brand_ID == itemCounter.Brand_ID);
                            var brandTypeData = repository.masterData.FindBrandTypeBy(c => c.Brand_Type_ID == brandData.Brand_Type_ID);

                            if (brandTypeData?.Brand_Type_Name != "Fragrances")
                            {
                                listCounterFilterFragrances.Add(itemCounter);
                            }
                        }

                        newCounter = listCounterFilterFragrances;
                    }


                    List<TTBAKeyInDetail> listBAKeyInDetail = newCounter.Select(c => new TTBAKeyInDetail
                    {
                        ID = Guid.NewGuid(),
                        BAKeyIn_ID = BAKeyInData.ID,
                        DepartmentStore_ID = BAKeyInData.DepartmentStore_ID,
                        DistributionChannel_ID = BAKeyInData.DistributionChannel_ID,
                        Brand_ID = c.Brand_ID,
                        Year = BAKeyInData.Year,
                        Month = BAKeyInData.Month,
                        Week = BAKeyInData.Week,
                        Created_By = BAKeyInData.Created_By.GetValueOrDefault(),
                        Created_Date = DateTime.Now,
                        Counter_ID = c.Counter_ID
                    }).ToList();

                    await repository.baKeyIn.CreateBAKeyInDetail(listBAKeyInDetail);
                    BAKeyInDetailList = repository.baKeyIn.GetBAKeyInDetailBy(c => c.BAKeyIn_ID == BAKeyInData.ID);
                }

                string previousYear = (Int32.Parse(BAKeyInData.Year) - 1).ToString();

                foreach (var itemBADetail in BAKeyInDetailList)
                {
                    var BAKeyInDetailPreviousYear = repository.baKeyIn.GetBAKeyInDetailBy(
                        c => c.DepartmentStore_ID == BAKeyInData.DepartmentStore_ID
                        && c.DistributionChannel_ID == BAKeyInData.DistributionChannel_ID
                        && c.Brand_ID == itemBADetail.brandID
                        && c.Year == previousYear
                        && c.Month == BAKeyInData.Month
                        && c.Week == "4").FirstOrDefault();

                    if (BAKeyInDetailPreviousYear != null)
                    {
                        itemBADetail.amountSalePreviousYear = BAKeyInDetailPreviousYear.amountSale;
                    }
                }

                var brandBAData = repository.masterData.FindBrandBy(c => c.Brand_ID == BAKeyInData.Brand_ID);
                var departmentStoreData = repository.masterData.FindDepartmentStoreBy(c => c.Department_Store_ID == BAKeyInData.DepartmentStore_ID);
                var channelBAData = repository.masterData.FindDistributionChannelBy(c => c.Distribution_Channel_ID == BAKeyInData.DistributionChannel_ID);

                response.status = repository.masterData.GetKeyInStatusBy(c => c.ID == BAKeyInData.KeyIn_Status_ID)?.Status_Name;
                response.brand = brandBAData?.Brand_Name;
                response.departmentStore = departmentStoreData?.Department_Store_Name;
                response.channel = channelBAData?.Distribution_Channel_Name;
                response.year = BAKeyInData.Year;
                response.month = Enum.GetName(typeof(MonthEnum), Int32.Parse(BAKeyInData.Month));
                response.week = BAKeyInData.Week;
                response.data = BAKeyInDetailList;

            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> SaveBAKeyInDetail(SaveBAKeyInDetailRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();
            DateTime dateNoew = DateTime.Now;

            try
            {
                var inprogressStatus = repository.masterData.GetKeyInStatusBy(c => c.Status_Name == "In-Progress");
                var updateBAKeyIn = await repository.baKeyIn.UpdateBAKeyIn(request, inprogressStatus.ID);

                if (updateBAKeyIn)
                {
                    response.isSuccess = await SaveBAKeyInDetailData(request);
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

        public async Task<SaveDataResponse> SubmitBAKeyInDetail(SaveBAKeyInDetailRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();
            DateTime dateNoew = DateTime.Now;

            try
            {
                var submitStatus = repository.masterData.GetKeyInStatusBy(c => c.Status_Name == "Submit");
                var updateBAKeyIn = await repository.baKeyIn.UpdateBAKeyIn(request, submitStatus.ID, true);

                if (updateBAKeyIn)
                {
                    response.isSuccess = await SaveBAKeyInDetailData(request);
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

        public GetAdminKeyInResponse GetAdminKeyInData(GetAdminKeyInRequest request)
        {
            GetAdminKeyInResponse response = new GetAdminKeyInResponse();

            try
            {
                var counterList = repository.masterData.GetCounterList();
                var counterByFilter = counterList.Where(
                    c => (!request.departmentStoreID.HasValue || (request.departmentStoreID.HasValue && request.departmentStoreID == c.departmentStoreID))
                    && (!request.retailerGroupID.HasValue || (request.retailerGroupID.HasValue && request.retailerGroupID == c.retailerGroupID))
                    && (!request.distributionChannelID.HasValue || (request.distributionChannelID.HasValue && request.distributionChannelID == c.distributionChannelID))
                    && (!request.brandID.HasValue || (request.brandID.HasValue && request.brandID == c.brandID)));

                List<AdminKeyInDetailData> adminKeyInDetailList = new List<AdminKeyInDetailData>();

                foreach (var itemCounter in counterByFilter)
                {
                    if (request.week != "4")
                    {

                        var brandData = repository.masterData.FindBrandBy(c => c.Brand_ID == itemCounter.brandID);
                        var brandTypeData = repository.masterData.FindBrandTypeBy(c => c.Brand_Type_ID == brandData.Brand_Type_ID);

                        if (brandTypeData?.Brand_Type_Name != "Fragrances")
                        {

                            AdminKeyInDetailData dataDetail = GetAdminKeyInDetailData(itemCounter, request);
                            adminKeyInDetailList.Add(dataDetail);
                        }
                    }
                    else
                    {
                        AdminKeyInDetailData dataDetail = GetAdminKeyInDetailData(itemCounter, request);
                        adminKeyInDetailList.Add(dataDetail);
                    }
                   
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public async Task<SaveAdminKeyInResponse> SaveAdminKeyIn(SaveAdminKeyInDetailRequest request)
        {
            SaveAdminKeyInResponse response = new SaveAdminKeyInResponse();
            DateTime dateNoew = DateTime.Now;

            try
            {
                var updateAdminKeyInDetailID = request.data.Where(e => e.ID != Guid.Empty).Select(c => c.ID);
                var newAdminKeyInDetail = request.data.Where(e => e.ID == Guid.Empty);

                bool addDetailResult = true;
                bool updateDetailResult = true;

                if (updateAdminKeyInDetailID.Any())
                {
                    var adminInDetailData = repository.adminKeyIn.GetAdminKeyInDetailBy(c => updateAdminKeyInDetailID.Contains(c.ID));

                    foreach (var itemDetail in adminInDetailData)
                    {
                        var adminKeyInDetailUpdate = request.data.FirstOrDefault(c => c.ID == itemDetail.ID);
                        itemDetail.Rank = adminKeyInDetailUpdate.rank;
                        itemDetail.Amount_Sales = adminKeyInDetailUpdate.amountSale;
                        itemDetail.Whole_Sales = adminKeyInDetailUpdate.wholeSale;
                        itemDetail.SK = adminKeyInDetailUpdate.sk;
                        itemDetail.MU = adminKeyInDetailUpdate.mu;
                        itemDetail.FG = adminKeyInDetailUpdate.fg;
                        itemDetail.OT = adminKeyInDetailUpdate.ot;
                        itemDetail.Remark = adminKeyInDetailUpdate.remark;
                        itemDetail.Updated_By = request.userID;
                        itemDetail.Updated_Date = dateNoew;
                    }

                    updateDetailResult = await repository.adminKeyIn.UpdateAdminKeyInDetail(adminInDetailData);
                }

                if (newAdminKeyInDetail.Any())
                {
                    List<TTAdminKeyInDetail> listNewAdminDetail = newAdminKeyInDetail.Select(c => new TTAdminKeyInDetail
                    {
                        ID = Guid.NewGuid(),
                        DepartmentStore_ID = c.departmentStoreID,
                        DistributionChannel_ID = c.distributionChannelID,
                        Brand_ID = c.brandID,
                        Counter_ID = c.counterID,
                        RetailerGroup_ID = c.retailerGroupID,
                        FG = c.fg,
                        MU = c.mu,
                        OT = c.ot,
                        SK = c.sk,
                        Amount_Sales = c.amountSale,
                        Whole_Sales = c.wholeSale,
                        Month = c.month,
                        Year = c.year,
                        Week = c.week,
                        Rank = c.rank,
                        Remark = c.remark,
                        Universe = c.universe,
                        Created_By = request.userID,
                        Created_Date = dateNoew
                    }).ToList();

                    addDetailResult = await repository.adminKeyIn.AddAdminKeyInDetail(listNewAdminDetail);
                }

                if (updateDetailResult && addDetailResult)
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
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public GetAdminKeyInOptionResponse GetAdminKeyInOption()
        {
            GetAdminKeyInOptionResponse response = new GetAdminKeyInOptionResponse();

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
                string currentYear = DateTime.Now.Year.ToString();

                yearList.Add(currentYear);

                var adminKeyInData = repository.adminKeyIn.GetAdminKeyInDetailBy(c => c.ID != Guid.Empty);

                var olldYearList = adminKeyInData.Where(e => e.Year != currentYear).GroupBy(c => c.Year).Select(s => s.Key);

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

        private async Task<(bool, List<TTBAKeyInDetail>)> CreateBAKeyInDetail(CreateBAKeyInRequest request, Guid keyInID)
        {
            DateTime dateNow = DateTime.Now;

            try
            {
                var counterList = repository.masterData.GetCounterListBy(
                         e => e.Distribution_Channel_ID == request.distributionChannelID
                         && e.Department_Store_ID == request.departmentStoreID
                         && e.Active_Flag && e.Delete_Flag != true);

                if (request.week != "4")
                {
                    // Filter Brand Type Fragrances
                    List<TMCounter> listCounterFilterFragrances = new List<TMCounter>();

                    foreach (var itemCounter in counterList)
                    {
                        var brandData = repository.masterData.FindBrandBy(c => c.Brand_ID == itemCounter.Brand_ID);
                        var brandTypeData = repository.masterData.FindBrandTypeBy(c => c.Brand_Type_ID == brandData.Brand_Type_ID);

                        if (brandTypeData?.Brand_Type_Name != "Fragrances")
                        {
                            listCounterFilterFragrances.Add(itemCounter);
                        }
                    }

                    counterList = listCounterFilterFragrances;
                }

                List<TTBAKeyInDetail> listBAKeyInDetail = counterList.Select(c => new TTBAKeyInDetail
                {
                    ID = Guid.NewGuid(),
                    BAKeyIn_ID = keyInID,
                    DepartmentStore_ID = request.departmentStoreID,
                    DistributionChannel_ID = request.distributionChannelID,
                    Brand_ID = c.Brand_ID,
                    Year = request.year,
                    Month = request.month,
                    Week = request.week,
                    Created_By = request.userID.GetValueOrDefault(),
                    Created_Date = dateNow,
                    Counter_ID = c.Counter_ID
                }).ToList();

                var createBAKeyInDetailResponse = await repository.baKeyIn.CreateBAKeyInDetail(listBAKeyInDetail);
                return (createBAKeyInDetailResponse, listBAKeyInDetail);
            }
            catch (Exception ex)
            {
                return (false, null);
            }
        }

        private async Task<bool> SaveBAKeyInDetailData(SaveBAKeyInDetailRequest request)
        {
            DateTime dateNoew = DateTime.Now;

            try
            {
                var baKeyInDetailID = request.BAKeyInDetailList.Select(c => c.ID);
                var baKeyInDetailData = repository.baKeyIn.GetBAKeyInDetailListData(c => baKeyInDetailID.Contains(c.ID));

                foreach (var itemDetail in baKeyInDetailData)
                {
                    var baKeyInDetailUpdate = request.BAKeyInDetailList.FirstOrDefault(c => c.ID == itemDetail.ID);
                    itemDetail.Rank = baKeyInDetailUpdate.rank;
                    itemDetail.Amount_Sales = baKeyInDetailUpdate.amountSale;
                    itemDetail.Whole_Sales = baKeyInDetailUpdate.wholeSale;
                    itemDetail.SK = baKeyInDetailUpdate.sk;
                    itemDetail.MU = baKeyInDetailUpdate.mu;
                    itemDetail.FG = baKeyInDetailUpdate.fg;
                    itemDetail.OT = baKeyInDetailUpdate.ot;
                    itemDetail.Remark = baKeyInDetailUpdate.remark;
                    itemDetail.Updated_By = request.userID;
                    itemDetail.Updated_Date = dateNoew;
                }

                var updateDetailResult = await repository.baKeyIn.UpdateBAKeyInDetail(baKeyInDetailData);
                return updateDetailResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private AdminKeyInDetailData GetAdminKeyInDetailData(CounterData itemCounter, GetAdminKeyInRequest request)
        {

            var adminKeyInData = repository.adminKeyIn.FindAdminKeyInDetailBy(
                c => c.RetailerGroup_ID == itemCounter.retailerGroupID
                && c.DepartmentStore_ID == itemCounter.departmentStoreID
                && c.DistributionChannel_ID == itemCounter.distributionChannelID
                && c.Brand_ID == itemCounter.brandID
                && c.Year == request.year
                && c.Month == request.month
                && c.Week == request.week);

            string previousYear = (int.Parse(request.year) - 1).ToString();

            var BAKeyInDetailPreviousYear = repository.baKeyIn.GetBAKeyInDetailBy(
               c => c.DepartmentStore_ID == itemCounter.departmentStoreID
               && c.DistributionChannel_ID == itemCounter.distributionChannelID
               && c.Brand_ID == itemCounter.brandID
               && c.Year == previousYear
               && c.Month == request.month
               && c.Week == "4").FirstOrDefault();

            AdminKeyInDetailData dataDetail = new AdminKeyInDetailData
            {
                ID = adminKeyInData != null ? adminKeyInData.ID : Guid.Empty,
                retailerGroupID = itemCounter.retailerGroupID,
                departmentStoreID = itemCounter.departmentStoreID,
                departmentStoreName = itemCounter.departmentStoreName,
                distributionChannelID = itemCounter.distributionChannelID,
                brandID = itemCounter.brandID,
                brandName = itemCounter.brandName,
                year = request.year,
                month = request.month,
                week = request.week,
                counterID = itemCounter.counterID,
                amountSale = adminKeyInData != null ? adminKeyInData.Amount_Sales : null,
                wholeSale = adminKeyInData != null ? adminKeyInData.Whole_Sales : null,
                fg = adminKeyInData != null ? adminKeyInData.FG : null,
                mu = adminKeyInData != null ? adminKeyInData.MU : null,
                ot = adminKeyInData != null ? adminKeyInData.OT : null,
                sk = adminKeyInData != null ? adminKeyInData.SK : null,
                rank = adminKeyInData != null ? adminKeyInData.Rank : null,
                remark = adminKeyInData != null ? adminKeyInData.Remark : null,
                universe = request.universe,
                amountSalePreviousYear = BAKeyInDetailPreviousYear != null ? BAKeyInDetailPreviousYear.amountSale : null
            };

            return dataDetail;
        }
    }
}
