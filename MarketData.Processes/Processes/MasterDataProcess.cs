﻿using ExcelDataReader;
using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Request;
using MarketData.Model.Request.MasterData;
using MarketData.Model.Response;
using MarketData.Model.Response.MasterData;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketData.Processes.Processes
{
    public class MasterDataProcess
    {
        private readonly Repository repository;

        public MasterDataProcess(Repository repository)
        {
            this.repository = repository;
        }

        #region Brand Type

        public GetBrandTypeListResponse GetBrandTypeList()
        {
            GetBrandTypeListResponse response = new GetBrandTypeListResponse();

            try
            {
                List<TMBrandType> dataList = repository.masterData.GetBrandTypeList();

                if (dataList.Any())
                {
                    response.data = dataList.Select(c => new BrandTypeData
                    {
                        brandTypeID = c.Brand_Type_ID,
                        brandTypeName = c.Brand_Type_Name,
                        active = c.Active_Flag,
                        createdDate = c.Created_Date
                    }).ToList();
                }
                else
                {
                    response.data = new List<BrandTypeData>();
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public BrandTypeData GetBrandTypeDetail(Guid brandTypeID)
        {
            BrandTypeData response = new BrandTypeData();

            try
            {
                var brandTypeData = repository.masterData.FindBrandTypeBy(c => c.Brand_Type_ID == brandTypeID);

                if (brandTypeData != null)
                {
                    response.brandTypeID = brandTypeData.Brand_Type_ID;
                    response.brandTypeName = brandTypeData.Brand_Type_Name;
                    response.active = brandTypeData.Active_Flag;
                    response.createdDate = brandTypeData.Created_Date;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return response;
        }

        public SaveDataResponse SaveBrandType(SaveBrandTypeRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var brandTypeByName = repository.masterData.FindBrandTypeBy(c => c.Brand_Type_Name.ToLower() == request.brandTypeName.ToLower());

                // Brand type name not exist Or Update old Brand type
                if (brandTypeByName == null || (brandTypeByName != null && brandTypeByName.Brand_Type_ID == request.brandTypeID))
                {
                    response.isSuccess = repository.masterData.SaveBrandType(request);
                }
                else
                {
                    response.isSuccess = false;
                    response.isDuplicated = true;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public SaveDataResponse DeleteBrandType(DeleteBrandTypeRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                response.isSuccess = repository.masterData.DeleteBrandType(request);

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        #endregion

        #region Brand Segment

        public GetBrandSegmentListResponse GetBrandSegmentList()
        {
            GetBrandSegmentListResponse response = new GetBrandSegmentListResponse();

            try
            {
                List<TMBrandSegment> dataList = repository.masterData.GetBrandSegmentList();

                if (dataList.Any())
                {
                    response.data = dataList.Select(c => new BrandSegmentData
                    {
                        brandSegmentID = c.Brand_Segment_ID,
                        brandSegmentName = c.Brand_Segment_Name,
                        active = c.Active_Flag,
                        createdDate = c.Created_Date
                    }).ToList();
                }
                else
                {
                    response.data = new List<BrandSegmentData>();
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public BrandSegmentData GetBrandSegmentDetail(Guid brandSegmentID)
        {
            BrandSegmentData response = new BrandSegmentData();

            try
            {
                var brandSegmentData = repository.masterData.FindBrandSegmentBy(c => c.Brand_Segment_ID == brandSegmentID);

                if (brandSegmentData != null)
                {
                    response.brandSegmentID = brandSegmentData.Brand_Segment_ID;
                    response.brandSegmentName = brandSegmentData.Brand_Segment_Name;
                    response.active = brandSegmentData.Active_Flag;
                    response.createdDate = brandSegmentData.Created_Date;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return response;
        }

        public SaveDataResponse SaveBrandSegment(SaveBrandSegmentRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var brandSegmentByName = repository.masterData.FindBrandSegmentBy(c => c.Brand_Segment_Name.ToLower() == request.brandSegmentName.ToLower());

                // Brand segment name not exist Or Update old Brand segment
                if (brandSegmentByName == null || (brandSegmentByName != null && brandSegmentByName.Brand_Segment_ID == request.brandSegmentID))
                {
                    response.isSuccess = repository.masterData.SaveBrandSegment(request);
                }
                else
                {
                    response.isSuccess = false;
                    response.isDuplicated = true;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public SaveDataResponse DeleteBrandSegment(DeleteBrandSegmentRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                response.isSuccess = repository.masterData.DeleteBrandSegment(request);

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        #endregion

        #region Brand Group

        public GetBrandGroupListResponse GetBrandGroupList()
        {
            GetBrandGroupListResponse response = new GetBrandGroupListResponse();

            try
            {
                List<TMBrandGroup> dataList = repository.masterData.GetBrandGroupList();

                if (dataList.Any())
                {
                    response.data = dataList.Select(c => new BrandGroupData
                    {
                        brandGroupID = c.Brand_Group_ID,
                        brandGroupName = c.Brand_Group_Name,
                        isLoreal = c.Is_Loreal_Brand,
                        active = c.Active_Flag,
                        createdDate = c.Created_Date
                    }).ToList();
                }
                else
                {
                    response.data = new List<BrandGroupData>();
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public BrandGroupData GetBrandGroupDetail(Guid brandGroupID)
        {
            BrandGroupData response = new BrandGroupData();

            try
            {
                var brandGroupData = repository.masterData.FindBrandGroupBy(c => c.Brand_Group_ID == brandGroupID);

                if (brandGroupData != null)
                {
                    response.brandGroupID = brandGroupData.Brand_Group_ID;
                    response.brandGroupName = brandGroupData.Brand_Group_Name;
                    response.isLoreal = brandGroupData.Is_Loreal_Brand;
                    response.active = brandGroupData.Active_Flag;
                    response.createdDate = brandGroupData.Created_Date;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return response;
        }

        public SaveDataResponse SaveBrandGroup(SaveBrandGroupRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var brandGroupByName = repository.masterData.FindBrandGroupBy(c => c.Brand_Group_Name.ToLower() == request.brandGroupName.ToLower());

                // Brand group name not exist Or Update old Brand group
                if (brandGroupByName == null || (brandGroupByName != null && brandGroupByName.Brand_Group_ID == request.brandGroupID))
                {
                    response.isSuccess = repository.masterData.SaveBrandGroup(request);
                }
                else
                {
                    response.isSuccess = false;
                    response.isDuplicated = true;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public SaveDataResponse DeleteBrandGroup(DeleteBrandGroupRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                response.isSuccess = repository.masterData.DeleteBrandGroup(request);

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        #endregion

        #region Brand

        public GetBrandListResponse GetBrandList()
        {
            GetBrandListResponse response = new GetBrandListResponse();

            try
            {
                List<BrandData> dataList = repository.masterData.GetBrandList();

                if (dataList.Any())
                {
                    response.data = dataList;
                }
                else
                {
                    response.data = new List<BrandData>();
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public GetBrandDetailResponse GetBrandDetail(Guid brandID)
        {

            GetBrandDetailResponse response = null;

            try
            {
                var brandData = repository.masterData.FindBrandBy(c => c.Brand_ID == brandID);

                if (brandData != null)
                {
                    response = new GetBrandDetailResponse
                    {
                        brandID = brandData.Brand_ID,
                        brandName = brandData.Brand_Name,
                        brandShortName = brandData.Brand_Short_Name,
                        brandGroupID = brandData.Brand_Group_ID,
                        brandSegmentID = brandData.Brand_Segment_ID,
                        brandColor = brandData.Brand_Color,
                        active = brandData.Active_Flag,
                        brandTypeID = brandData.Brand_Type_ID,
                        lorealBrandRank = brandData.Loreal_Brand_Rank,
                        universe = brandData.Universe
                    };
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return response;
        }

        public async Task<SaveDataResponse> SaveBrand(SaveBrandRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var brandByName = repository.masterData.FindBrandBy(c => c.Brand_Name.ToLower() == request.brandName.ToLower());
                var branGroupData = repository.masterData.FindBrandGroupBy(e => e.Brand_Group_ID == request.brandGroupID);

                // Brand name not exist Or Update old Brand
                if (brandByName == null || (brandByName != null && brandByName.Brand_ID == request.brandID))
                {
                    if (branGroupData != null && branGroupData.Is_Loreal_Brand)
                    {
                        var brandByShortName = repository.masterData.FindBrandBy(c => c.Brand_Short_Name.ToLower() == request.brandShortName.ToLower());
                        var brandByColor = repository.masterData.FindBrandBy(c => c.Brand_Color == request.brandColor);

                        if ((brandByShortName == null || (brandByShortName != null && brandByShortName.Brand_ID == request.brandID)) ||
                            (brandByColor == null || (brandByColor != null && brandByColor.Brand_ID == request.brandID)))
                        {
                            response.isSuccess = await repository.masterData.SaveBrand(request);
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.isDuplicated = true;
                        }
                    }
                    else
                    {
                        response.isSuccess = await repository.masterData.SaveBrand(request);
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.isDuplicated = true;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public SaveDataResponse DeleteBrand(DeleteBrandRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                response.isSuccess = repository.masterData.DeleteBrand(request);
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public async Task<ImportDataResponse> ImportBrandData(ImportDataRequest request)
        {
            ImportDataResponse response = new ImportDataResponse();

            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                List<SaveBrandRequest> saveBrandList = new List<SaveBrandRequest>();

                using (var stream = System.IO.File.Open(request.filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read()) //Each row of the file
                        {
                            // Validate Column File
                            if (reader.Depth == 0)
                            {
                                string column1 = reader.GetValue(0)?.ToString();
                                string column2 = reader.GetValue(1)?.ToString();
                                string column3 = reader.GetValue(2)?.ToString();
                                string column4 = reader.GetValue(3)?.ToString();
                                string column5 = reader.GetValue(4)?.ToString();
                                string column6 = reader.GetValue(5)?.ToString();

                                if (column1 != "Name" ||
                                    column2 != "Short name" ||
                                    column3 != "Brand_Group" ||
                                    column4 != "Segment" ||
                                    column5 != "Type" ||
                                    column6 != "BrandColor")
                                {
                                    response.isSuccess = false;
                                    response.wrongFormatFile = true;
                                }
                            }

                            if (reader.Depth != 0)
                            {
                                saveBrandList.Add(new SaveBrandRequest
                                {
                                    brandName = reader.GetValue(0)?.ToString(),
                                    brandShortName = reader.GetValue(1)?.ToString(),
                                    brandGroupName = reader.GetValue(2)?.ToString(),
                                    brandSegmentName = reader.GetValue(3)?.ToString(),
                                    brandTypeName = reader.GetValue(4)?.ToString(),
                                    brandColor = reader.GetValue(5)?.ToString()
                                });
                            }
                        }
                    }
                }

                var groupByBrandGroup = saveBrandList.Where(e => !string.IsNullOrWhiteSpace(e.brandGroupName))
                    .GroupBy(c => c.brandGroupName);

                var groupByBrandSegment = saveBrandList.Where(e => !string.IsNullOrWhiteSpace(e.brandSegmentName))
                   .GroupBy(c => c.brandSegmentName);

                var groupByBrandType = saveBrandList.Where(e => !string.IsNullOrWhiteSpace(e.brandTypeName))
                   .GroupBy(c => c.brandTypeName);

                var brandGroupData = new Dictionary<string, Guid>();
                var brandSegmentData = new Dictionary<string, Guid>();
                var brandTypeData = new Dictionary<string, Guid>();

                foreach (var itemBrandGroup in groupByBrandGroup)
                {
                    var brandGroupByName = repository.masterData.FindBrandGroupBy(c => c.Brand_Group_Name.ToLower() == itemBrandGroup.Key.ToLower());

                    if (brandGroupByName == null)
                    {
                        SaveBrandGroupRequest saveBrandGroupRequest = new SaveBrandGroupRequest
                        {
                            brandGroupName = itemBrandGroup.Key,
                            active = true,
                            isLoreal = !string.IsNullOrWhiteSpace(itemBrandGroup.FirstOrDefault().brandColor),
                            userID = request.userID
                        };

                        var createBrandGroupResult = repository.masterData.CreateBrandGroup(saveBrandGroupRequest);

                        if (createBrandGroupResult != null)
                        {
                            brandGroupData.Add(itemBrandGroup.Key, createBrandGroupResult.Brand_Group_ID);
                        }
                    }
                    else
                    {
                        brandGroupData.Add(itemBrandGroup.Key, brandGroupByName.Brand_Group_ID);
                    }
                }

                foreach (var itemBrandSegment in groupByBrandSegment)
                {
                    var brandSegmentByName = repository.masterData.FindBrandSegmentBy(c => c.Brand_Segment_Name.ToLower() == itemBrandSegment.Key.ToLower());

                    if (brandSegmentByName == null)
                    {
                        SaveBrandSegmentRequest saveBrandSegmentRequest = new SaveBrandSegmentRequest
                        {
                            brandSegmentName = itemBrandSegment.Key,
                            active = true,
                            userID = request.userID
                        };

                        var createBrandSegmentResult = repository.masterData.CreateBrandSegment(saveBrandSegmentRequest);

                        if (createBrandSegmentResult != null)
                        {
                            brandSegmentData.Add(itemBrandSegment.Key, createBrandSegmentResult.Brand_Segment_ID);
                        }
                    }
                    else
                    {
                        brandSegmentData.Add(itemBrandSegment.Key, brandSegmentByName.Brand_Segment_ID);
                    }
                }

                foreach (var itemBrandType in groupByBrandType)
                {
                    var brandTypetByName = repository.masterData.FindBrandTypeBy(c => c.Brand_Type_Name.ToLower() == itemBrandType.Key.ToLower());

                    if (brandTypetByName == null)
                    {
                        SaveBrandTypeRequest saveBrandTypeRequest = new SaveBrandTypeRequest
                        {
                            brandTypeName = itemBrandType.Key,
                            active = true,
                            userID = request.userID
                        };

                        var createBrandTypeResult = repository.masterData.CreateBrandType(saveBrandTypeRequest);

                        if (createBrandTypeResult != null)
                        {
                            brandTypeData.Add(itemBrandType.Key, createBrandTypeResult.Brand_Type_ID);
                        }
                    }
                    else
                    {
                        brandTypeData.Add(itemBrandType.Key, brandTypetByName.Brand_Type_ID);
                    }
                }

                foreach (var saveBrandRequest in saveBrandList)
                {
                    saveBrandRequest.brandGroupID = brandGroupData.FirstOrDefault(c => c.Key == saveBrandRequest.brandGroupName).Value;
                    saveBrandRequest.brandSegmentID = brandSegmentData.FirstOrDefault(c => c.Key == saveBrandRequest.brandSegmentName).Value;
                    saveBrandRequest.brandTypeID = brandTypeData.FirstOrDefault(c => c.Key == saveBrandRequest.brandTypeName).Value;
                    saveBrandRequest.active = true;
                    saveBrandRequest.userID = request.userID;

                    await SaveBrand(saveBrandRequest);
                }

                response.isSuccess = true;
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        #endregion

        #region Retailer Group

        public GetRetailerGroupListResponse GetRetailerGroupList()
        {
            GetRetailerGroupListResponse response = new GetRetailerGroupListResponse();

            try
            {
                var dataList = repository.masterData.GetRetailerGroupList();

                if (dataList.Any())
                {
                    response.data = dataList.Select(c => new RetailerGroupData
                    {
                        retailerGroupID = c.Retailer_Group_ID,
                        retailerGroupName = c.Retailer_Group_Name,
                        active = c.Active_Flag,
                        createdDate = c.Created_Date
                    }).ToList();
                }
                else
                {
                    response.data = new List<RetailerGroupData>();
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public RetailerGroupData GetRetailerGroupDetail(Guid retailerGroupID)
        {
            RetailerGroupData response = new RetailerGroupData();

            try
            {
                var retailGroupData = repository.masterData.FindRetailerGroupBy(c => c.Retailer_Group_ID == retailerGroupID);

                if (retailGroupData != null)
                {
                    response.retailerGroupID = retailGroupData.Retailer_Group_ID;
                    response.retailerGroupName = retailGroupData.Retailer_Group_Name;
                    response.active = retailGroupData.Active_Flag;
                    response.createdDate = retailGroupData.Created_Date;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return response;
        }

        public async Task<SaveDataResponse> SaveRetailerGroup(SaveRetailerGroupRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var retailerGroupByName = repository.masterData.FindRetailerGroupBy(c => c.Retailer_Group_Name.ToLower() == request.retailerGroupName.ToLower());

                // Retailer group name not exist Or Update old Retailer group
                if (retailerGroupByName == null || (retailerGroupByName != null && retailerGroupByName.Retailer_Group_ID == request.retailerGroupID))
                {
                    response.isSuccess = await repository.masterData.SaveRetailerGroup(request);
                }
                else
                {
                    response.isSuccess = false;
                    response.isDuplicated = true;
                    response.responseError = "Retailer group name is duplicated";
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public SaveDataResponse DeleteRetailerGroup(DeleteRetailerGroupRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                response.isSuccess = repository.masterData.DeleteRetailerGroup(request);
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        #endregion

        #region Distribution Channel

        public GetDistributionChannelListResponse GetDistributionChannelList()
        {
            GetDistributionChannelListResponse response = new GetDistributionChannelListResponse();

            try
            {
                var dataList = repository.masterData.GetDistributionChannelList();

                if (dataList.Any())
                {
                    response.data = dataList.Select(c => new DistributionChannelData
                    {
                        distributionChannelID = c.Distribution_Channel_ID,
                        distributionChannelName = c.Distribution_Channel_Name,
                        active = c.Active_Flag,
                        createdDate = c.Created_Date
                    }).ToList();
                }
                else
                {
                    response.data = new List<DistributionChannelData>();
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public DistributionChannelData GetDistributionChannelDetail(Guid distributionChannelD)
        {
            DistributionChannelData response = new DistributionChannelData();

            try
            {
                var retailGroupData = repository.masterData.FindDistributionChannelBy(c => c.Distribution_Channel_ID == distributionChannelD);

                if (retailGroupData != null)
                {
                    response.distributionChannelID = retailGroupData.Distribution_Channel_ID;
                    response.distributionChannelName = retailGroupData.Distribution_Channel_Name;
                    response.active = retailGroupData.Active_Flag;
                    response.createdDate = retailGroupData.Created_Date;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return response;
        }

        public async Task<SaveDataResponse> SaveDistributionChannel(SaveDistributionChannelRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var distributionChannelByName = repository.masterData.FindDistributionChannelBy(c => c.Distribution_Channel_Name.ToLower() == request.distributionChannelName.ToLower());

                // Channel name not exist Or Update old Channel
                if (distributionChannelByName == null ||
                    (distributionChannelByName != null && distributionChannelByName.Distribution_Channel_ID == request.distributionChannelID))
                {
                    response.isSuccess = await repository.masterData.SaveDistributionChannel(request);
                }
                else
                {
                    response.isSuccess = false;
                    response.isDuplicated = true;
                    response.responseError = "Distribution channel name is duplicated";
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> DeleteDistributionChannel(DeleteDistributionChannelRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                response.isSuccess = await repository.masterData.DeleteDistributionChannel(request);
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        #endregion

        #region Department Store

        public GetDepartmentStoreListResponse GetDepartmentStoreList()
        {
            GetDepartmentStoreListResponse response = new GetDepartmentStoreListResponse();

            try
            {
                var dataList = repository.masterData.GetDepartmentStoreList();

                if (dataList.Any())
                {
                    response.data = dataList.Select(c => new DepartmentStoreData
                    {
                        departmentStoreID = c.departmentStoreID,
                        departmentStoreName = c.departmentStoreName,
                        distributionChannelName = c.distributionChannelName,
                        retailerGroupName = c.retailerGroupName,
                        rank = c.rank,
                        active = c.active,
                        region = c.region
                    }).ToList();
                }
                else
                {
                    response.data = new List<DepartmentStoreData>();
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public DepartmentStoreData GetDepartmentStoreDetail(Guid departmentStoreID)
        {
            DepartmentStoreData response = new DepartmentStoreData();

            try
            {
                var departmentStoreData = repository.masterData.FindDepartmentStoreBy(c => c.Department_Store_ID == departmentStoreID);

                if (departmentStoreData != null)
                {
                    response.departmentStoreID = departmentStoreData.Department_Store_ID;
                    response.departmentStoreName = departmentStoreData.Department_Store_Name;
                    response.retailerGroupID = departmentStoreData.Retailer_Group_ID;
                    response.distributionChannelID = departmentStoreData.Distribution_Channel_ID;
                    response.regionID = departmentStoreData.Region_ID;
                    response.rank = departmentStoreData.Rank;
                    response.active = departmentStoreData.Active_Flag;
                    response.createdDate = departmentStoreData.Created_Date;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return response;
        }

        public async Task<SaveDataResponse> SaveDepartmentStore(SaveDepsrtmentStoreRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var departmentStoreByName = repository.masterData.FindDepartmentStoreBy(c => c.Department_Store_Name.ToLower() == request.departmentStoreName.ToLower());

                // Department Store name not exist Or Update old Department Store
                if (departmentStoreByName == null ||
                    (departmentStoreByName != null && departmentStoreByName.Department_Store_ID == request.departmentStoreID))
                {
                    response.isSuccess = await repository.masterData.SaveDepartmentStore(request);
                }
                else
                {
                    response.isSuccess = false;
                    response.isDuplicated = true;
                    response.responseError = "Department Store name is duplicated";
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> DeleteDepartmentStore(DeleteDepartmentStoreRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                response.isSuccess = await repository.masterData.DeleteDepartmentStore(request);
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public async Task<ImportDataResponse> ImportDepartmentStoreData(ImportDataRequest request)
        {
            ImportDataResponse response = new ImportDataResponse();

            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                List<SaveDepsrtmentStoreRequest> saveDepartmentStoreList = new List<SaveDepsrtmentStoreRequest>();

                using (var stream = System.IO.File.Open(request.filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read()) //Each row of the file
                        {
                            // Validate Column File
                            if (reader.Depth == 0)
                            {
                                string column1 = reader.GetValue(0)?.ToString();
                                string column2 = reader.GetValue(1)?.ToString();
                                string column3 = reader.GetValue(2)?.ToString();
                                string column4 = reader.GetValue(3)?.ToString();
                                string column5 = reader.GetValue(4)?.ToString();

                                if (column1 != "DepartmentStores" ||
                                    column2 != "Regions" ||
                                    column3 != "DepartmentStores_Group" ||
                                    column4 != "Rank" ||
                                    column5 != "Distribution_Channels")
                                {
                                    response.isSuccess = false;
                                    response.wrongFormatFile = true;
                                }
                            }

                            if (reader.Depth != 0)
                            {
                                SaveDepsrtmentStoreRequest saveStore = new SaveDepsrtmentStoreRequest
                                {
                                    departmentStoreName = reader.GetValue(0)?.ToString(),
                                    region = reader.GetValue(1)?.ToString(),
                                    retailerGroupName = reader.GetValue(2)?.ToString(),
                                    distributionChannelName = reader.GetValue(4)?.ToString()
                                };

                                int rank = 0;
                                int.TryParse(reader.GetValue(3)?.ToString(), out rank);

                                if (rank != 0)
                                {
                                    saveStore.rank = rank;
                                }

                                saveDepartmentStoreList.Add(saveStore);
                            }
                        }
                    }
                }

                var groupByRetailerGroup = saveDepartmentStoreList.Where(e => !string.IsNullOrWhiteSpace(e.retailerGroupName))
                    .GroupBy(c => c.retailerGroupName);

                var groupByDistrubution = saveDepartmentStoreList.Where(e => !string.IsNullOrWhiteSpace(e.distributionChannelName))
                   .GroupBy(c => c.distributionChannelName);

                var retailerGroupData = new Dictionary<string, Guid>();
                var distributionChannelData = new Dictionary<string, Guid>();

                foreach (var itemRetailerGroup in groupByRetailerGroup)
                {
                    var retailerGroupByName = repository.masterData.FindRetailerGroupBy(c => c.Retailer_Group_Name.ToLower() == itemRetailerGroup.Key.ToLower());

                    if (retailerGroupByName == null)
                    {
                        SaveRetailerGroupRequest saveRetailerGroupRequest = new SaveRetailerGroupRequest
                        {
                            retailerGroupName = itemRetailerGroup.Key,
                            active = true,
                            userID = request.userID
                        };

                        var createRetailerGroupResult = await repository.masterData.CreateRetailerGroup(saveRetailerGroupRequest);

                        if (createRetailerGroupResult != null)
                        {
                            retailerGroupData.Add(itemRetailerGroup.Key, createRetailerGroupResult.Retailer_Group_ID);
                        }
                    }
                    else
                    {
                        retailerGroupData.Add(itemRetailerGroup.Key, retailerGroupByName.Retailer_Group_ID);
                    }
                }

                foreach (var itemChannel in groupByDistrubution)
                {
                    var channelByName = repository.masterData.FindDistributionChannelBy(c => c.Distribution_Channel_Name.ToLower() == itemChannel.Key.ToLower());

                    if (channelByName == null)
                    {
                        SaveDistributionChannelRequest saveChannelRequest = new SaveDistributionChannelRequest
                        {
                            distributionChannelName = itemChannel.Key,
                            active = true,
                            userID = request.userID
                        };

                        var createChannelResult = await repository.masterData.CreateDistributionChannel(saveChannelRequest);

                        if (createChannelResult != null)
                        {
                            distributionChannelData.Add(itemChannel.Key, createChannelResult.Distribution_Channel_ID);
                        }
                    }
                    else
                    {
                        distributionChannelData.Add(itemChannel.Key, channelByName.Distribution_Channel_ID);
                    }
                }

                foreach (var saveDepartmentStoreRequest in saveDepartmentStoreList)
                {
                    saveDepartmentStoreRequest.retailerGroupID = retailerGroupData.FirstOrDefault(c => c.Key == saveDepartmentStoreRequest.departmentStoreName).Value;
                    saveDepartmentStoreRequest.distributionChannelID = distributionChannelData.FirstOrDefault(c => c.Key == saveDepartmentStoreRequest.distributionChannelName).Value;
                    saveDepartmentStoreRequest.active = true;
                    saveDepartmentStoreRequest.userID = request.userID;

                    await SaveDepartmentStore(saveDepartmentStoreRequest);
                }

                response.isSuccess = true;
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        #endregion
    }
}
