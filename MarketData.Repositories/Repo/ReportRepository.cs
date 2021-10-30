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
    }
}
