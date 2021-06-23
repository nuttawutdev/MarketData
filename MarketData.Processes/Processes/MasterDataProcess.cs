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
                        brandTypeName = c.Brand_Type_Name,
                        active = c.Active_Flag
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
                return null;
            }

            return response;
        }
    }
}
