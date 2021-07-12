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

        public List<TMBrandType> GetBrandTypeList()
        {
            try
            {
                var searchData = _dbContext.TMBrandType.Where(c => c.Delete_Flag != true).AsNoTracking();
                searchData = searchData.OrderBy(x => x.Brand_Type_Name);
                return searchData.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SaveBrandType(SaveBrandTypeRequest request)
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
                        Delete_Flag = false,
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

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TMBrandType CreateBrandType(SaveBrandTypeRequest request)
        {
            try
            {
                TMBrandType insertBrandType = new TMBrandType
                {
                    Brand_Type_ID = Guid.NewGuid(),
                    Brand_Type_Name = request.brandTypeName,
                    Active_Flag = request.active,
                    Delete_Flag = false,
                    Created_By = request.userID,
                    Created_Date = DateTime.Now
                };

                _dbContext.TMBrandType.Add(insertBrandType);

                if (_dbContext.SaveChanges() > 0)
                {
                    return insertBrandType;
                }
                else
                {
                    return null;
                }
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

        public List<TMBrandSegment> GetBrandSegmentList()
        {
            try
            {
                var searchData = _dbContext.TMBrandSegment.Where(c => c.Delete_Flag != true).AsNoTracking();
                searchData = searchData.OrderBy(c => c.Brand_Segment_Name);
                return searchData.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async  Task<bool> SaveBrandSegment(SaveBrandSegmentRequest request)
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

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TMBrandSegment CreateBrandSegment(SaveBrandSegmentRequest request)
        {
            try
            {
                TMBrandSegment insertBrandSegment = new TMBrandSegment
                {
                    Brand_Segment_ID = Guid.NewGuid(),
                    Brand_Segment_Name = request.brandSegmentName,
                    Active_Flag = request.active,
                    Delete_Flag = false,
                    Created_By = request.userID,
                    Created_Date = DateTime.Now
                };

                _dbContext.TMBrandSegment.Add(insertBrandSegment);

                if (_dbContext.SaveChanges() > 0)
                {
                    return insertBrandSegment;
                }
                else
                {
                    return null;
                }
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

        public List<TMBrandGroup> GetBrandGroupList()
        {
            try
            {
                var searchData = _dbContext.TMBrandGroup.Where(c => c.Delete_Flag != true).AsNoTracking();
                searchData = searchData.OrderBy(c => c.Brand_Group_Name);
                return searchData.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SaveBrandGroup(SaveBrandGroupRequest request)
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

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TMBrandGroup CreateBrandGroup(SaveBrandGroupRequest request)
        {
            try
            {
                TMBrandGroup insertBrandGroup = new TMBrandGroup
                {
                    Brand_Group_ID = Guid.NewGuid(),
                    Brand_Group_Name = request.brandGroupName,
                    Is_Loreal_Brand = request.isLoreal,
                    Active_Flag = request.active,
                    Delete_Flag = false,
                    Created_By = request.userID,
                    Created_Date = DateTime.Now
                };

                _dbContext.TMBrandGroup.Add(insertBrandGroup);

                if (_dbContext.SaveChanges() > 0)
                {
                    return insertBrandGroup;
                }
                else
                {
                    return null;
                }
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

        #region Brand
        public List<BrandData> GetBrandList()
        {
            try
            {
                var searchData = _dbContext.TMBrand.Where(c => c.Delete_Flag != true).AsNoTracking();

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

                brandList = brandList.OrderBy(x => x.brandName);
                return brandList.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TMBrand FindBrandBy(Expression<Func<TMBrand, bool>> expression)
        {
            try
            {
                return _dbContext.TMBrand.Where(expression).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SaveBrand(SaveBrandRequest request)
        {
            try
            {
                if (request.brandID == null)
                {
                    TMBrand insertBrand = new TMBrand
                    {
                        Brand_ID = Guid.NewGuid(),
                        Brand_Name = request.brandName,
                        Brand_Short_Name = request.brandShortName,
                        Brand_Color = request.brandColor,
                        Brand_Segment_ID = request.brandSegmentID,
                        Brand_Type_ID = request.brandTypeID,
                        Delete_Flag = false,
                        Loreal_Brand_Rank = request.lorealBrandRank,
                        Universe = request.universe,
                        Brand_Group_ID = request.brandGroupID,
                        Active_Flag = request.active,
                        Created_By = request.userID,
                        Created_Date = DateTime.Now
                    };

                    _dbContext.TMBrand.Add(insertBrand);
                }
                else
                {
                    var brandUpdate = _dbContext.TMBrand.Find(request.brandID);

                    if (brandUpdate != null)
                    {
                        brandUpdate.Brand_Name = request.brandName;
                        brandUpdate.Brand_Short_Name = request.brandShortName;
                        brandUpdate.Brand_Color = request.brandColor;
                        brandUpdate.Brand_Segment_ID = request.brandSegmentID;
                        brandUpdate.Brand_Group_ID = request.brandGroupID;
                        brandUpdate.Brand_Type_ID = request.brandTypeID;
                        brandUpdate.Loreal_Brand_Rank = request.lorealBrandRank;
                        brandUpdate.Universe = request.universe;
                        brandUpdate.Active_Flag = request.active;
                        brandUpdate.Updated_By = request.userID;
                        brandUpdate.Updated_Date = DateTime.Now;

                        _dbContext.TMBrand.Update(brandUpdate);
                    }
                    else
                    {
                        return false;
                    }
                }

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteBrand(DeleteBrandRequest request)
        {
            try
            {
                var brandUpdate = _dbContext.TMBrand.Find(request.brandID);

                if (brandUpdate != null)
                {
                    brandUpdate.Active_Flag = false;
                    brandUpdate.Delete_Flag = true;
                    brandUpdate.Updated_By = request.userID;
                    brandUpdate.Updated_Date = DateTime.Now;

                    _dbContext.TMBrand.Update(brandUpdate);
                }

                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Retailer Group

        public List<TMRetailerGroup> GetRetailerGroupList()
        {
            try
            {
                var searchData = _dbContext.TMRetailerGroup.Where(c => c.Delete_Flag != true).AsNoTracking();
                searchData = searchData.OrderByDescending(x => x.Retailer_Group_Name);
                return searchData.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SaveRetailerGroup(SaveRetailerGroupRequest request)
        {
            try
            {
                if (request.retailerGroupID == null)
                {
                    TMRetailerGroup insertRetailerGroup = new TMRetailerGroup
                    {
                        Retailer_Group_ID = Guid.NewGuid(),
                        Retailer_Group_Name = request.retailerGroupName,
                        Active_Flag = request.active,
                        Delete_Flag = false,
                        Created_By = request.userID,
                        Created_Date = DateTime.Now
                    };

                    _dbContext.TMRetailerGroup.Add(insertRetailerGroup);
                }
                else
                {
                    var retailerGroupUpdate = _dbContext.TMRetailerGroup.Find(request.retailerGroupID);

                    if (retailerGroupUpdate != null)
                    {
                        retailerGroupUpdate.Retailer_Group_Name = request.retailerGroupName;
                        retailerGroupUpdate.Active_Flag = request.active;
                        retailerGroupUpdate.Updated_By = request.userID;
                        retailerGroupUpdate.Updated_Date = DateTime.Now;

                        _dbContext.TMRetailerGroup.Update(retailerGroupUpdate);
                    }
                    else
                    {
                        return false;
                    }
                }

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TMRetailerGroup> CreateRetailerGroup(SaveRetailerGroupRequest request)
        {
            try
            {
                TMRetailerGroup insertRetailerGroup = new TMRetailerGroup
                {
                    Retailer_Group_ID = Guid.NewGuid(),
                    Retailer_Group_Name = request.retailerGroupName,
                    Active_Flag = request.active,
                    Delete_Flag = false,
                    Created_By = request.userID,
                    Created_Date = DateTime.Now
                };

                _dbContext.TMRetailerGroup.Add(insertRetailerGroup);

                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return insertRetailerGroup;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TMRetailerGroup FindRetailerGroupBy(Expression<Func<TMRetailerGroup, bool>> expression)
        {
            try
            {
                return _dbContext.TMRetailerGroup.Where(expression).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteRetailerGroup(DeleteRetailerGroupRequest request)
        {
            try
            {
                var retailerGroupUpdate = _dbContext.TMRetailerGroup.Find(request.retailerGroupID);

                if (retailerGroupUpdate != null)
                {
                    retailerGroupUpdate.Active_Flag = false;
                    retailerGroupUpdate.Delete_Flag = true;
                    retailerGroupUpdate.Updated_By = request.userID;
                    retailerGroupUpdate.Updated_Date = DateTime.Now;

                    _dbContext.TMRetailerGroup.Update(retailerGroupUpdate);
                }

                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Distribution Channel

        public List<TMDistributionChannel> GetDistributionChannelList()
        {
            try
            {
                var searchData = _dbContext.TMDistributionChannel.Where(c => c.Delete_Flag != true).AsNoTracking();
                searchData = searchData.OrderByDescending(x => x.Distribution_Channel_Name);
                return searchData.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SaveDistributionChannel(SaveDistributionChannelRequest request)
        {
            try
            {
                if (request.distributionChannelID == null)
                {
                    TMDistributionChannel insertChannel = new TMDistributionChannel
                    {
                        Distribution_Channel_ID = Guid.NewGuid(),
                        Distribution_Channel_Name = request.distributionChannelName,
                        Active_Flag = request.active,
                        Delete_Flag = false,
                        Created_By = request.userID,
                        Created_Date = DateTime.Now
                    };

                    _dbContext.TMDistributionChannel.Add(insertChannel);
                }
                else
                {
                    var channelUpdate = _dbContext.TMDistributionChannel.Find(request.distributionChannelID);

                    if (channelUpdate != null)
                    {
                        channelUpdate.Distribution_Channel_Name = request.distributionChannelName;
                        channelUpdate.Active_Flag = request.active;
                        channelUpdate.Updated_By = request.userID;
                        channelUpdate.Updated_Date = DateTime.Now;

                        _dbContext.TMDistributionChannel.Update(channelUpdate);
                    }
                    else
                    {
                        return false;
                    }
                }

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TMDistributionChannel> CreateDistributionChannel(SaveDistributionChannelRequest request)
        {
            try
            {
                TMDistributionChannel insertChannel = new TMDistributionChannel
                {
                    Distribution_Channel_ID = Guid.NewGuid(),
                    Distribution_Channel_Name = request.distributionChannelName,
                    Active_Flag = request.active,
                    Delete_Flag = false,
                    Created_By = request.userID,
                    Created_Date = DateTime.Now
                };

                _dbContext.TMDistributionChannel.Add(insertChannel);

                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return insertChannel;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TMDistributionChannel FindDistributionChannelBy(Expression<Func<TMDistributionChannel, bool>> expression)
        {
            try
            {
                return _dbContext.TMDistributionChannel.Where(expression).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteDistributionChannel(DeleteDistributionChannelRequest request)
        {
            try
            {
                var channelUpdate = _dbContext.TMDistributionChannel.Find(request.distributionChannelID);

                if (channelUpdate != null)
                {
                    channelUpdate.Active_Flag = true;
                    channelUpdate.Delete_Flag = true;
                    channelUpdate.Updated_By = request.userID;
                    channelUpdate.Updated_Date = DateTime.Now;

                    _dbContext.TMDistributionChannel.Update(channelUpdate);
                }
                else
                {
                    return false;
                }

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Department Store

        public List<DepartmentStoreData> GetDepartmentStoreList()
        {
            try
            {
                var searchData = _dbContext.TMDepartmentStore.Where(c => c.Delete_Flag != true).AsNoTracking();

                var departmentStoreList = (
                   from e in searchData
                   join r in _dbContext.TMRetailerGroup
                       on e.Retailer_Group_ID equals r.Retailer_Group_ID
                       into joinRetailer
                   from retailer in joinRetailer.DefaultIfEmpty()
                   join d in _dbContext.TMDistributionChannel
                       on e.Distribution_Channel_ID equals d.Distribution_Channel_ID
                       into joinChannel
                   from channel in joinChannel.DefaultIfEmpty()
                   join re in _dbContext.TMRegion
                       on e.Region_ID equals re.Region_ID
                       into joinRegion
                   from region in joinRegion.DefaultIfEmpty()
                   select new DepartmentStoreData
                   {
                       departmentStoreID = e.Department_Store_ID,
                       departmentStoreName = e.Department_Store_Name,
                       distributionChannelID = channel != null ? channel.Distribution_Channel_ID : Guid.Empty,
                       distributionChannelName = channel != null ? channel.Distribution_Channel_Name : string.Empty,
                       retailerGroupID = retailer != null ? retailer.Retailer_Group_ID : Guid.Empty,
                       retailerGroupName = retailer != null ? retailer.Retailer_Group_Name : string.Empty,
                       region = region != null ? region.Region_Name : string.Empty,
                       active = e.Active_Flag,
                       rank = e.Rank
                   }).OrderBy(c => c.departmentStoreName).ToList();

                return departmentStoreList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TMDepartmentStore FindDepartmentStoreBy(Expression<Func<TMDepartmentStore, bool>> expression)
        {
            try
            {
                return _dbContext.TMDepartmentStore.Where(expression).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SaveDepartmentStore(SaveDepsrtmentStoreRequest request)
        {
            try
            {
                if (request.departmentStoreID == null)
                {
                    TMDepartmentStore insertDepartmentStore = new TMDepartmentStore
                    {
                        Department_Store_ID = Guid.NewGuid(),
                        Department_Store_Name = request.departmentStoreName,
                        Distribution_Channel_ID = request.distributionChannelID,
                        Retailer_Group_ID = request.retailerGroupID,
                        Region_ID = request.regionID.GetValueOrDefault(),
                        Rank = request.rank,
                        Delete_Flag = false,
                        Active_Flag = request.active,
                        Created_By = request.userID,
                        Created_Date = DateTime.Now
                    };

                    _dbContext.TMDepartmentStore.Add(insertDepartmentStore);
                }
                else
                {
                    var departmentStoreUpdate = _dbContext.TMDepartmentStore.Find(request.departmentStoreID);

                    if (departmentStoreUpdate != null)
                    {
                        departmentStoreUpdate.Department_Store_Name = request.departmentStoreName;
                        departmentStoreUpdate.Distribution_Channel_ID = request.distributionChannelID;
                        departmentStoreUpdate.Retailer_Group_ID = request.retailerGroupID;
                        departmentStoreUpdate.Region_ID = request.regionID.GetValueOrDefault();
                        departmentStoreUpdate.Rank = request.rank;
                        departmentStoreUpdate.Active_Flag = request.active;
                        departmentStoreUpdate.Updated_By = request.userID;
                        departmentStoreUpdate.Updated_Date = DateTime.Now;

                        _dbContext.TMDepartmentStore.Update(departmentStoreUpdate);
                    }
                    else
                    {
                        return false;
                    }
                }

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteDepartmentStore(DeleteDepartmentStoreRequest request)
        {
            try
            {
                var departmentStoreUpdate = _dbContext.TMDepartmentStore.Find(request.departmentStoreID);

                if (departmentStoreUpdate != null)
                {
                    departmentStoreUpdate.Active_Flag = false;
                    departmentStoreUpdate.Delete_Flag = true;
                    departmentStoreUpdate.Updated_By = request.userID;
                    departmentStoreUpdate.Updated_Date = DateTime.Now;

                    _dbContext.TMDepartmentStore.Update(departmentStoreUpdate);
                }

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Counter

        public List<CounterData> GetCounterList()
        {
            try
            {
                var searchData = _dbContext.TMCounter.Where(c => c.Delete_Flag != true).AsNoTracking();

                var counterList = (
                   from e in searchData
                   join d in _dbContext.TMDepartmentStore
                       on e.Department_Store_ID equals d.Department_Store_ID
                       into joinDepartment
                   from department in joinDepartment.DefaultIfEmpty()
                   join b in _dbContext.TMBrand
                       on e.Brand_ID equals b.Brand_ID
                       into joinBrand
                   from brand in joinBrand.DefaultIfEmpty()
                   join c in _dbContext.TMDistributionChannel
                       on e.Distribution_Channel_ID equals c.Distribution_Channel_ID
                       into joinChannel
                   from channel in joinChannel.DefaultIfEmpty()
                   join r in _dbContext.TMRetailerGroup
                       on department.Retailer_Group_ID equals r.Retailer_Group_ID
                       into joinRetailerGroup
                   from retailer in joinRetailerGroup.DefaultIfEmpty()
                   select new CounterData
                   {
                       counterID = e.Counter_ID,
                       departmentStoreID = department != null ? department.Department_Store_ID : Guid.Empty,
                       departmentStoreName = department != null ? department.Department_Store_Name : string.Empty,
                       brandID = brand != null ? brand.Brand_ID : Guid.Empty,
                       retailerGroupName = retailer.Retailer_Group_Name,
                       brandName = brand != null ? brand.Brand_Name : string.Empty,
                       distributionChannelID = channel != null ? channel.Distribution_Channel_ID : Guid.Empty,
                       distributionChannelName = channel != null ? channel.Distribution_Channel_Name : string.Empty,
                       active = e.Active_Flag
                   }).OrderBy(c => c.departmentStoreName).ToList();

                return counterList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TMCounter FindCounterBy(Expression<Func<TMCounter, bool>> expression)
        {
            try
            {
                return _dbContext.TMCounter.Where(expression).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SaveCounter(SaveCounterRequest request)
        {
            try
            {
                if (request.counterID == null)
                {
                    TMCounter insertCounter = new TMCounter
                    {
                        Counter_ID = Guid.NewGuid(),
                        Brand_ID = request.brandID,
                        Distribution_Channel_ID = request.distributionChannelID,
                        Department_Store_ID = request.departmentStoreID,
                        Delete_Flag = false,
                        Active_Flag = request.active,
                        Created_By = request.userID,
                        Created_Date = DateTime.Now
                    };

                    _dbContext.TMCounter.Add(insertCounter);
                }
                else
                {
                    var counterUpdate = _dbContext.TMCounter.Find(request.counterID);

                    if (counterUpdate != null)
                    {
                        counterUpdate.Brand_ID = request.brandID;
                        counterUpdate.Distribution_Channel_ID = request.distributionChannelID;
                        counterUpdate.Department_Store_ID = request.departmentStoreID;
                        counterUpdate.Active_Flag = request.active;
                        counterUpdate.Updated_By = request.userID;
                        counterUpdate.Updated_Date = DateTime.Now;

                        _dbContext.TMCounter.Update(counterUpdate);
                    }
                    else
                    {
                        return false;
                    }
                }

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteCounter(DeleteCounterRequest request)
        {
            try
            {
                var counterUpdate = _dbContext.TMCounter.Find(request.counterID);

                if (counterUpdate != null)
                {
                    counterUpdate.Active_Flag = false;
                    counterUpdate.Delete_Flag = true;
                    counterUpdate.Updated_By = request.userID;
                    counterUpdate.Updated_Date = DateTime.Now;

                    _dbContext.TMCounter.Update(counterUpdate);
                }

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public List<TMRegion> GetRegionList()
        {
            try
            {
                return _dbContext.TMRegion.AsNoTracking().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
