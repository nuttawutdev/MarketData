using MarketData.Model.Entiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MarketData.Repositories.Repo
{
    public class UrlRepository
    {
        private readonly MarketDataDBContext _dbContext;
        public UrlRepository(MarketDataDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TMUrl> CreateUrl(string ref1,DateTime expireDate,string type,string ref2 = null)
        {
            try
            {
                TMUrl newUrl = new TMUrl
                {
                    ID = Guid.NewGuid(),
                    Ref1 = ref1,
                    Flag_Active = true,
                    Type_Url = type,
                    Ref2 = ref2,
                    Expire_Date = expireDate
                };

                _dbContext.TMUrl.Add(newUrl);

                if(await _dbContext.SaveChangesAsync() > 0)
                {
                    return newUrl;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public TMUrl GetUrlDataBy(Expression<Func<TMUrl, bool>> expression)
        {
            try
            {
                try
                {
                    return _dbContext.TMUrl.Where(expression).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateUrlData(TMUrl urlData)
        {
            try
            {
                _dbContext.TMUrl.Update(urlData);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UnActiveOldUrl(string ref1,string type)
        {
            try
            {
                var oldUrl = _dbContext.TMUrl.Where(c=>c.Ref1 == ref1 && c.Type_Url == type);

                foreach(var itemUrl in oldUrl)
                {
                    itemUrl.Flag_Active = false;
                }

                _dbContext.TMUrl.UpdateRange(oldUrl);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
