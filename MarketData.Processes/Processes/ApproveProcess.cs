using MarketData.Model.Data;
using MarketData.Model.Request.Approve;
using MarketData.Model.Response;
using MarketData.Model.Response.Approve;
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
                    response.data = approveKeyInData;
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
                string currentYear = DateTime.Now.Year.ToString();

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

                var BAKeyInData = repository.baKeyIn.FindBAKeyInBy(c => c.ID == keyInData.BAKeyIn_ID);
                var BAKeyInDetailList = repository.baKeyIn.GetBAKeyInDetailBy(c => c.BAKeyIn_ID == keyInData.BAKeyIn_ID);

                string previousYear = (Int32.Parse(BAKeyInData.Year) - 1).ToString();

                foreach (var itemBADetail in BAKeyInDetailList)
                {
                    var BAKeyInDetailPreviousYear = repository.baKeyIn.GetBAKeyInDetailListData(
                        c => c.DepartmentStore_ID == BAKeyInData.DepartmentStore_ID
                        && c.DistributionChannel_ID == BAKeyInData.DistributionChannel_ID
                        && c.Brand_ID == itemBADetail.brandID
                        && c.Year == previousYear
                        && c.Month == BAKeyInData.Month
                        && c.Week == "4").FirstOrDefault();

                    if (BAKeyInDetailPreviousYear != null)
                    {
                        itemBADetail.amountSalePreviousYear = BAKeyInDetailPreviousYear.Amount_Sales;
                    }
                }

                var brandBAData = repository.masterData.FindBrandBy(c => c.Brand_ID == BAKeyInData.Brand_ID);
                var departmentStoreData = repository.masterData.FindDepartmentStoreBy(c => c.Department_Store_ID == BAKeyInData.DepartmentStore_ID);
                var retailerGroupData = repository.masterData.FindRetailerGroupBy(c => c.Retailer_Group_ID == BAKeyInData.RetailerGroup_ID);
                var channelBAData = repository.masterData.FindDistributionChannelBy(c => c.Distribution_Channel_ID == BAKeyInData.DistributionChannel_ID);

                response.universe = BAKeyInData.Universe;
                response.status = repository.masterData.GetKeyInStatusBy(c => c.ID == BAKeyInData.KeyIn_Status_ID)?.Status_Name;
                response.brand = brandBAData?.Brand_Name;
                response.departmentStore = departmentStoreData?.Department_Store_Name;
                response.retailerGroup = retailerGroupData?.Retailer_Group_Name;
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
                    approveData.Action_Date = DateTime.Now;

                    var updateApproveResult = await repository.approve.UpdateApproveKeyInData(approveData);

                    if (updateApproveResult)
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
                    approveData.Action_Date = DateTime.Now;

                    var updateApproveResult = await repository.approve.UpdateApproveKeyInData(approveData);

                    if (updateApproveResult)
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
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }
    }
}
