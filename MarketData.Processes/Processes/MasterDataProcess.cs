using ExcelDataReader;
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

        public async Task<SaveDataResponse> SaveBrandType(SaveBrandTypeRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                request.brandTypeName = request.brandTypeName.Trim();

                var brandTypeByName = repository.masterData.FindBrandTypeBy(c =>
                c.Brand_Type_Name.ToLower() == request.brandTypeName.ToLower() && c.Delete_Flag != true);

                // Brand type name not exist Or Update old Brand type
                if (brandTypeByName == null || (brandTypeByName != null && brandTypeByName.Brand_Type_ID == request.brandTypeID))
                {
                    response.isSuccess = await repository.masterData.SaveBrandType(request);
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

        public async Task<SaveDataResponse> SaveBrandSegment(SaveBrandSegmentRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                request.brandSegmentName = request.brandSegmentName.Trim();

                var brandSegmentByName = repository.masterData.FindBrandSegmentBy(c =>
                c.Brand_Segment_Name.ToLower() == request.brandSegmentName.ToLower()
                && c.Delete_Flag != true);

                // Brand segment name not exist Or Update old Brand segment
                if (brandSegmentByName == null || (brandSegmentByName != null && brandSegmentByName.Brand_Segment_ID == request.brandSegmentID))
                {
                    response.isSuccess = await repository.masterData.SaveBrandSegment(request);
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

        public async Task<SaveDataResponse> SaveBrandGroup(SaveBrandGroupRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                request.brandGroupName = request.brandGroupName.Trim();

                var brandGroupByName = repository.masterData.FindBrandGroupBy
                    (c => c.Brand_Group_Name.ToLower() == request.brandGroupName.ToLower()
                    && c.Delete_Flag != true);

                // Brand group name not exist Or Update old Brand group
                if (brandGroupByName == null || (brandGroupByName != null && brandGroupByName.Brand_Group_ID == request.brandGroupID))
                {
                    response.isSuccess = await repository.masterData.SaveBrandGroup(request);
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

        public BrandData GetBrandDetail(Guid brandID)
        {

            BrandData response = null;

            try
            {
                var brandData = repository.masterData.FindBrandBy(c => c.Brand_ID == brandID);

                if (brandData != null)
                {
                    response = new BrandData
                    {
                        brandID = brandData.Brand_ID,
                        brandName = brandData.Brand_Name,
                        brandShortName = brandData.Brand_Short_Name,
                        brandGroupID = brandData.Brand_Group_ID,
                        brandSegmentID = brandData.Brand_Segment_ID,
                        color = brandData.Brand_Color,
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
                request.brandName = request.brandName.Trim();

                var brandByName = repository.masterData.FindBrandBy(c => c.Brand_Name.ToLower() == request.brandName.ToLower()
                && c.Delete_Flag != true);

                var brrandGroupSelect = repository.masterData.FindBrandGroupBy(c => c.Brand_Group_ID == request.brandGroupID);

                if (brrandGroupSelect.Is_Loreal_Brand)
                {
                    if (string.IsNullOrWhiteSpace(request.brandShortName))
                    {
                        response.isSuccess = false;
                        response.responseError = "Short name is required on Loreal Brand group.";
                        return response;
                    }

                    if (!request.lorealBrandRank.HasValue)
                    {
                        response.isSuccess = false;
                        response.responseError = "Loreal brand rank is required on Loreal Brand group.";
                        return response;
                    }
                }


                TMBrand brandByShortName = null;
                TMBrand brandByColor = null;

                if (!string.IsNullOrWhiteSpace(request.brandShortName))
                {
                    request.brandShortName = request.brandShortName.Trim();

                    brandByShortName = repository.masterData.FindBrandBy(c => c.Brand_Short_Name != null && c.Brand_Short_Name.ToLower() == request.brandShortName.ToLower()
                                        && c.Delete_Flag != true);
                }

                if (request.brandColor != "#ffffff" && !string.IsNullOrWhiteSpace(request.brandColor))
                {
                    brandByColor = repository.masterData.FindBrandBy(c => !string.IsNullOrWhiteSpace(c.Brand_Color) && c.Brand_Color == request.brandColor && c.Delete_Flag != true);
                }

                // Brand name not exist Or Update old Brand
                if ((brandByName == null || (brandByName != null && brandByName.Brand_ID == request.brandID))
                    && (brandByShortName == null || (brandByShortName != null && brandByShortName.Brand_ID == request.brandID))
                    && (brandByColor == null || (brandByColor != null && brandByColor.Brand_ID == request.brandID)))
                {
                    response.isSuccess = await repository.masterData.SaveBrand(request);
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

                List<ImportResultByRowResponse> importResult = new List<ImportResultByRowResponse>();

                using (var reader = ExcelReaderFactory.CreateReader(request.fileStream))
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

                            if ((column1 != null && column1.ToLower() != "name") ||
                                (column2 != null && column2.ToLower() != "short name") ||
                                (column3 != null && column3.ToLower() != "brand_group") ||
                                (column4 != null && column4.ToLower() != "segment") ||
                                (column5 != null && column5.ToLower() != "type") ||
                                (column6 != null && column6.ToLower() != "brand color"))
                            {
                                response.isSuccess = false;
                                response.wrongFormatFile = true;
                                response.responseError = "Wrong format file!";
                                return response;
                            }
                        }

                        if (reader.Depth != 0)
                        {
                            string brandNameValue = reader.GetValue(0)?.ToString();
                            string brandShortName = reader.GetValue(1)?.ToString();
                            string brandGroupValue = reader.GetValue(2)?.ToString();
                            string brandSegmentValue = reader.GetValue(3)?.ToString();
                            string brandTypeValue = reader.GetValue(4)?.ToString();
                            string brandColor = reader.GetValue(5)?.ToString();

                            if (!string.IsNullOrWhiteSpace(brandNameValue) && !string.IsNullOrWhiteSpace(brandGroupValue)
                                && !string.IsNullOrWhiteSpace(brandSegmentValue) && !string.IsNullOrWhiteSpace(brandTypeValue))
                            {
                                saveBrandList.Add(new SaveBrandRequest
                                {
                                    brandName = brandNameValue,
                                    brandShortName = brandShortName,
                                    brandGroupName = brandGroupValue,
                                    brandSegmentName = brandSegmentValue,
                                    brandTypeName = brandTypeValue,
                                    brandColor = brandColor,
                                    row = reader.Depth + 1
                                });
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(brandNameValue) || !string.IsNullOrWhiteSpace(brandGroupValue)
                                    || !string.IsNullOrWhiteSpace(brandSegmentValue) || !string.IsNullOrWhiteSpace(brandTypeValue)
                                    || !string.IsNullOrWhiteSpace(brandShortName) || !string.IsNullOrWhiteSpace(brandColor))
                                {

                                    int row = reader.Depth + 1;
                                    ImportResultByRowResponse resultImport = new ImportResultByRowResponse
                                    {
                                        row = reader.Depth + 1,
                                        result = $"Row {row} required value in column "
                                    };

                                    if (string.IsNullOrWhiteSpace(brandNameValue))
                                    {
                                        resultImport.result += "Name, ";
                                    }

                                    if (string.IsNullOrWhiteSpace(brandGroupValue))
                                    {
                                        resultImport.result += "Brand_Group, ";
                                    }
                                    if (string.IsNullOrWhiteSpace(brandSegmentValue))
                                    {
                                        resultImport.result += "Segment, ";
                                    }
                                    if (string.IsNullOrWhiteSpace(brandTypeValue))
                                    {
                                        resultImport.result += "Type, ";
                                    }

                                    importResult.Add(resultImport);
                                    response.countImportFailed = response.countImportFailed + 1;
                                }
                            }
                        }
                    }
                }

                if (saveBrandList.Any())
                {
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
                        var brandGroupByName = repository.masterData.FindBrandGroupBy(
                            c => c.Brand_Group_Name.ToLower() == itemBrandGroup.Key.ToLower()
                            && c.Delete_Flag != true);

                        if (brandGroupByName == null)
                        {
                            SaveBrandGroupRequest saveBrandGroupRequest = new SaveBrandGroupRequest
                            {
                                brandGroupName = itemBrandGroup.Key,
                                active = true,
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
                        var brandSegmentByName = repository.masterData.FindBrandSegmentBy(
                            c => c.Brand_Segment_Name.ToLower() == itemBrandSegment.Key.ToLower()
                            && c.Delete_Flag != true);

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
                        var brandTypetByName = repository.masterData.FindBrandTypeBy(
                            c => c.Brand_Type_Name.ToLower() == itemBrandType.Key.ToLower()
                            && c.Delete_Flag != true);

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

                        if (!string.IsNullOrWhiteSpace(saveBrandRequest.brandName)
                            && saveBrandRequest.brandGroupID != Guid.Empty
                            && saveBrandRequest.brandSegmentID != Guid.Empty
                            && saveBrandRequest.brandTypeID != Guid.Empty)
                        {
                            var result = await SaveBrand(saveBrandRequest);

                            if (result.isSuccess)
                            {
                                response.countImportSuccess = response.countImportSuccess + 1;
                            }
                            else
                            {
                                ImportResultByRowResponse resultImport = new ImportResultByRowResponse
                                {
                                    row = saveBrandRequest.row
                                };

                                if (result.isDuplicated == true)
                                {
                                    resultImport.result = $"Row {saveBrandRequest.row} data is duplicate.";
                                }
                                else
                                {
                                    resultImport.result = $"Row {saveBrandRequest.row} save data failed.";
                                }

                                importResult.Add(resultImport);
                                response.countImportFailed = response.countImportFailed + 1;
                            }
                        }
                        else
                        {
                            ImportResultByRowResponse resultImport = new ImportResultByRowResponse
                            {
                                row = saveBrandRequest.row,
                            };

                            resultImport.result = $"Row {saveBrandRequest.row} required value in column ";

                            if (string.IsNullOrWhiteSpace(saveBrandRequest.brandName))
                            {
                                resultImport.result += "Name ";
                            }

                            if (saveBrandRequest.brandGroupID == Guid.Empty)
                            {
                                resultImport.result += "Brand_Group ";
                            }

                            if (saveBrandRequest.brandSegmentID == Guid.Empty)
                            {
                                resultImport.result += "Segment ";
                            }

                            if (saveBrandRequest.brandTypeID == Guid.Empty)
                            {
                                resultImport.result += "Type ";
                            }

                            importResult.Add(resultImport);
                            response.countImportFailed = response.countImportFailed + 1;
                        }
                    }

                    response.importResult = importResult.OrderBy(c => c.row).ToList();

                    if (response.countImportSuccess > 0)
                    {
                        response.isSuccess = true;
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.wrongFormatFile = true;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
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
                request.retailerGroupName = request.retailerGroupName.Trim();

                var retailerGroupByName = repository.masterData.FindRetailerGroupBy(
                    c => c.Retailer_Group_Name.ToLower() == request.retailerGroupName.ToLower()
                    && c.Delete_Flag != true);

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
                request.distributionChannelName = request.distributionChannelName.Trim();

                var distributionChannelByName = repository.masterData.FindDistributionChannelBy(
                    c => c.Distribution_Channel_Name.ToLower() == request.distributionChannelName.ToLower()
                    && c.Delete_Flag != true);

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
                        distributionChannelID = c.distributionChannelID,
                        distributionChannelName = c.distributionChannelName,
                        retailerGroupName = c.retailerGroupName,
                        retailerGroupID = c.retailerGroupID,
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
                request.departmentStoreName = request.departmentStoreName.Trim();

                var departmentStoreByName = repository.masterData.FindDepartmentStoreBy(
                    c => c.Department_Store_Name.ToLower() == request.departmentStoreName.ToLower()
                    && c.Delete_Flag != true);

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

                List<ImportResultByRowResponse> importResult = new List<ImportResultByRowResponse>();

                using (var reader = ExcelReaderFactory.CreateReader(request.fileStream))
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

                            if ((column1 != null && column1.ToLower() != "departmentstores") ||
                                (column2 != null && column2.ToLower() != "regions") ||
                                (column3 != null && column3.ToLower() != "departmentstores_group") ||
                                (column4 != null && column4.ToLower() != "rank") ||
                                (column5 != null && column5.ToLower() != "distribution_channels"))
                            {
                                response.isSuccess = false;
                                response.wrongFormatFile = true;
                                response.responseError = "Wrong format file!";
                                return response;
                            }
                        }

                        if (reader.Depth != 0)
                        {
                            string departmentStoreName = reader.GetValue(0)?.ToString();
                            string region = reader.GetValue(1)?.ToString();
                            string retailerGroupName = reader.GetValue(2)?.ToString();
                            string rankText = reader.GetValue(3)?.ToString();
                            string distributionChannelName = reader.GetValue(4)?.ToString();

                            if (!string.IsNullOrWhiteSpace(departmentStoreName) && !string.IsNullOrWhiteSpace(region)
                                && !string.IsNullOrWhiteSpace(retailerGroupName) && !string.IsNullOrWhiteSpace(distributionChannelName))
                            {
                                SaveDepsrtmentStoreRequest saveStore = new SaveDepsrtmentStoreRequest
                                {
                                    departmentStoreName = reader.GetValue(0)?.ToString(),
                                    region = reader.GetValue(1)?.ToString(),
                                    retailerGroupName = reader.GetValue(2)?.ToString(),
                                    distributionChannelName = reader.GetValue(4)?.ToString(),
                                    row = reader.Depth + 1
                                };

                                int rank = 0;
                                int.TryParse(reader.GetValue(3)?.ToString(), out rank);

                                if (rank != 0)
                                {
                                    saveStore.rank = rank;
                                }

                                saveDepartmentStoreList.Add(saveStore);
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(departmentStoreName) || !string.IsNullOrWhiteSpace(region)
                                    || !string.IsNullOrWhiteSpace(rankText) || !string.IsNullOrWhiteSpace(retailerGroupName) || !string.IsNullOrWhiteSpace(distributionChannelName))
                                {
                                    int row = reader.Depth + 1;
                                    ImportResultByRowResponse resultImport = new ImportResultByRowResponse
                                    {
                                        row = reader.Depth + 1,
                                        result = $"Row {row} required value in column "
                                    };

                                    if (string.IsNullOrWhiteSpace(departmentStoreName))
                                    {
                                        resultImport.result += "DepartmentStores, ";
                                    }

                                    if (string.IsNullOrWhiteSpace(region))
                                    {
                                        resultImport.result += "Regions, ";
                                    }

                                    if (string.IsNullOrWhiteSpace(retailerGroupName))
                                    {
                                        resultImport.result += "DepartmentStores_Group, ";
                                    }

                                    if (string.IsNullOrWhiteSpace(distributionChannelName))
                                    {
                                        resultImport.result += "Distribution_Channels, ";
                                    }

                                    importResult.Add(resultImport);
                                    response.countImportFailed = response.countImportFailed + 1;
                                }
                            }
                        }
                    }
                }


                if (saveDepartmentStoreList.Any())
                {
                    var groupByRetailerGroup = saveDepartmentStoreList.Where(e => !string.IsNullOrWhiteSpace(e.retailerGroupName))
                        .GroupBy(c => c.retailerGroupName);

                    var groupByDistrubution = saveDepartmentStoreList.Where(e => !string.IsNullOrWhiteSpace(e.distributionChannelName))
                       .GroupBy(c => c.distributionChannelName);

                    var groupByRegion = saveDepartmentStoreList.Where(e => !string.IsNullOrWhiteSpace(e.region))
                       .GroupBy(c => c.region);

                    var regionData = repository.masterData.GetRegionList();

                    var retailerGroupData = new Dictionary<string, Guid>();
                    var distributionChannelData = new Dictionary<string, Guid>();
                    var newRegionData = new Dictionary<string, Guid>();

                    foreach (var itemRetailerGroup in groupByRetailerGroup)
                    {
                        var retailerGroupByName = repository.masterData.FindRetailerGroupBy(
                            c => c.Retailer_Group_Name.ToLower() == itemRetailerGroup.Key.ToLower()
                            && c.Delete_Flag != true);

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
                        var channelByName = repository.masterData.FindDistributionChannelBy(
                            c => c.Distribution_Channel_Name.ToLower() == itemChannel.Key.ToLower()
                            && c.Delete_Flag != true);

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

                    foreach (var itemRegion in groupByRegion)
                    {
                        var regionByName = repository.masterData.FindRegionBy(
                            c => c.Region_Name.ToLower() == itemRegion.Key.ToLower());

                        if (regionByName == null)
                        {

                            var createRegionResult = await repository.masterData.CreateRegion(itemRegion.Key);

                            if (createRegionResult != null)
                            {
                                newRegionData.Add(itemRegion.Key, createRegionResult.Region_ID);
                            }
                        }
                        else
                        {
                            newRegionData.Add(itemRegion.Key, regionByName.Region_ID);
                        }
                    }

                    foreach (var saveDepartmentStoreRequest in saveDepartmentStoreList)
                    {
                        saveDepartmentStoreRequest.regionID = newRegionData.FirstOrDefault(c => c.Key == saveDepartmentStoreRequest.region).Value;
                        saveDepartmentStoreRequest.retailerGroupID = retailerGroupData.FirstOrDefault(c => c.Key == saveDepartmentStoreRequest.retailerGroupName).Value;
                        saveDepartmentStoreRequest.distributionChannelID = distributionChannelData.FirstOrDefault(c => c.Key == saveDepartmentStoreRequest.distributionChannelName).Value;
                        saveDepartmentStoreRequest.active = true;
                        saveDepartmentStoreRequest.userID = request.userID;

                        if (!string.IsNullOrWhiteSpace(saveDepartmentStoreRequest.departmentStoreName)
                            && saveDepartmentStoreRequest.retailerGroupID != Guid.Empty
                            && saveDepartmentStoreRequest.distributionChannelID != Guid.Empty
                            && saveDepartmentStoreRequest.regionID != Guid.Empty)
                        {
                            var result = await SaveDepartmentStore(saveDepartmentStoreRequest);
                            if (result.isSuccess)
                            {
                                response.countImportSuccess = response.countImportSuccess + 1;
                            }
                            else
                            {
                                ImportResultByRowResponse resultImport = new ImportResultByRowResponse
                                {
                                    row = saveDepartmentStoreRequest.row
                                };

                                if (result.isDuplicated == true)
                                {
                                    resultImport.result = $"Row {saveDepartmentStoreRequest.row} data is duplicate.";
                                }
                                else
                                {
                                    resultImport.result = $"Row {saveDepartmentStoreRequest.row} save data failed.";
                                }

                                importResult.Add(resultImport);
                                response.countImportFailed = response.countImportFailed + 1;
                            }
                        }
                        else
                        {
                            ImportResultByRowResponse resultImport = new ImportResultByRowResponse
                            {
                                row = saveDepartmentStoreRequest.row,
                                result = $"Row {saveDepartmentStoreRequest.row} required value in column "
                            };

                            if (string.IsNullOrWhiteSpace(saveDepartmentStoreRequest.departmentStoreName))
                            {
                                resultImport.result += "DepartmentStores, ";
                            }

                            if (saveDepartmentStoreRequest.regionID == Guid.Empty)
                            {
                                resultImport.result += "Regions, ";
                            }

                            if (saveDepartmentStoreRequest.retailerGroupID == Guid.Empty)
                            {
                                resultImport.result += "DepartmentStores_Group, ";
                            }

                            if (saveDepartmentStoreRequest.distributionChannelID == Guid.Empty)
                            {
                                resultImport.result += "Distribution_Channels, ";
                            }

                            importResult.Add(resultImport);
                            response.countImportFailed = response.countImportFailed + 1;
                        }
                    }

                    response.importResult = importResult.OrderBy(c => c.row).ToList();

                    if (response.countImportSuccess > 0)
                    {
                        response.isSuccess = true;
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.wrongFormatFile = true;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
            }

            return response;
        }

        #endregion

        #region Counter

        public GetCounterListResponse GetCounterList()
        {
            GetCounterListResponse response = new GetCounterListResponse();

            try
            {
                var dataList = repository.masterData.GetCounterList();

                if (dataList.Any())
                {
                    response.data = dataList;
                }
                else
                {
                    response.data = new List<CounterData>();
                }
            }
            catch (Exception ex)
            {
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public CounterData GetCounterDetail(Guid counterID)
        {
            CounterData response = new CounterData();

            try
            {
                var counterData = repository.masterData.FindCounterBy(c => c.Counter_ID == counterID);

                if (counterData != null)
                {
                    response.counterID = counterData.Counter_ID;
                    response.brandID = counterData.Brand_ID;
                    response.departmentStoreID = counterData.Department_Store_ID;
                    response.distributionChannelID = counterData.Distribution_Channel_ID;
                    response.active = counterData.Active_Flag;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return response;
        }

        public async Task<SaveDataResponse> SaveCounter(SaveCounterRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                var counterExist = repository.masterData.FindCounterBy(
                    c => c.Brand_ID == request.brandID
                    && c.Department_Store_ID == request.departmentStoreID
                    && c.Distribution_Channel_ID == request.distributionChannelID
                    && c.Delete_Flag != true);

                // Counter not exist Or Update old Counter
                if (counterExist == null || (counterExist != null && counterExist.Counter_ID == request.counterID))
                {
                    response.isSuccess = await repository.masterData.SaveCounter(request);
                }
                else
                {
                    response.isSuccess = false;
                    response.isDuplicated = true;
                    response.responseError = "Counter is exist";
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public async Task<SaveDataResponse> DeleteCounter(DeleteCounterRequest request)
        {
            SaveDataResponse response = new SaveDataResponse();

            try
            {
                response.isSuccess = await repository.masterData.DeleteCounter(request);
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }

        public async Task<ImportDataResponse> ImportCounterData(ImportDataRequest request)
        {
            ImportDataResponse response = new ImportDataResponse();

            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                List<SaveCounterRequest> saveCounterList = new List<SaveCounterRequest>();

                using (var reader = ExcelReaderFactory.CreateReader(request.fileStream))
                {
                    while (reader.Read()) //Each row of the file
                    {
                        // Validate Column File
                        if (reader.Depth == 0)
                        {
                            string column1 = reader.GetValue(0)?.ToString();
                            string column2 = reader.GetValue(1)?.ToString();
                            string column3 = reader.GetValue(2)?.ToString();

                            if ((column3 != null && column3.ToLower() != "brand") ||
                                (column2 != null && column2.ToLower() != "department store") ||
                                (column1 != null && column1.ToLower() != "distribution channel"))
                            {
                                response.isSuccess = false;
                                response.wrongFormatFile = true;
                                return response;
                            }
                        }

                        if (reader.Depth != 0)
                        {
                            saveCounterList.Add(new SaveCounterRequest
                            {
                                distributionChannelName = reader.GetValue(0)?.ToString(),
                                departmentStoreName = reader.GetValue(1)?.ToString(),
                                brandName = reader.GetValue(2)?.ToString(),
                            });
                        }
                    }
                }

                if (saveCounterList.Any())
                {
                    var groupByBrandName = saveCounterList.Where(e => !string.IsNullOrWhiteSpace(e.brandName))
                     .GroupBy(c => c.brandName);

                    var groupByDepartmentStoreName = saveCounterList.Where(e => !string.IsNullOrWhiteSpace(e.departmentStoreName))
                       .GroupBy(c => c.departmentStoreName);

                    var groupByChannel = saveCounterList.Where(e => !string.IsNullOrWhiteSpace(e.distributionChannelName))
                       .GroupBy(c => c.distributionChannelName);

                    var brandNameData = new Dictionary<string, Guid>();
                    var departmentStoreData = new Dictionary<string, Guid>();
                    var channelData = new Dictionary<string, Guid>();

                    foreach (var itemBrandName in groupByBrandName)
                    {
                        var brandByName = repository.masterData.FindBrandBy(
                            c => c.Brand_Name.ToLower() == itemBrandName.Key.ToLower()
                            && c.Delete_Flag != true);

                        if (brandByName != null)
                        {
                            brandNameData.Add(itemBrandName.Key, brandByName.Brand_ID);
                        }
                    }

                    foreach (var itemDepartment in groupByDepartmentStoreName)
                    {
                        var departmentStoreByName = repository.masterData.FindDepartmentStoreBy(
                            c => c.Department_Store_Name.ToLower() == itemDepartment.Key.ToLower()
                            && c.Delete_Flag != true);

                        if (departmentStoreByName != null)
                        {
                            departmentStoreData.Add(itemDepartment.Key, departmentStoreByName.Department_Store_ID);
                        }
                    }

                    foreach (var itemChannel in groupByChannel)
                    {
                        var channelByName = repository.masterData.FindDistributionChannelBy(
                            c => c.Distribution_Channel_Name.ToLower() == itemChannel.Key.ToLower()
                            && c.Delete_Flag != true);

                        if (channelByName != null)
                        {
                            channelData.Add(itemChannel.Key, channelByName.Distribution_Channel_ID);
                        }
                    }

                    foreach (var saveCounterRequest in saveCounterList)
                    {
                        var brandData = brandNameData.FirstOrDefault(c => c.Key.ToLower() == saveCounterRequest.brandName.ToLower());
                        var departmentStore = departmentStoreData.FirstOrDefault(c => c.Key.ToLower() == saveCounterRequest.departmentStoreName.ToLower());
                        var channel = channelData.FirstOrDefault(c => c.Key.ToLower() == saveCounterRequest.distributionChannelName.ToLower());

                        if (brandData.Key != null && departmentStore.Key != null && channel.Key != null)
                        {
                            saveCounterRequest.brandID = brandData.Value;
                            saveCounterRequest.departmentStoreID = departmentStore.Value;
                            saveCounterRequest.distributionChannelID = channel.Value;
                            saveCounterRequest.active = true;
                            saveCounterRequest.userID = request.userID;

                            if (saveCounterRequest.brandID != Guid.Empty
                                && saveCounterRequest.departmentStoreID != Guid.Empty
                                && saveCounterRequest.distributionChannelID != Guid.Empty)
                            {
                                var result = await SaveCounter(saveCounterRequest);
                                if (result.isSuccess)
                                {
                                    response.countImportSuccess = response.countImportSuccess + 1;
                                }
                                else
                                {
                                    response.countImportFailed = response.countImportFailed + 1;
                                }
                            }
                        }
                    }

                    if (response.countImportSuccess > 0)
                    {
                        response.isSuccess = true;
                    }

                }
                else
                {
                    response.isSuccess = false;
                    response.wrongFormatFile = true;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
            }

            return response;
        }

        #endregion

        public GetRegionListResponse GetRegion()
        {
            GetRegionListResponse response = new GetRegionListResponse();

            try
            {
                var searchData = repository.masterData.GetRegionList();

                if (searchData != null && searchData.Any())
                {
                    response.data = searchData.Select(c => new RegionData
                    {
                        regionID = c.Region_ID,
                        regionName = c.Region_Name
                    }).ToList();
                }
                else
                {
                    response.data = new List<RegionData>();
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
