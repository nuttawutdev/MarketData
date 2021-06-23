using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Request;
using MarketData.Model.Response;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
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
       
        public GetBrandTypeListResponse GetBrandTypeList(GetBrandTypeListRequest request)
        {
            GetBrandTypeListResponse response = new GetBrandTypeListResponse();

            try
            {
                (List<TMBrandType> dataList, int totalRecord) = repository.masterData.GetBrandTypeList(request);

                if (dataList.Any())
                {
                    response.data = dataList.Select(c => new BrandTypeData
                    {
                        brandTypeID = c.Brand_Type_ID,
                        brandTypeName = c.Brand_Type_Name,
                        active = c.Active_Flag,
                        createdDate = c.Created_Date
                    }).ToList();
                    response.totalRecord = totalRecord;
                    response.totalPage = totalRecord != 0 ? Convert.ToInt32(Math.Ceiling((double)totalRecord / request.pageSize)) : 0;
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

        #endregion
    }
}
