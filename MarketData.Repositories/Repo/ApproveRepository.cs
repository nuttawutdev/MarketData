using MarketData.Helper;
using MarketData.Model.Data;
using MarketData.Model.Entiry;
using MarketData.Model.Response.Approve;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MarketData.Repositories.Repo
{
    public class ApproveRepository
    {
        private readonly MarketDataDBContext _dbContext;

        public ApproveRepository(MarketDataDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateApproveKeyInData(TTBAKeyIn request)
        {
            try
            {
                var submittedStatus = _dbContext.TMApproveStatus.FirstOrDefault(c => c.Status_Name == "Submitted");

                TTApproveKeyIn aaproveKeyInData = new TTApproveKeyIn
                {
                    ID = Guid.NewGuid(),
                    BAKeyIn_ID = request.ID,
                    Status_ID = submittedStatus.ID,
                    BA_Remark = request.Remark
                };

                _dbContext.TTApproveKeyIn.Add(aaproveKeyInData);


                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ApproveKeyInData> GetApproveKeyInData()
        {
            try
            {
                var approveKeyInData = _dbContext.TTApproveKeyIn.AsNoTracking();

                var approveKeyInDataList = (
                    from a in approveKeyInData
                    join baKeyIn in _dbContext.TTBAKeyIn
                        on a.BAKeyIn_ID equals baKeyIn.ID
                        into joinBAKeyIn
                    from baKeyIn in joinBAKeyIn.DefaultIfEmpty()
                    join b in _dbContext.TMBrand
                        on baKeyIn.Brand_ID equals b.Brand_ID
                        into joinBrand
                    from brand in joinBrand.DefaultIfEmpty()
                    join d in _dbContext.TMDepartmentStore
                        on baKeyIn.DepartmentStore_ID equals d.Department_Store_ID
                        into joinStore
                    from store in joinStore.DefaultIfEmpty()
                    join c in _dbContext.TMDistributionChannel
                       on baKeyIn.DistributionChannel_ID equals c.Distribution_Channel_ID
                       into joinChannel
                    from channel in joinChannel.DefaultIfEmpty()
                    join s in _dbContext.TMApproveStatus
                        on a.Status_ID equals s.ID
                        into joinStatus
                    from status in joinStatus.DefaultIfEmpty()
                    join u in _dbContext.TMUser
                        on a.Action_By equals u.ID
                        into joinUser
                    from user in joinUser.DefaultIfEmpty()
                    select new ApproveKeyInData
                    {
                        approveKeyInID = a.ID,
                        baKeyInID = a.BAKeyIn_ID,
                        brandID = brand != null ? brand.Brand_ID : Guid.Empty,
                        brandName = brand != null ? brand.Brand_Name : string.Empty,
                        departmentStoreID = store.Department_Store_ID,
                        departmentStoreName = store.Department_Store_Name,
                        distributionChannelID = channel.Distribution_Channel_ID,
                        distributionChannelName = channel.Distribution_Channel_Name,
                        year = baKeyIn.Year,
                        month = baKeyIn.Month,
                        week = baKeyIn.Week,
                        retailerGroupID = store.Retailer_Group_ID,
                        statusID = status.ID,
                        statusName = status.Status_Name,
                        dateApprove = a.Action_Date,
                        approveDate = a.Action_Date.HasValue ? a.Action_Date.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") : "",
                        approver = user != null ? user.DisplayName : string.Empty,
                        universe = baKeyIn.Universe
                    }).ToList();

                return approveKeyInDataList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TTApproveKeyIn> GetApproveKeyInBy(Expression<Func<TTApproveKeyIn, bool>> expression)
        {
            try
            {
                return _dbContext.TTApproveKeyIn.Where(expression).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TTApproveKeyInDetail> GetApproveKeyInDetail(Expression<Func<TTApproveKeyInDetail, bool>> expression)
        {
            try
            {
                return _dbContext.TTApproveKeyInDetail.Where(expression).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateBrandApproveKeyInDetail(List<Guid> oldBrandList, Guid newBrandID, string userID)
        {
            try
            {
                var approveKeyInDetailByOldBrand = GetApproveKeyInDetail(c => oldBrandList.Contains(c.Brand_ID));

                foreach (var item in approveKeyInDetailByOldBrand)
                {
                    item.Previous_BrandID = item.Brand_ID;
                    item.Brand_ID = newBrandID;
                    item.Updated_By = new Guid(userID);
                    item.Updated_Date = Utility.GetDateNowThai();
                }

                _dbContext.TTApproveKeyInDetail.UpdateRange(approveKeyInDetailByOldBrand);

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TTApproveKeyIn FindApproveKeyInBy(Expression<Func<TTApproveKeyIn, bool>> expression)
        {
            try
            {
                return _dbContext.TTApproveKeyIn.Where(expression).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateApproveKeyInData(TTApproveKeyIn updateData)
        {
            try
            {
                _dbContext.TTApproveKeyIn.Update(updateData);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> InsertApproveKeyInDetail(List<TTApproveKeyInDetail> dataDetail)
        {
            try
            {
                _dbContext.TTApproveKeyInDetail.AddRange(dataDetail);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BAKeyInDetailData> GetApproveKeyInDetailBy(Expression<Func<TTApproveKeyInDetail, bool>> expression)
        {
            try
            {
                var approveKeyInDetail = _dbContext.TTApproveKeyInDetail.Where(expression).ToList();

                var baKeyInDetailDataList = (
                     from e in approveKeyInDetail
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
                         keyInID = e.ApproveKeyIn_ID,
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
    }
}
