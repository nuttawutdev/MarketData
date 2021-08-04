using MarketData.Model.Entiry;
using MarketData.Model.Response.KeyIn;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MarketData.Repositories.Repo
{
    public class BAKeyInRepository
    {
        private readonly MarketDataDBContext _dbContext;
        public BAKeyInRepository(MarketDataDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<TMUserCounter> GetUserCounter(Guid userID)
        {
            try
            {
                var userCounterData = _dbContext.TMUserCounter.Where(c => c.User_ID == userID).AsNoTracking();
                return userCounterData.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BAKeyInData> GetBAKeyInByCounter(Guid departmentStoreID, Guid brandID, Guid channelID)
        {
            try
            {
                var baKeyInData = _dbContext.TTBAKeyIn.Where(
                    c => c.DepartmentStore_ID == departmentStoreID
                    && c.Brand_ID == brandID && c.DistributionChannel_ID == channelID).AsNoTracking();

                var baKeyInDataList = (
                  from e in baKeyInData
                  join ap in _dbContext.TMUser
                      on e.Approved_By equals ap.ID
                      into joinApprover
                  from approver in joinApprover.DefaultIfEmpty()
                  join b in _dbContext.TMBrand
                      on e.Brand_ID equals b.Brand_ID
                      into joinBrand
                  from brand in joinBrand.DefaultIfEmpty()
                  join d in _dbContext.TMDepartmentStore
                      on e.DepartmentStore_ID equals d.Department_Store_ID
                      into joinStore
                  from store in joinStore.DefaultIfEmpty()
                  join c in _dbContext.TMDistributionChannel
                     on e.DistributionChannel_ID equals c.Distribution_Channel_ID
                     into joinChannel
                  from channel in joinChannel.DefaultIfEmpty()
                  join s in _dbContext.TMKeyInStatus
                     on e.KeyIn_Status_ID equals s.ID
                     into joinStatus
                  from status in joinStatus.DefaultIfEmpty()
                  join c in _dbContext.TMUser
                    on e.Created_By equals c.ID
                    into joinCreateBy
                  from createBy in joinCreateBy.DefaultIfEmpty()
                  join u in _dbContext.TMUser
                   on e.Updated_By equals u.ID
                   into joinUpdateBy
                  from updateBy in joinUpdateBy.DefaultIfEmpty()
                  select new BAKeyInData
                  {
                      keyInID = e.ID,
                      year = e.Year,
                      month = e.Month,
                      week = e.Week,
                      counter = $"{brand.Brand_Name} {store.Department_Store_Name}",
                      retailerGroupID = e.RetailerGroup_ID,
                      departmentStoreID = e.DepartmentStore_ID,
                      brandID = e.Brand_ID,
                      distributionChannelID = e.DistributionChannel_ID,
                      distributionChannelName = channel.Distribution_Channel_Name,
                      statusID = e.KeyIn_Status_ID,
                      statusName = status.Status_Name,
                      lastEdit = updateBy != null ? updateBy.UserName : createBy != null ? createBy.UserName : string.Empty,
                      approver = approver != null ? approver.UserName : string.Empty,
                      approveDate = e.Approved_Date.HasValue ? e.Approved_Date.GetValueOrDefault().ToString("yyyy-MM-dd") : "",
                      submitDate = e.Submited_Date.HasValue ? e.Submited_Date.GetValueOrDefault().ToString("yyyy-MM-dd") : "",
                      remark = e.Remark
                  });

                return baKeyInDataList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TTBAKeyIn> GetBAKeyInBy(Expression<Func<TTBAKeyIn, bool>> expression)
        {
            try
            {
                return _dbContext.TTBAKeyIn.Where(expression).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
