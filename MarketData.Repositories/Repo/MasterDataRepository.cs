using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Request;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public bool SaveBrandType(SaveBrandTypeRequest request)
        {
            try
            {
                if (request.brandTypeID == null)
                {
                    TMBrandType insertBrandType = new TMBrandType
                    {
                        Brand_Type_ID = Guid.NewGuid(),
                        Brand_Type_Name = request.brandTypeName,
                        Active_Flag = request.active,
                        Created_By = request.userID,
                        Created_Date = DateTime.Now
                    };

                    _dbContext.TMBrandType.Add(insertBrandType);
                }
                else
                {
                    var brandTypeUpdate = _dbContext.TMBrandType.Find(request.brandTypeID);

                    if (brandTypeUpdate != null)
                    {
                        brandTypeUpdate.Brand_Type_Name = request.brandTypeName;
                        brandTypeUpdate.Active_Flag = request.active;
                        brandTypeUpdate.Updated_By = request.userID;
                        brandTypeUpdate.Updated_Date = DateTime.Now;

                        _dbContext.TMBrandType.Update(brandTypeUpdate);
                    }
                    else
                    {
                        return false;
                    }
                }

                return  _dbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TMBrandType FindBrandTypeBy(Expression<Func<TMBrandType, bool>> expression)
        {
            try
            {
                return _dbContext.TMBrandType.Where(expression).FirstOrDefault();     
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
