using MarketData.Model.Entiry;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MarketData.Repositories.Repo
{
    public class ReportRepository
    {
        private readonly MarketDataDBContext _dbContext;

        public ReportRepository(MarketDataDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Brand_Ranking> GetBrandRankingBy(Expression<Func<Brand_Ranking, bool>> expression)
        {
            try
            {
                return _dbContext.Brand_Ranking.AsNoTracking().Where(expression).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Loreal_Store> GetLorealStore()
        {
            try
            {
                return _dbContext.Loreal_Store.AsNoTracking().ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Brand_Frangances> GetBrandFragances()
        {
            try
            {
                return _dbContext.Brand_Frangances.AsNoTracking().ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
