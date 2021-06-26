using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Request;
using MarketData.Model.Request.MasterData;
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

        #region Brand Type

        public (List<TMBrandType>, int) GetBrandTypeList(GetBrandTypeListRequest request)
        {
            try
            {
                var searchData = _dbContext.TMBrandType.Where(
                    c => c.Delete_Flag != true && (string.IsNullOrWhiteSpace(request.textSearch) || c.Brand_Type_Name.ToLower().Contains(request.textSearch.ToLower())))
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

                return _dbContext.SaveChanges() > 0;
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

        public bool DeleteBrandType(DeleteBrandTypeRequest request)
        {
            try
            {
                var brandTypeUpdate = _dbContext.TMBrandType.Find(request.brandTypeID);

                if (brandTypeUpdate != null)
                {
                    brandTypeUpdate.Active_Flag = false;
                    brandTypeUpdate.Delete_Flag = true;
                    brandTypeUpdate.Updated_By = request.userID;
                    brandTypeUpdate.Updated_Date = DateTime.Now;

                    _dbContext.TMBrandType.Update(brandTypeUpdate);
                }

                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Brand Segment

        public (List<TMBrandSegment>, int) GetBrandSegmentList(GetBrandSegmentListRequest request)
        {
            try
            {
                var searchData = _dbContext.TMBrandSegment.Where(
                    c => c.Delete_Flag != true && (string.IsNullOrWhiteSpace(request.textSearch) || c.Brand_Segment_Name.ToLower().Contains(request.textSearch.ToLower())))
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
                        case "brandsegmentname":
                            {
                                searchData = request.sortBy == "desc"
                                    ? searchData.OrderByDescending(x => x.Brand_Segment_Name)
                                    : searchData.OrderBy(x => x.Brand_Segment_Name);
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

        public bool SaveBrandSegment(SaveBrandSegmentRequest request)
        {
            try
            {
                if (request.brandSegmentID == null)
                {
                    TMBrandSegment insertBrandSegment = new TMBrandSegment
                    {
                        Brand_Segment_ID = Guid.NewGuid(),
                        Brand_Segment_Name = request.brandSegmentName,
                        Active_Flag = request.active,
                        Created_By = request.userID,
                        Created_Date = DateTime.Now
                    };

                    _dbContext.TMBrandSegment.Add(insertBrandSegment);
                }
                else
                {
                    var brandSegmentUpdate = _dbContext.TMBrandSegment.Find(request.brandSegmentID);

                    if (brandSegmentUpdate != null)
                    {
                        brandSegmentUpdate.Brand_Segment_Name = request.brandSegmentName;
                        brandSegmentUpdate.Active_Flag = request.active;
                        brandSegmentUpdate.Updated_By = request.userID;
                        brandSegmentUpdate.Updated_Date = DateTime.Now;

                        _dbContext.TMBrandSegment.Update(brandSegmentUpdate);
                    }
                    else
                    {
                        return false;
                    }
                }

                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TMBrandSegment FindBrandSegmentBy(Expression<Func<TMBrandSegment, bool>> expression)
        {
            try
            {
                return _dbContext.TMBrandSegment.Where(expression).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteBrandSegment(DeleteBrandSegmentRequest request)
        {
            try
            {
                var brandSegmentUpdate = _dbContext.TMBrandSegment.Find(request.brandSegmentID);

                if (brandSegmentUpdate != null)
                {
                    brandSegmentUpdate.Active_Flag = false;
                    brandSegmentUpdate.Delete_Flag = true;
                    brandSegmentUpdate.Updated_By = request.userID;
                    brandSegmentUpdate.Updated_Date = DateTime.Now;

                    _dbContext.TMBrandSegment.Update(brandSegmentUpdate);
                }

                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Brand Group

        public (List<TMBrandGroup>, int) GetBrandGroupList(GetBrandGroupListRequest request)
        {
            try
            {
                var searchData = _dbContext.TMBrandGroup.Where(
                    c => c.Delete_Flag != true && (string.IsNullOrWhiteSpace(request.textSearch) || c.Brand_Group_Name.ToLower().Contains(request.textSearch.ToLower())))
                    .AsNoTracking();

                if (request.active != "All")
                {
                    bool activeFilter = request.active == "Y";
                    searchData = searchData.Where(e => e.Active_Flag == activeFilter);
                }

                if (request.isLoreal != "All")
                {
                    bool isLorealFilter = request.isLoreal == "Y";
                    searchData = searchData.Where(e => e.Is_Loreal_Brand == isLorealFilter);
                }

                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortBy))
                {
                    switch (request.sortColumn.ToLower())
                    {
                        case "brandgroupname":
                            {
                                searchData = request.sortBy == "desc"
                                    ? searchData.OrderByDescending(x => x.Brand_Group_Name)
                                    : searchData.OrderBy(x => x.Brand_Group_Name);
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

        public bool SaveBrandGroup(SaveBrandGroupRequest request)
        {
            try
            {
                if (request.brandGroupID == null)
                {
                    TMBrandGroup insertBrandGroup = new TMBrandGroup
                    {
                        Brand_Group_ID = Guid.NewGuid(),
                        Brand_Group_Name = request.brandGroupName,
                        Is_Loreal_Brand = request.isLoreal,
                        Active_Flag = request.active,
                        Created_By = request.userID,
                        Created_Date = DateTime.Now
                    };

                    _dbContext.TMBrandGroup.Add(insertBrandGroup);
                }
                else
                {
                    var brandGroupUpdate = _dbContext.TMBrandGroup.Find(request.brandGroupID);

                    if (brandGroupUpdate != null)
                    {
                        brandGroupUpdate.Brand_Group_Name = request.brandGroupName;
                        brandGroupUpdate.Is_Loreal_Brand = request.isLoreal;
                        brandGroupUpdate.Active_Flag = request.active;
                        brandGroupUpdate.Updated_By = request.userID;
                        brandGroupUpdate.Updated_Date = DateTime.Now;

                        _dbContext.TMBrandGroup.Update(brandGroupUpdate);
                    }
                    else
                    {
                        return false;
                    }
                }

                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TMBrandGroup FindBrandGroupBy(Expression<Func<TMBrandGroup, bool>> expression)
        {
            try
            {
                return _dbContext.TMBrandGroup.Where(expression).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteBrandGroup(DeleteBrandGroupRequest request)
        {
            try
            {
                var brandGroupUpdate = _dbContext.TMBrandGroup.Find(request.brandGroupID);

                if (brandGroupUpdate != null)
                {
                    brandGroupUpdate.Active_Flag = false;
                    brandGroupUpdate.Delete_Flag = true;
                    brandGroupUpdate.Updated_By = request.userID;
                    brandGroupUpdate.Updated_Date = DateTime.Now;

                    _dbContext.TMBrandGroup.Update(brandGroupUpdate);
                }

                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public (List<BrandData>, int) GetBrandList(GetBrandListRequest request)
        {
            try
            {
                var searchData = _dbContext.TMBrand.Where(c => c.Delete_Flag != true).AsNoTracking();

                if (request.active != "All")
                {
                    bool activeFilter = request.active == "Y";
                    searchData = searchData.Where(e => e.Active_Flag == activeFilter);
                }

                if (!string.IsNullOrWhiteSpace(request.textSearch))
                {
                    switch (request.searchBy)
                    {
                        case "Brand Name":
                            {
                                searchData = searchData.Where(c => c.Brand_Name.ToLower().Contains(request.textSearch));
                                break;
                            }
                        case "Brand Group":
                            {
                                var brandGroupIDFilter = _dbContext.TMBrandGroup.Where(g => g.Brand_Group_Name.ToLower().Contains(request.textSearch.ToLower())).Select(c => c.Brand_Group_ID);
                                searchData = searchData.Where(c => brandGroupIDFilter.Contains(c.Brand_Group_ID));
                                break;
                            }
                        case "Brand Segment":
                            {
                                var brandSegmentIDFilter = _dbContext.TMBrandSegment.Where(g => g.Brand_Segment_Name.ToLower().Contains(request.textSearch.ToLower())).Select(c => c.Brand_Segment_ID);
                                searchData = searchData.Where(c => brandSegmentIDFilter.Contains(c.Brand_Segment_ID));
                                break;
                            }
                        case "Brand Type":
                            {
                                var brandTypeIDFilter = _dbContext.TMBrandType.Where(g => g.Brand_Type_Name.ToLower().Contains(request.textSearch.ToLower())).Select(c => c.Brand_Type_ID);
                                searchData = searchData.Where(c => brandTypeIDFilter.Contains(c.Brand_Type_ID));
                                break;
                            }
                    }
                }

                var brandList = (
                   from e in searchData
                   join g in _dbContext.TMBrandGroup
                       on e.Brand_Group_ID equals g.Brand_Group_ID
                       into joinGroup
                   from groups in joinGroup.DefaultIfEmpty()
                   join s in _dbContext.TMBrandSegment
                       on e.Brand_Segment_ID equals s.Brand_Segment_ID
                       into joinSegment
                   from segment in joinSegment.DefaultIfEmpty()
                   join t in _dbContext.TMBrandType
                       on e.Brand_Type_ID equals t.Brand_Type_ID
                       into joinType
                   from type in joinType.DefaultIfEmpty()
                   select new BrandData
                   {
                       brandID = e.Brand_ID,
                       brandName = e.Brand_Name,
                       brandGroupName = groups != null ? groups.Brand_Group_Name : "",
                       brandSegmentName = segment != null ? segment.Brand_Segment_Name : "",
                       brandTypeName = type != null ? type.Brand_Type_Name : "",
                       universe = e.Universe,
                       brandBirth = e.Created_Date.HasValue ? e.Created_Date.GetValueOrDefault().ToString("dd/MM/yyyy") : "",
                       active = e.Active_Flag,
                       color = e.Brand_Color,
                       isLorealBrand = groups != null ? groups.Is_Loreal_Brand : false
                   });


                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortBy))
                {
                    switch (request.sortColumn)
                    {
                        case "Brand Name":
                            {
                                brandList = request.sortBy == "desc"
                                    ? brandList.OrderByDescending(x => x.brandName)
                                    : brandList.OrderBy(x => x.brandName);
                                break;
                            }
                        case "Brand Group":
                            {
                                brandList = request.sortBy == "desc"
                                    ? brandList.OrderByDescending(x => x.brandGroupName)
                                    : brandList.OrderBy(x => x.brandGroupName);
                                break;
                            }
                        case "Brand Segment":
                            {
                                brandList = request.sortBy == "desc"
                                    ? brandList.OrderByDescending(x => x.brandSegmentName)
                                    : brandList.OrderBy(x => x.brandSegmentName);
                                break;
                            }
                        case "Brand Type":
                            {
                                brandList = request.sortBy == "desc"
                                    ? brandList.OrderByDescending(x => x.brandTypeName)
                                    : brandList.OrderBy(x => x.brandTypeName);
                                break;
                            }
                    }
                }
                else
                {
                    brandList = brandList.OrderBy(x => x.brandName);
                }

                var brandPaging = brandList
                  .Skip((request.pageNo - 1) * request.pageSize)
                  .Take(request.pageSize).ToList();

                return (brandPaging, brandList.Count());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
