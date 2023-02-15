﻿using MarketData.Helper;
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

        #region BA KeyIn
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

                response.data = baKeyInData.OrderByDescending(c => c.year).ThenByDescending(c => c.month).ThenByDescending(c => c.week).ToList();

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

                string currentYear = Utility.GetDateNowThai().Year.ToString();

                response.year.Add(currentYear);

                if (listBAKeyIn.Any())
                {
                    var groupByYear = listBAKeyIn.GroupBy(e => e.Year).Where(c => c.Key != currentYear);

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
            DateTime dateNow = Utility.GetDateNowThai();
            CreateBAKeyInDetailResponse response = new CreateBAKeyInDetailResponse();
            var keyInStatusNew = repository.masterData.GetKeyInStatusBy(c => c.Status_Name == "New");

            try
            {
                int monthKeyIn = Int32.Parse(request.month);
                int yearKeyIn = Int32.Parse(request.year);

                if (yearKeyIn == Utility.GetDateNowThai().Year && monthKeyIn > Utility.GetDateNowThai().Month)
                {
                    response.responseError = "ไม่สามารถเลือกบันทึกข้อมูลล่วงหน้าได้";
                    return response;
                }


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
                                //&& c.RetailerGroup_ID == request.retailerGroupID
                                && c.Universe == request.universe
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

                        BAKeyInDetailList = BAKeyInDetailList
                          .GroupBy(c => new { c.brandID, c.departmentStoreID, c.channelID, c.amountSale })
                          .Select(g => g.FirstOrDefault())
                          .ToList();

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

                                            if (brandTypeData?.Brand_Type_Name != "Fragrances"
                                                //|| itemCounter.Alway_Show_Current_Year == true
                                                )
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

                        var keyInStauts = repository.masterData.GetKeyInStatusBy(c => c.ID == baKeyInData.KeyIn_Status_ID);
                        response.isSubmited = keyInStauts.Status_Name == "Submit";
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
                var listBAKeyInDetail = repository.baKeyIn.GetBAKeyInDetailBy(c => c.BAKeyIn_ID == baKeyInID);

                listBAKeyInDetail = listBAKeyInDetail
                  .GroupBy(c => new { c.brandID, c.departmentStoreID, c.channelID, c.amountSale })
                  .Select(g => g.FirstOrDefault())
                  .ToList();

                List<BAKeyInDetailData> BAKeyInDetailList = GroupBAKeyInDetailData(listBAKeyInDetail);

                var counterList = repository.masterData.GetCounterListBy(
                        e => e.Distribution_Channel_ID == BAKeyInData.DistributionChannel_ID
                        && e.Department_Store_ID == BAKeyInData.DepartmentStore_ID
                        && e.Active_Flag && e.Delete_Flag != true);


                // New Counter
                if (counterList.Count > listBAKeyInDetail.Count)
                {
                    var existBrandList = BAKeyInDetailList.Select(c => c.brandID);
                    var newCounter = counterList.Where(e => !existBrandList.Contains(e.Brand_ID));

                    if (BAKeyInData.Week != "4")
                    {
                        var brandIDCounter = newCounter.GroupBy(c => c.Brand_ID).Select(e => e.Key);
                        var brandDataList = repository.masterData.GetBrandListBy(c => brandIDCounter.Contains(c.Brand_ID));
                        var brandTypeList = repository.masterData.GetBrandTypeList().Where(e => e.Active_Flag);

                        // Filter Brand Type Fragrances
                        List<TMCounter> listCounterFilterFragrances = new List<TMCounter>();

                        foreach (var itemCounter in newCounter)
                        {
                            var brandData = brandDataList.FirstOrDefault(c => c.Brand_ID == itemCounter.Brand_ID);
                            var brandTypeData = brandTypeList.FirstOrDefault(c => c.Brand_Type_ID == brandData.Brand_Type_ID);

                            if (brandTypeData?.Brand_Type_Name != "Fragrances"
                                //|| itemCounter.Alway_Show_Current_Year == true
                                )
                            {
                                listCounterFilterFragrances.Add(itemCounter);
                            }
                        }

                        newCounter = listCounterFilterFragrances;
                    }

                    List<TTBAKeyInDetail> listBAKeyInDetailInsert = newCounter.Select(c => new TTBAKeyInDetail
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
                        Created_Date = Utility.GetDateNowThai(),
                        Counter_ID = c.Counter_ID
                    }).ToList();

                    await repository.baKeyIn.CreateBAKeyInDetail(listBAKeyInDetailInsert);
                    BAKeyInDetailList = repository.baKeyIn.GetBAKeyInDetailBy(c => c.BAKeyIn_ID == BAKeyInData.ID);
                    BAKeyInDetailList = BAKeyInDetailList
                      .GroupBy(c => new { c.brandID, c.departmentStoreID, c.channelID, c.amountSale })
                      .Select(g => g.FirstOrDefault())
                      .ToList();

                    BAKeyInDetailList = GroupBAKeyInDetailData(BAKeyInDetailList);
                }

                string previousYear = (Int32.Parse(BAKeyInData.Year) - 1).ToString();

                var adjustDataPreviousYearWeek4 = repository.adjust.FindAdjustDataBy(
                  c => c.Year == previousYear
                  && c.Month == BAKeyInData.Month
                  && c.Week == "4"
                  && c.DistributionChannel_ID == BAKeyInData.DistributionChannel_ID
                  && c.DepartmentStore_ID == BAKeyInData.DepartmentStore_ID
                  && c.Universe == BAKeyInData.Universe
                  //&& c.RetailerGroup_ID == BAKeyInData.RetailerGroup_ID
                  );

                if (adjustDataPreviousYearWeek4 != null)
                {
                    var adjustDetailPreviousYearList = repository.adjust.GetAdjustDataDetaillBy(c => c.AdjustData_ID == adjustDataPreviousYearWeek4.ID);
                    foreach (var itemBADetail in BAKeyInDetailList)
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
                var retailerGroupData = repository.masterData.FindRetailerGroupBy(c => c.Retailer_Group_ID == BAKeyInData.RetailerGroup_ID);
                var channelBAData = repository.masterData.FindDistributionChannelBy(c => c.Distribution_Channel_ID == BAKeyInData.DistributionChannel_ID);

                response.remark = BAKeyInData.Remark;
                response.universe = BAKeyInData.Universe;
                response.status = repository.masterData.GetKeyInStatusBy(c => c.ID == BAKeyInData.KeyIn_Status_ID)?.Status_Name;
                response.brand = brandBAData?.Brand_Name;
                response.departmentStore = departmentStoreData?.Department_Store_Name;
                response.retailerGroup = retailerGroupData?.Retailer_Group_Name;
                response.channel = channelBAData?.Distribution_Channel_Name;
                response.year = BAKeyInData.Year;
                response.month = Enum.GetName(typeof(MonthEnum), Int32.Parse(BAKeyInData.Month));
                response.week = BAKeyInData.Week;

                if (BAKeyInData.Year == GetDateNowThai().Year.ToString()
                    || (BAKeyInData.Year == (GetDateNowThai().Year - 1).ToString() && BAKeyInData.Month == "12"))
                {
                    response.data = BAKeyInDetailList
                       .Where(e => e.amountSalePreviousYear > 0
                       || e.counterCreateDate.GetValueOrDefault().Year == GetDateNowThai().Year
                       || e.alwayShow == true)
                       .OrderBy(c => c.brandName).ToList();
                }
                else
                {
                    response.data = BAKeyInDetailList
                    .Where(e => e.amountSalePreviousYear > 0)
                    .OrderBy(c => c.brandName).ToList();
                }

                //if (adjustDataPreviousYearWeek4 != null)
                //{
                //    if (BAKeyInData.Year == GetDateNowThai().Year.ToString())
                //    {
                //        response.data = BAKeyInDetailList
                //           .Where(e => e.amountSalePreviousYear > 0
                //           || e.counterCreateDate.GetValueOrDefault().Year == GetDateNowThai().Year
                //           || e.alwayShow == true)
                //           .OrderBy(c => c.brandName).ToList();
                //    }
                //    else
                //    {
                //        response.data = BAKeyInDetailList
                //        .Where(e => e.amountSalePreviousYear > 0)
                //        .OrderBy(c => c.brandName).ToList();
                //    }
                //}
                //else
                //{
                //    response.data = BAKeyInDetailList.OrderBy(c => c.brandName).ToList();
                //}

                var rejectStatus = repository.masterData.GetApproveKeyInStatusBy(r => r.Status_Name == "Reject");
                var approveData = repository.approve.GetApproveKeyInBy(c => c.BAKeyIn_ID == BAKeyInData.ID).OrderByDescending(d => d.Action_Date).FirstOrDefault();

                if (approveData != null && approveData.Status_ID == rejectStatus.ID)
                {
                    response.rejectReason = approveData.Remark;
                }
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
            DateTime dateNoew = Utility.GetDateNowThai();

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
            DateTime dateNoew = Utility.GetDateNowThai();

            try
            {
                var submitStatus = repository.masterData.GetKeyInStatusBy(c => c.Status_Name == "Submit");
                var updateBAKeyIn = await repository.baKeyIn.UpdateBAKeyIn(request, submitStatus.ID, true);

                if (updateBAKeyIn)
                {
                    var saveDetailResult = await SaveBAKeyInDetailData(request);
                    if (saveDetailResult)
                    {
                        var baKeyInData = repository.baKeyIn.FindBAKeyInBy(c => c.ID == request.BAKeyInID);
                        var createApproveKeyInResult = await repository.approve.CreateApproveKeyInData(baKeyInData);

                        if (createApproveKeyInResult)
                        {
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
                    response.isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
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
        #endregion

        #region Admin KeyIn
        public GetAdminKeyInResponse GetAdminKeyInData(GetAdminKeyInRequest request)
        {
            GetAdminKeyInResponse response = new GetAdminKeyInResponse();

            try
            {
                var counterList = repository.masterData.GetCounterList().Where(e => e.active);
                var counterByFilter = counterList.Where(
                    c => (!request.departmentStoreID.HasValue || (request.departmentStoreID.HasValue && request.departmentStoreID == c.departmentStoreID))
                    && (!request.retailerGroupID.HasValue || (request.retailerGroupID.HasValue && request.retailerGroupID == c.retailerGroupID))
                    && (!request.distributionChannelID.HasValue || (request.distributionChannelID.HasValue && request.distributionChannelID == c.distributionChannelID))
                    && (!request.brandID.HasValue || (request.brandID.HasValue && request.brandID == c.brandID)));

                List<AdminKeyInDetailData> adminKeyInDetailList = new List<AdminKeyInDetailData>();

                var brandIDCounter = counterByFilter.GroupBy(c => c.brandID).Select(e => e.Key);
                var brandDataList = repository.masterData.GetBrandListBy(c => brandIDCounter.Contains(c.Brand_ID));
                var brandTypeList = repository.masterData.GetBrandTypeList().Where(e => e.Active_Flag);

                var allAdminKeyInData = repository.adminKeyIn.GetAdminKeyInDetailBy(e => e.Year == request.year
                                        && e.Month == request.month && e.Week == request.week);

                string previousYear = (int.Parse(request.year) - 1).ToString();

                var adjustDataPreviousYearWeek4 = repository.adjust.GetAdjustDataBy(
                         c => c.Year == previousYear
                        && c.Month == request.month
                        && c.Week == "4"
                        && c.Universe == request.universe);
                var groupDataPreviousYearWeek4 = adjustDataPreviousYearWeek4.GroupBy(
                        x => new
                        {
                            x.Year,
                            x.Month,
                            x.Universe,
                            x.Week,
                            x.DepartmentStore_ID,
                            x.DistributionChannel_ID,
                            x.RetailerGroup_ID
                        }).Select(c => new TTAdjustData()
                        {
                            ID = c.OrderByDescending(e => e.Update_Date).FirstOrDefault().ID,
                            Create_By = c.OrderByDescending(e => e.Update_Date).FirstOrDefault().Create_By,
                            Create_Date = c.OrderByDescending(e => e.Update_Date).FirstOrDefault().Create_Date,
                            DepartmentStore_ID = c.OrderByDescending(e => e.Update_Date).FirstOrDefault().DepartmentStore_ID,
                            DistributionChannel_ID = c.OrderByDescending(e => e.Update_Date).FirstOrDefault().DistributionChannel_ID,
                            Month = c.Key.Month,
                            RetailerGroup_ID = c.OrderByDescending(e => e.Update_Date).FirstOrDefault().RetailerGroup_ID,
                            Status_ID = c.OrderByDescending(e => e.Update_Date).FirstOrDefault().Status_ID,
                            Universe = c.Key.Universe,
                            Update_By = c.OrderByDescending(e => e.Update_Date).FirstOrDefault().Update_By,
                            Update_Date = c.OrderByDescending(e => e.Update_Date).FirstOrDefault().Update_Date,
                            Week = c.Key.Week,
                            Year = c.Key.Year
                        }).ToList();
                var adjustDataIDList = groupDataPreviousYearWeek4.Select(e => e.ID);

                var allAdjustDataDetail = repository.adjust.GetAdjustDataDetaillBy(c => adjustDataIDList.Contains(c.AdjustData_ID));

                var allBAKeyInData = repository.baKeyIn.GetBAKeyInDetailListData(e => e.ID != Guid.Empty);

                foreach (var itemCounter in counterByFilter)
                {
                    if (request.week != "4")
                    {
                        var brandData = brandDataList.FirstOrDefault(c => c.Brand_ID == itemCounter.brandID);
                        var brandTypeData = brandTypeList.FirstOrDefault(c => c.Brand_Type_ID == brandData.Brand_Type_ID);

                        if (brandTypeData?.Brand_Type_Name != "Fragrances"
                            // || itemCounter.alwayShow
                            )
                        {
                            AdminKeyInDetailData dataDetail = GetAdminKeyInDetailData(itemCounter, request, allAdminKeyInData, groupDataPreviousYearWeek4, allAdjustDataDetail, allBAKeyInData);
                            adminKeyInDetailList.Add(dataDetail);
                        }
                    }
                    else
                    {
                        AdminKeyInDetailData dataDetail = GetAdminKeyInDetailData(itemCounter, request, allAdminKeyInData, groupDataPreviousYearWeek4, allAdjustDataDetail, allBAKeyInData);
                        adminKeyInDetailList.Add(dataDetail);
                    }
                }

                adminKeyInDetailList = adminKeyInDetailList.DistinctBy(p => new { p.brandID, p.departmentStoreID, p.amountSale }).ToList();
                foreach (var itemAdmin in adminKeyInDetailList)
                {
                    itemAdmin.brandColor = brandDataList.FirstOrDefault(c => c.Brand_ID == itemAdmin.brandID).Brand_Color;
                }

                if (request.year == GetDateNowThai().Year.ToString()
                    || (request.year == (GetDateNowThai().Year - 1).ToString() && request.month == "12"))
                {
                    response.data = adminKeyInDetailList.Where(e => e.amountSalePreviousYear > 0
                         || e.counterCreateDate.GetValueOrDefault().Year == GetDateNowThai().Year
                         || e.alwayShow)
                         .OrderBy(c => c.brandName).ToList();
                }
                else
                {
                    response.data = adminKeyInDetailList.Where(e => e.amountSalePreviousYear > 0)
                         .OrderBy(c => c.brandName).ToList();
                }


                var amountPreviousYear = adminKeyInDetailList.Where(c => c.amountSalePreviousYear > 0);

                if (amountPreviousYear.Any())
                {
                    response.totalAmountPreviosYear = string.Format("{0:#,0}", amountPreviousYear.Sum(e => e.amountSalePreviousYear.Value));
                }
                else
                {
                    response.totalAmountPreviosYear = string.Empty;
                }

            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }


            return response;
        }

        public async Task<SaveDataResponse> SaveAdminKeyIn(SaveAdminKeyInDetailRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();
            DateTime dateNoew = Utility.GetDateNowThai();

            try
            {
                bool addDetailResult = true;
                bool updateDetailResult = true;

                List<TTAdminKeyInDetail> listSaveData = new List<TTAdminKeyInDetail>();
                List<TTAdminKeyInDetail> listUpdateData = new List<TTAdminKeyInDetail>();
                List<TTAdminKeyInDetail> listAddNewData = new List<TTAdminKeyInDetail>();

                foreach (var itemRequest in request.data.Where(e => e.amountSale.HasValue || e.wholeSale.HasValue || e.rank.HasValue || e.sk.HasValue || e.mu.HasValue || e.fg.HasValue || e.ot.HasValue || !string.IsNullOrWhiteSpace(e.remark)))
                {
                    var existData = repository.adminKeyIn.FindAdminKeyInDetailBy(
                        c => c.Year == itemRequest.year
                        && c.Month == itemRequest.month
                        && c.Week == itemRequest.week
                        && c.DistributionChannel_ID == itemRequest.distributionChannelID
                        // && c.RetailerGroup_ID == itemRequest.retailerGroupID
                        && c.DepartmentStore_ID == itemRequest.departmentStoreID
                        && c.Brand_ID == itemRequest.brandID
                        && c.Universe == itemRequest.universe);

                    if (existData != null)
                    {
                        existData.Rank = itemRequest.rank;
                        existData.Amount_Sales = itemRequest.amountSale;
                        existData.Whole_Sales = itemRequest.wholeSale;
                        existData.SK = itemRequest.sk;
                        existData.MU = itemRequest.mu;
                        existData.FG = itemRequest.fg;
                        existData.OT = itemRequest.ot;
                        existData.Remark = itemRequest.remark;
                        existData.Updated_By = request.userID;
                        existData.Updated_Date = dateNoew;

                        listUpdateData.Add(existData);
                        listSaveData.Add(existData);
                    }
                    else
                    {
                        TTAdminKeyInDetail newAdminDetail = new TTAdminKeyInDetail
                        {
                            ID = Guid.NewGuid(),
                            DepartmentStore_ID = itemRequest.departmentStoreID,
                            DistributionChannel_ID = itemRequest.distributionChannelID,
                            Brand_ID = itemRequest.brandID,
                            Counter_ID = itemRequest.counterID,
                            RetailerGroup_ID = itemRequest.retailerGroupID,
                            FG = itemRequest.fg,
                            MU = itemRequest.mu,
                            OT = itemRequest.ot,
                            SK = itemRequest.sk,
                            Amount_Sales = itemRequest.amountSale,
                            Whole_Sales = itemRequest.wholeSale,
                            Month = itemRequest.month,
                            Year = itemRequest.year,
                            Week = itemRequest.week,
                            Rank = itemRequest.rank,
                            Remark = itemRequest.remark,
                            Universe = itemRequest.universe,
                            Created_By = request.userID.GetValueOrDefault(),
                            Created_Date = dateNoew
                        };

                        listSaveData.Add(newAdminDetail);
                        listAddNewData.Add(newAdminDetail);
                    }
                }

                if (listAddNewData.Any())
                {
                    addDetailResult = await repository.adminKeyIn.AddAdminKeyInDetail(listAddNewData);
                }

                if (listUpdateData.Any())
                {
                    updateDetailResult = await repository.adminKeyIn.UpdateAdminKeyInDetail(listUpdateData);
                }

                if (updateDetailResult && addDetailResult)
                {
                    response.isSuccess = true;

                    #region Remove old adjust data
                    var groupDataKeyIn = listSaveData.GroupBy(
                        x => new
                        {
                            x.Year,
                            x.Month,
                            x.Week,
                            x.DistributionChannel_ID,
                            x.RetailerGroup_ID,
                            x.DepartmentStore_ID,
                            x.Universe
                        })
                        .Select(b => new
                        {
                            year = b.Key.Year,
                            month = b.Key.Month,
                            week = b.Key.Week,
                            DistributionChannel_ID = b.Key.DistributionChannel_ID,
                            RetailerGroup_ID = b.Key.RetailerGroup_ID,
                            DepartmentStore_ID = b.Key.DepartmentStore_ID,
                            Universe = b.Key.Universe,
                            brandKeyInData = b
                        });

                    foreach (var itemGrpup in groupDataKeyIn)
                    {
                        var adjustDataKeyIn = repository.adjust.FindAdjustDataBy(
                                 c => c.Year == itemGrpup.year
                                 && c.Month == itemGrpup.month
                                 && c.Week == itemGrpup.week
                                 && c.DistributionChannel_ID == itemGrpup.DistributionChannel_ID
                                 && c.DepartmentStore_ID == itemGrpup.DepartmentStore_ID
                                 && c.RetailerGroup_ID == itemGrpup.RetailerGroup_ID
                                 && c.Universe == itemGrpup.Universe);

                        var adjustStatusAdjusted = repository.masterData.GetAdjustStatusBy(e => e.Status_Name == "Adjusted");

                        if (adjustDataKeyIn != null && adjustDataKeyIn.Status_ID != adjustStatusAdjusted.ID)
                        {
                            var brandIDKeyIn = itemGrpup.brandKeyInData.Select(c => c.Brand_ID);

                            var adjustDataDetailListRemove = repository.adjust.GetAdjustDataDetaillBy(
                                e => e.AdjustData_ID == adjustDataKeyIn.ID
                                && brandIDKeyIn.Contains(e.Brand_ID));

                            await repository.adjust.RemoveAdjustDetai(adjustDataDetailListRemove);
                        }
                    }

                    #endregion
                }
                else
                {
                    response.isSuccess = false;
                }

            }
            catch (Exception ex)
            {
                response.responseError = ex.InnerException?.Message ?? ex.Message;
            }

            return response;
        }

        public GetAdminKeyInOptionResponse GetAdminKeyInOption()
        {
            GetAdminKeyInOptionResponse response = new GetAdminKeyInOptionResponse();

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

                int startYear = 2000;

                for (int i = startYear; i < Utility.GetDateNowThai().Year; i++)
                {
                    yearList.Add(i.ToString());
                }
                response.year = yearList.OrderByDescending(c => c).ToList();
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        #endregion

        #region Private Function
        private async Task<(bool, List<TTBAKeyInDetail>)> CreateBAKeyInDetail(CreateBAKeyInRequest request, Guid keyInID)
        {
            DateTime dateNow = Utility.GetDateNowThai();

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

                    var brandIDCounter = counterList.GroupBy(c => c.Brand_ID).Select(e => e.Key);
                    var brandDataList = repository.masterData.GetBrandListBy(c => brandIDCounter.Contains(c.Brand_ID));
                    var brandTypeList = repository.masterData.GetBrandTypeList().Where(e => e.Active_Flag);

                    foreach (var itemCounter in counterList)
                    {
                        var brandData = brandDataList.FirstOrDefault(c => c.Brand_ID == itemCounter.Brand_ID);
                        var brandTypeData = brandTypeList.FirstOrDefault(c => c.Brand_Type_ID == brandData.Brand_Type_ID);

                        if (brandTypeData?.Brand_Type_Name != "Fragrances"
                            //|| itemCounter.Alway_Show_Current_Year == true
                            )
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

                bool createBAKeyInDetailResponse;

                if (listBAKeyInDetail.Any())
                {
                    createBAKeyInDetailResponse = await repository.baKeyIn.CreateBAKeyInDetail(listBAKeyInDetail);
                }
                else
                {
                    createBAKeyInDetailResponse = true;
                }

                return (createBAKeyInDetailResponse, listBAKeyInDetail);
            }
            catch (Exception ex)
            {
                return (false, null);
            }
        }

        private async Task<bool> SaveBAKeyInDetailData(SaveBAKeyInDetailRequest request)
        {
            DateTime dateNoew = Utility.GetDateNowThai();

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

        private AdminKeyInDetailData GetAdminKeyInDetailData(CounterData itemCounter, GetAdminKeyInRequest request,
            List<TTAdminKeyInDetail> allAdminKeyInData, List<TTAdjustData> adjustDataPreviousYearWeek4, List<TTAdjustDataDetail> allAdjustDataDetail, List<TTBAKeyInDetail> allBAKeyInDetail)
        {
            var adminKeyInData = allAdminKeyInData.Where(
                c =>
                 c.DepartmentStore_ID == itemCounter.departmentStoreID
                && c.DistributionChannel_ID == itemCounter.distributionChannelID
                && c.Brand_ID == itemCounter.brandID);

            decimal? amountPreviousYear = null;
            var adminKeyInDetailPreviousYear = adjustDataPreviousYearWeek4.FirstOrDefault(
               c => c.DepartmentStore_ID == itemCounter.departmentStoreID
               && c.DistributionChannel_ID == itemCounter.distributionChannelID);

            if (adminKeyInDetailPreviousYear != null)
            {
                var adjustDataPreviousYearData = allAdjustDataDetail.Where(
                          p => p.AdjustData_ID == adminKeyInDetailPreviousYear.ID
                          && p.Brand_ID == itemCounter.brandID).OrderByDescending(e => e.Adjust_AmountSale).ToList();



                amountPreviousYear = adjustDataPreviousYearData != null && adjustDataPreviousYearData.Any() ? adjustDataPreviousYearData.Sum(c => c.Adjust_AmountSale) : null;
            }


            TTAdminKeyInDetail adminKeyInMostValue = null;
            if (adminKeyInData != null && adminKeyInData.Any())
            {
                adminKeyInMostValue = adminKeyInData.OrderByDescending(c => c.Amount_Sales).FirstOrDefault();
            }

            AdminKeyInDetailData dataDetail = new AdminKeyInDetailData
            {
                ID = adminKeyInData != null && adminKeyInData.Any() ? adminKeyInData.FirstOrDefault().ID : Guid.Empty,
                retailerGroupID = itemCounter.retailerGroupID,
                departmentStoreID = itemCounter.departmentStoreID,
                departmentStoreName = itemCounter.departmentStoreName,
                distributionChannelID = itemCounter.distributionChannelID,
                brandID = itemCounter.brandID,
                brandName = itemCounter.brandName,
                year = request.year,
                month = request.month,
                week = request.week,
                counterCreateDate = itemCounter.createDate,
                counterID = itemCounter.counterID,
                amountSale = adminKeyInData != null && adminKeyInData.Any() ? adminKeyInData.Sum(c => c.Amount_Sales) : null,
                wholeSale = adminKeyInData != null && adminKeyInData.Any() ? adminKeyInData.Sum(c => c.Whole_Sales) : null,
                fg = adminKeyInMostValue != null ? adminKeyInMostValue.FG : null,
                mu = adminKeyInMostValue != null ? adminKeyInMostValue.MU : null,
                ot = adminKeyInMostValue != null ? adminKeyInMostValue.OT : null,
                sk = adminKeyInMostValue != null ? adminKeyInMostValue.SK : null,
                rank = adminKeyInMostValue != null ? adminKeyInMostValue.Rank : null,
                remark = adminKeyInMostValue != null ? adminKeyInMostValue.Remark : null,
                universe = request.universe,
                amountSalePreviousYear = amountPreviousYear,
                alwayShow = itemCounter.alwayShow
            };

            return dataDetail;
        }
        #endregion
    }
}
