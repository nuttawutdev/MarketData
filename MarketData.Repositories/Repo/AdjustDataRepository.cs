﻿using MarketData.Helper;
using MarketData.Model.Entiry;
using MarketData.Model.Request.Adjust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MarketData.Repositories.Repo
{
    public class AdjustDataRepository
    {
        private readonly MarketDataDBContext _dbContext;

        public AdjustDataRepository(MarketDataDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<TTAdjustData> GetAdjustDataBy(Expression<Func<TTAdjustData, bool>> expression)
        {
            try
            {
                return _dbContext.TTAdjustData.Where(expression).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TTAdjustData FindAdjustDataBy(Expression<Func<TTAdjustData, bool>> expression)
        {
            try
            {
                return _dbContext.TTAdjustData.Where(expression).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TTAdjustDataDetail FindAdjustDataDetailBy(Expression<Func<TTAdjustDataDetail, bool>> expression)
        {
            try
            {
                return _dbContext.TTAdjustDataDetail.Where(expression).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TTAdjustData> CreateAdjustData(GetAdjustDetailRequest request)
        {
            try
            {
                var statusPending = _dbContext.TMAdjustStatus.FirstOrDefault(e => e.Status_Name == "Pending");

                TTAdjustData newAdjustData = new TTAdjustData
                {
                    ID = Guid.NewGuid(),
                    DistributionChannel_ID = request.distributionChannelID,
                    RetailerGroup_ID = request.retailerGroupID,
                    DepartmentStore_ID = request.departmentStoreID,
                    Week = request.week,
                    Month = request.month,
                    Year = request.year,
                    Universe = request.universe,
                    Status_ID = statusPending.ID,
                    Create_By = request.userID,
                    Create_Date = Utility.GetDateNowThai()
                };

                _dbContext.TTAdjustData.Add(newAdjustData);

                if(await _dbContext.SaveChangesAsync() > 0)
                {
                    return newAdjustData;
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

        public List<TTAdjustDataDetail> GetAdjustDataDetaillBy(Expression<Func<TTAdjustDataDetail, bool>> expression)
        {
            try
            {
                return _dbContext.TTAdjustDataDetail.Where(expression).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
        public async Task<bool> UpdateAdjustData(Guid adjustDataID,Guid userID,Guid statusID)
        {
            try
            {
                var adjustData = _dbContext.TTAdjustData.Find(adjustDataID);
                adjustData.Status_ID = statusID;
                adjustData.Update_By = userID;
                adjustData.Update_Date = Utility.GetDateNowThai();

                _dbContext.TTAdjustData.Update(adjustData);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveAllAdjustDetailByID(Guid adjustDataID)
        {
            try
            {
                var adjustDataDetail = _dbContext.TTAdjustDataDetail.Where(c=>c.AdjustData_ID == adjustDataID);   
                _dbContext.TTAdjustDataDetail.RemoveRange(adjustDataDetail);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveAdjustDetai(List<TTAdjustDataDetail> adjustDataDetail)
        {
            try
            {
                _dbContext.TTAdjustDataDetail.RemoveRange(adjustDataDetail);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveAllAdjustBrandDetailByID(Guid adjustDataID)
        {
            try
            {
                var adjustDataBrandDetail = _dbContext.TTAdjustDataBrandDetail.Where(c => c.AdjustData_ID == adjustDataID);
                _dbContext.TTAdjustDataBrandDetail.RemoveRange(adjustDataBrandDetail);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> InsertAdjustDataDetail(List<TTAdjustDataDetail> adjustDataDetailList)
        {
            try
            {
                _dbContext.TTAdjustDataDetail.AddRange(adjustDataDetailList);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> InsertAdjustDataBrandDetail(List<TTAdjustDataBrandDetail> adjustDataBrandDetailList)
        {
            try
            {
                _dbContext.TTAdjustDataBrandDetail.AddRange(adjustDataBrandDetailList);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
