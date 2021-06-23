using MarketData.Model.Entiry;
using MarketData.Model.Request;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketData.Repositories.Repo
{
    public class MasterDataRepository
    {
        private readonly MarketDataDBContext _dbContext;
        public MasterDataRepository(MarketDataDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public (List<TMBrandType>, int) GetBrandTypeList(GetBrandTypeListRequest request)
        {
            try
            {
                var searchData = _dbContext.TMBrandType.Where(c =>
               (string.IsNullOrWhiteSpace(request.textSearch) || c.Brand_Type_Name.ToLower().Contains(request.textSearch.ToLower())))
                    .AsNoTracking();

                if (request.active != "All")
                {
                    bool activeFilter = request.active == "Y";
                    searchData = searchData.Where(e => e.Active_Flag == activeFilter);
                }

                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortBy))
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "brandtypename":
                            {
                                searchData = request.sortBy == "desc"
                                    ? searchData.OrderByDescending(x => x.Brand_Type_Name)
                                    : searchData.OrderBy(x => x.Brand_Type_Name);
                                break;
                            }
                    }
                }
                else
                {
                    searchData = searchData.OrderByDescending(x => x.Created_Date);
                }

                var searchDataList = searchData
                  .Skip((request.pageNo - 1) * request.pageSize)
                  .Take(request.pageSize).ToList();

                return (searchDataList, searchData.Count());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
