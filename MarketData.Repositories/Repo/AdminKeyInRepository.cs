using MarketData.Helper;
using MarketData.Model.Entiry;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MarketData.Repositories.Repo
{
    public class AdminKeyInRepository
    {
        private readonly MarketDataDBContext _dbContext;

        public AdminKeyInRepository(MarketDataDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TTAdminKeyInDetail FindAdminKeyInDetailBy(Expression<Func<TTAdminKeyInDetail, bool>> expression)
        {
            try
            {
                return _dbContext.TTAdminKeyInDetail.Where(expression).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TTAdminKeyInDetail CreateAdminKeyInDetail(TTAdminKeyInDetail request)
        {
            try
            {
                _dbContext.TTAdminKeyInDetail.Add(request);

                if (_dbContext.SaveChanges() > 0)
                {
                    return request;
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

        public List<TTAdminKeyInDetail> GetAdminKeyInDetailBy(Expression<Func<TTAdminKeyInDetail, bool>> expression)
        {
            try
            {
                return _dbContext.TTAdminKeyInDetail.Where(expression).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateBrandAdminKeyInDetail(List<Guid> oldBrandList, Guid newBrandID, string userID)
        {
            try
            {
                var adminKeyInDetailByOldBrand = GetAdminKeyInDetailBy(c => oldBrandList.Contains(c.Brand_ID));

                foreach (var item in adminKeyInDetailByOldBrand)
                {
                    item.Previous_BrandID = item.Brand_ID;
                    item.Brand_ID = newBrandID;
                    item.Updated_By = new Guid(userID);
                    item.Updated_Date = Utility.GetDateNowThai();
                }

                _dbContext.TTAdminKeyInDetail.UpdateRange(adminKeyInDetailByOldBrand);

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RestoreBrandAdminKeyInDetail(Guid brandID, string userID)
        {
            try
            {
                var adminKeyInDetailByOldBrand = GetAdminKeyInDetailBy(c => c.Brand_ID == brandID && c.Previous_BrandID != null);

                foreach (var item in adminKeyInDetailByOldBrand)
                {
                    item.Brand_ID = item.Previous_BrandID.GetValueOrDefault();
                    item.Previous_BrandID = null;
                    item.Updated_By = new Guid(userID);
                    item.Updated_Date = Utility.GetDateNowThai();
                }

                _dbContext.TTAdminKeyInDetail.UpdateRange(adminKeyInDetailByOldBrand);

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<TTAdminKeyInDetail> GetAllAdminKeyInDetailBy()
        {
            try
            {
                return _dbContext.TTAdminKeyInDetail.AsNoTracking();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddAdminKeyInDetail(List<TTAdminKeyInDetail> listDetail)
        {
            try
            {
                _dbContext.TTAdminKeyInDetail.AddRange(listDetail);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateAdminKeyInDetail(List<TTAdminKeyInDetail> listDetail)
        {
            try
            {
                _dbContext.TTAdminKeyInDetail.UpdateRange(listDetail);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
