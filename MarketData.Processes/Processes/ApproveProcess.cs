using MarketData.Model.Data;
using MarketData.Model.Response.Approve;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
