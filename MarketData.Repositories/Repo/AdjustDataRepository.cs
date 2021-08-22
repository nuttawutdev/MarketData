using MarketData.Model.Entiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MarketData.Repositories.Repo
{
    public class AdjustDataRepository
    {
        private readonly MarketDataDBContext _dbContext;

        public AdjustDataRepository(MarketDataDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<TTAdjustData> GetAdjustDatalBy(Expression<Func<TTAdjustData, bool>> expression)
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

        public TTAdjustData FindAdjustDatalBy(Expression<Func<TTAdjustData, bool>> expression)
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
    }
}
