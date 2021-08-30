using MarketData.Model.Entiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MarketData.Repositories.Repo
{
    public class UserRepository
    {
        private readonly MarketDataDBContext _dbContext;
        public UserRepository(MarketDataDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TMUser FindUserBy(Expression<Func<TMUser, bool>> expression)
        {
            try
            {
                return _dbContext.TMUser.Where(expression).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TMUser> GetUserBy(Expression<Func<TMUser, bool>> expression)
        {
            try
            {
                return _dbContext.TMUser.Where(expression).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TMUserCounter> GetUserCounterBy(Expression<Func<TMUserCounter, bool>> expression)
        {
            try
            {
                return _dbContext.TMUserCounter.Where(expression).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
