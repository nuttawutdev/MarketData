using MarketData.Model.Entiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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
               
                if(_dbContext.SaveChanges() > 0)
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
    }
}
