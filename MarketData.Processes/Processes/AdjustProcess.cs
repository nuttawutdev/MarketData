using MarketData.Model.Data;
using MarketData.Model.Request.Adjust;
using MarketData.Model.Response.AdjustData;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            }
            catch(Exception ex)
            {

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
                string currentYear = DateTime.Now.Year.ToString();

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
    }
}
