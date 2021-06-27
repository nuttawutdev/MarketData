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
                (List<TMBrandType> dataList, int totalRecord) = repository.masterData.GetBrandTypeList();

                if (dataList.Any())
                {
                    response.data = dataList.Select(c => new BrandTypeData
                    {
                        brandTypeID = c.Brand_Type_ID,
                        brandTypeName = c.Brand_Type_Name,
                        active = c.Active_Flag,
                        createdDate = c.Created_Date
                    }).ToList();
                    //response.totalRecord = totalRecord;
                    //response.totalPage = totalRecord != 0 ? Convert.ToInt32(Math.Ceiling((double)totalRecord / request.pageSize)) : 0;
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

        public GetBrandSegmentListResponse GetBrandSegmentList(GetBrandSegmentListRequest request)
        {
            GetBrandSegmentListResponse response = new GetBrandSegmentListResponse();

            try
            {
                (List<TMBrandSegment> dataList, int totalRecord) = repository.masterData.GetBrandSegmentList(request);

                if (dataList.Any())
                {
                    response.data = dataList.Select(c => new BrandSegmentData
                    {
                        brandSegmentID = c.Brand_Segment_ID,
                        brandSegmentName = c.Brand_Segment_Name,
                        active = c.Active_Flag,
                        createdDate = c.Created_Date
                    }).ToList();
                    response.totalRecord = totalRecord;
                    response.totalPage = totalRecord != 0 ? Convert.ToInt32(Math.Ceiling((double)totalRecord / request.pageSize)) : 0;
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

        public GetBrandGroupListResponse GetBrandGroupList(GetBrandGroupListRequest request)
        {
            GetBrandGroupListResponse response = new GetBrandGroupListResponse();

            try
            {
                (List<TMBrandGroup> dataList, int totalRecord) = repository.masterData.GetBrandGroupList(request);

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
                    response.totalRecord = totalRecord;
                    response.totalPage = totalRecord != 0 ? Convert.ToInt32(Math.Ceiling((double)totalRecord / request.pageSize)) : 0;
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

        public GetBrandListResponse GetBrandList(GetBrandListRequest request)
        {
            GetBrandListResponse response = new GetBrandListResponse();

            try
            {
                (List<BrandData> dataList, int totalRecord) = repository.masterData.GetBrandList(request);

                if (dataList.Any())
                {
                    response.data = dataList;
                    response.totalRecord = totalRecord;
                    response.totalPage = totalRecord != 0 ? Convert.ToInt32(Math.Ceiling((double)totalRecord / request.pageSize)) : 0;
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

        public SaveDataResponse SaveBrand(SaveBrandRequest request)
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
                            response.isSuccess = repository.masterData.SaveBrand(request);
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.isDuplicated = true;
                        }
                    }
                    else
                    {
                        response.isSuccess = repository.masterData.SaveBrand(request);
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

        public ImportBrandDataResponse ImportDataBrand(ImportBrandDataRequest request)
        {
            ImportBrandDataResponse response = new ImportBrandDataResponse();

            try
            {
                var fileName = "C:\\Users\\Nuttawut\\Downloads\\Brand.xlsx";
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                List<SaveBrandRequest> saveBrandList = new List<SaveBrandRequest>();

                using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
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
                                    column2 != "Short Name" ||
                                    column3 != "Brand Group" ||
                                    column4 != "Segment" ||
                                    column5 != "Type" ||
                                    column6 != "Brand Color")
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

                }

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.responseError = ex.Message ?? ex.InnerException?.Message;
            }

            return response;
        }
    }
}
