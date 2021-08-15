using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Request.KeyIn;
using MarketData.Model.Response.KeyIn;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
                    && c.Brand_ID == brandID && c.DistributionChannel_ID == channelID).AsNoTracking().ToList();


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

        public TTBAKeyIn FindBAKeyInBy(Expression<Func<TTBAKeyIn, bool>> expression)
        {
            try
            {
                return _dbContext.TTBAKeyIn.Where(expression).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TTBAKeyIn CreateBAKeyIn(CreateBAKeyInRequest request)
        {
            try
            {
                var keyInStatusNew = _dbContext.TMKeyInStatus.FirstOrDefault(c => c.Status_Name == "New");

                TTBAKeyIn newBAKeyIn = new TTBAKeyIn
                {
                    ID = Guid.NewGuid(),
                    RetailerGroup_ID = request.retailerGroupID,
                    DepartmentStore_ID = request.departmentStoreID,
                    DistributionChannel_ID = request.distributionChannelID,
                    Brand_ID = request.brandID,
                    Year = request.year,
                    Month = request.month,
                    Week = request.week,
                    Universe = request.universe,
                    Created_By = request.userID,
                    Created_Date = DateTime.Now,
                    KeyIn_Status_ID = keyInStatusNew.ID,
                };

                _dbContext.TTBAKeyIn.Add(newBAKeyIn);

                if (_dbContext.SaveChanges() > 0)
                {
                    return newBAKeyIn;
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

        public async Task<bool> CreateBAKeyInDetail(List<TTBAKeyInDetail> listBAKeyInDetail)
        {
            try
            {
                _dbContext.TTBAKeyInDetail.AddRange(listBAKeyInDetail);

                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BAKeyInDetailData> GetBAKeyInDetailBy(Expression<Func<TTBAKeyInDetail, bool>> expression)
        {
            try
            {
                var BAKeyInDetail = _dbContext.TTBAKeyInDetail.Where(expression).ToList();

                var baKeyInDetailDataList = (
                     from e in BAKeyInDetail
                     join b in _dbContext.TMBrand
                         on e.Brand_ID equals b.Brand_ID
                         into joinBrand
                     from brand in joinBrand.DefaultIfEmpty()
                     select new BAKeyInDetailData
                     {
                         ID = e.ID,
                         brandID = e.Brand_ID,
                         departmentStoreID = e.DepartmentStore_ID,
                         amountSale = e.Amount_Sales,
                         keyInID = e.BAKeyIn_ID,
                         fg = e.FG,
                         sk = e.SK,
                         mu = e.MU,
                         ot = e.OT,
                         brandName = brand.Brand_Name,
                         brandColor = brand.Brand_Color,
                         channelID = e.DistributionChannel_ID,
                         counterID = e.Counter_ID,
                         month = e.Month,
                         rank = e.Rank,
                         remark = e.Remark,
                         week = e.Week,
                         wholeSale = e.Whole_Sales,
                         year = e.Year,
                     }).ToList();

                return baKeyInDetailDataList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TTBAKeyInDetail> GetBAKeyInDetailListData(Expression<Func<TTBAKeyInDetail, bool>> expression)
        {
            try
            {
                return _dbContext.TTBAKeyInDetail.Where(expression).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateBAKeyIn(SaveBAKeyInDetailRequest request, Guid status, bool isSubmit = false)
        {
            try
            {
                var baKeyInData = _dbContext.TTBAKeyIn.Find(request.BAKeyInID);
                baKeyInData.Updated_By = request.userID;
                baKeyInData.Updated_Date = DateTime.Now;
                baKeyInData.KeyIn_Status_ID = status;

                int monthKeyIn = Int32.Parse(baKeyInData.Month);
                int YearKeyIn = Int32.Parse(baKeyInData.Year);

                if (isSubmit)
                {
                    if (baKeyInData.Week == "1")
                    {
                        DateTime dateDeadLine = new DateTime(YearKeyIn, monthKeyIn, 10, 0, 0, 0);

                        if (DateTime.Now > dateDeadLine)
                        {
                            baKeyInData.Remark = "ล่าช้า";
                        }
                    }
                    else if (baKeyInData.Week == "2")
                    {
                        DateTime dateDeadLine = new DateTime(YearKeyIn, monthKeyIn, 18, 0, 0, 0);

                        if (DateTime.Now > dateDeadLine)
                        {
                            baKeyInData.Remark = "ล่าช้า";
                        }
                    }
                    else if (baKeyInData.Week == "3")
                    {
                        DateTime dateDeadLine = new DateTime(YearKeyIn, monthKeyIn, 25, 0, 0, 0);

                        if (DateTime.Now > dateDeadLine)
                        {
                            baKeyInData.Remark = "ล่าช้า";
                        }
                    }
                    else if (baKeyInData.Week == "4")
                    {
                        DateTime dateDeadLine = new DateTime(YearKeyIn, monthKeyIn, 5, 0, 0, 0).AddMonths(1);

                        if (DateTime.Now > dateDeadLine)
                        {
                            baKeyInData.Remark = "ล่าช้า";
                        }
                    }

                    baKeyInData.Submited_Date = DateTime.Now;
                }

                _dbContext.TTBAKeyIn.Update(baKeyInData);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> ApproveBAKeyIn(Guid baKeyInID, Guid userID)
        {
            try
            {
                var baKeyInData = _dbContext.TTBAKeyIn.Find(baKeyInID);
                baKeyInData.Approved_By = userID;
                baKeyInData.Approved_Date = DateTime.Now;

                _dbContext.TTBAKeyIn.Update(baKeyInData);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RejectBAKeyIn(Guid baKeyInID,Guid userID)
        {
            try
            {
                var keyInStatusReject = _dbContext.TMKeyInStatus.FirstOrDefault(c => c.Status_Name == "Reject");

                var baKeyInData = _dbContext.TTBAKeyIn.Find(baKeyInID);
                baKeyInData.Approved_By = userID;
                baKeyInData.KeyIn_Status_ID = keyInStatusReject.ID;
                baKeyInData.Approved_Date = DateTime.Now;

                _dbContext.TTBAKeyIn.Update(baKeyInData);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateBAKeyInDetail(List<TTBAKeyInDetail> listDetail)
        {
            try
            {
                _dbContext.TTBAKeyInDetail.UpdateRange(listDetail);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
