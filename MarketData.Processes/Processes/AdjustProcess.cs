using MarketData.Helper;
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
                var departmentStoreList = repository.masterData.GetDepartmentStoreList().Where(e => e.active);
                var departmentStoreByFilter = departmentStoreList.Where(
                    c => (!request.departmentStoreID.HasValue || (request.departmentStoreID.HasValue && request.departmentStoreID == c.departmentStoreID))
                    && (!request.retailerGroupID.HasValue || (request.retailerGroupID.HasValue && request.retailerGroupID == c.retailerGroupID))
                    && (!request.distributionChannelID.HasValue || (request.distributionChannelID.HasValue && request.distributionChannelID == c.distributionChannelID)));

                var departmentSoteIDList = departmentStoreByFilter.Select(c => c.departmentStoreID);
                var counterByDepartmentStoreList = repository.masterData.GetCounterListBy(e => departmentSoteIDList.Contains(e.Department_Store_ID));
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

                    if(adjustStatusData != null)
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

        public GetAdjustDetailResponse GetAdjustDataDetail(GetAdjustDetailRequest request)
        {
            GetAdjustDetailResponse response = new GetAdjustDetailResponse();
            return response;
        }
    }
}
