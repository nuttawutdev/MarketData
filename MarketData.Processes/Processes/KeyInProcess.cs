using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Response.KeyIn;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    var baKeyInByCounter = repository.baKeyIn.GetBAKeyInBy(c=> c.DepartmentStore_ID == itemUserCounter.DepartmentStore_ID
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
            catch(Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }
    }
}
