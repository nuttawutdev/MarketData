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
                var queryResult = _dbContext.Brand_Ranking.AsNoTracking().Where(expression).ToList();
                var result = queryResult.GroupBy(
                        x => new
                        {
                            x.Brand_ID,
                            x.BrandG_Id,
                            x.Universe,
                            x.Sales_Week,
                            x.Sales_Month,
                            x.Sales_Year,
                            x.Brand_Type_ID,
                            x.Store_Id,
                            x.Time_Keyin
                        })
                    .Select(e => new Brand_Ranking
                    {
                        ID = e.FirstOrDefault().ID,
                        Brand_ID = e.Key.Brand_ID,
                        Brand_Name = e.FirstOrDefault().Brand_Name,
                        BrandG_Id = e.Key.BrandG_Id,
                        Universe = e.Key.Universe,
                        Amount_Sales = e.FirstOrDefault().Amount_Sales,
                        Brand_Type_ID = e.Key.Brand_Type_ID,
                        Brand_Type_Name = e.FirstOrDefault().Brand_Type_Name,
                        Department_Store_Name = e.FirstOrDefault().Department_Store_Name,
                        Is_Loreal_Brand = e.FirstOrDefault().Is_Loreal_Brand,
                        Net_Sales = e.FirstOrDefault().Net_Sales,
                        Report_Color = e.FirstOrDefault().Report_Color,
                        Sales_Month = e.Key.Sales_Month,
                        Sales_Week = e.Key.Sales_Week,
                        Sales_Year = e.Key.Sales_Year,
                        Store_Id = e.Key.Store_Id,
                        Time_Keyin = e.Key.Time_Keyin,
                        Whole_Sales = e.FirstOrDefault().Whole_Sales
                    }).ToList();
                return result;

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

        public List<Data_Exporting> GetDataExportingBy(Expression<Func<Data_Exporting, bool>> expression)
        {
            try
            {
                return _dbContext.Data_Exporting.AsNoTracking().Where(expression).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
