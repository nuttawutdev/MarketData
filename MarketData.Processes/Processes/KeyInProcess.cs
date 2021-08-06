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

        public BAKeyInDetailResponse CreateBAKeyInDetail(CreateBAKeyInRequest request)
        {
            DateTime dateNow = DateTime.Now;
            BAKeyInDetailResponse response = new BAKeyInDetailResponse();
            var keyInStatusNew = repository.masterData.GetKeyInStatusBy(c => c.Status_Name == "New");

            try
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
                        (bool createDetailResult, List<TTBAKeyInDetail> listDetail) = CreateBAKeyInDetail(request, createBAKeyInResponse.ID);

                        if (createDetailResult)
                        {
                            var listBrandIDInCounter = listDetail.Select(r => r.Brand_ID);
                            var brandListData = repository.masterData.GetBrandListBy(e => listBrandIDInCounter.Contains(e.Brand_ID));

                            response.data = listDetail.Select(e => new BAKeyInDetailData
                            {
                                ID = e.ID,
                                keyInID = createBAKeyInResponse.ID,
                                departmentStoreID = e.DepartmentStore_ID,
                                brandID = e.Brand_ID,
                                brandName = brandListData.FirstOrDefault(c => c.Brand_ID == e.Brand_ID)?.Brand_Name,
                                channelID = e.DistributionChannel_ID,
                                yaer = e.Year,
                                month = e.Month,
                                week = e.Week,
                                counterID = e.Counter_ID
                            }).ToList();

                            response.isSuccess = true;
                            response.status = keyInStatusNew.Status_Name;
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
                        (bool createDetailResult, List<TTBAKeyInDetail> listDetail) = CreateBAKeyInDetail(request, baKeyInData.ID);

                        if (createDetailResult)
                        {
                            var listBrandIDInCounter = listDetail.Select(r => r.Brand_ID);
                            var brandListData = repository.masterData.GetBrandListBy(e => listBrandIDInCounter.Contains(e.Brand_ID));

                            response.data = listDetail.Select(e => new BAKeyInDetailData
                            {
                                ID = e.ID,
                                keyInID = baKeyInData.ID,
                                departmentStoreID = e.DepartmentStore_ID,
                                brandID = e.Brand_ID,
                                brandName = brandListData.FirstOrDefault(c => c.Brand_ID == e.Brand_ID)?.Brand_Name,
                                channelID = e.DistributionChannel_ID,
                                yaer = e.Year,
                                month = e.Month,
                                week = e.Week,
                                counterID = e.Counter_ID
                            }).ToList();

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

                        // New Counter
                        if (counterList.Count > BAKeyInDetailList.Count)
                        {
                            var existBrandList = BAKeyInDetailList.Select(c => c.brandID);
                            var newCounter = counterList.Where(e => !existBrandList.Contains(e.Brand_ID));

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
                                Created_By = request.userID,
                                Created_Date = dateNow,
                                Counter_ID = c.Counter_ID
                            }).ToList();

                            repository.baKeyIn.CreateBAKeyInDetail(listBAKeyInDetail);
                            BAKeyInDetailList = repository.baKeyIn.GetBAKeyInDetailBy(c => c.BAKeyIn_ID == baKeyInData.ID);
                        }

                        string previousYear = (Int32.Parse(request.year) - 1).ToString();

                        foreach (var itemBADetail in BAKeyInDetailList)
                        {
                            var BAKeyInDetailPreviousYear = repository.baKeyIn.GetBAKeyInDetailBy(
                                c => c.DepartmentStore_ID == request.departmentStoreID
                                && c.DistributionChannel_ID == request.distributionChannelID
                                && c.Brand_ID == itemBADetail.brandID
                                && c.Year == previousYear
                                && c.Month == request.month
                                && c.Week == "4").FirstOrDefault();

                            if (BAKeyInDetailPreviousYear != null)
                            {
                                itemBADetail.amountSalePreviousYear = BAKeyInDetailPreviousYear.amountSale;
                            }
                        }

                        response.isSuccess = true;
                    }

                    response.status = repository.masterData.GetKeyInStatusBy(c => c.ID == baKeyInData.KeyIn_Status_ID)?.Status_Name;
                    response.data = BAKeyInDetailList;

                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            var brandBAData = repository.masterData.FindBrandBy(c => c.Brand_ID == request.brandID);
            var departmentStoreData = repository.masterData.FindDepartmentStoreBy(c => c.Department_Store_ID == request.departmentStoreID);
            var channelBAData = repository.masterData.FindDistributionChannelBy(c => c.Distribution_Channel_ID == request.distributionChannelID);

            response.brand = brandBAData?.Brand_Name;
            response.departmentStore = departmentStoreData?.Department_Store_Name;
            response.channel = channelBAData?.Distribution_Channel_Name;
            response.year = request.year;
            response.month = Enum.GetName(typeof(MonthEnum), Int32.Parse(request.month));
            response.week = request.week;

            return response;
        }

        public BAKeyInDetailResponse GetBAKeyInDetail(Guid baKeyInID)
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

                    repository.baKeyIn.CreateBAKeyInDetail(listBAKeyInDetail);
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

        private (bool, List<TTBAKeyInDetail>) CreateBAKeyInDetail(CreateBAKeyInRequest request, Guid keyInID)
        {
            DateTime dateNow = DateTime.Now;

            try
            {
                var counterList = repository.masterData.GetCounterListBy(
                         e => e.Distribution_Channel_ID == request.distributionChannelID
                         && e.Department_Store_ID == request.departmentStoreID
                         && e.Active_Flag && e.Delete_Flag != true);

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
                    Created_By = request.userID,
                    Created_Date = dateNow,
                    Counter_ID = c.Counter_ID
                }).ToList();

                var createBAKeyInDetailResponse = repository.baKeyIn.CreateBAKeyInDetail(listBAKeyInDetail);
                return (createBAKeyInDetailResponse, listBAKeyInDetail);
            }
            catch (Exception ex)
            {
                return (false, null);
            }
        }
        
    }
}
