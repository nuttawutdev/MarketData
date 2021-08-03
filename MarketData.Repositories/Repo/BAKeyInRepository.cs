using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Repositories.Repo
{
    public class BAKeyInRepository
    {
        private readonly MarketDataDBContext _dbContext;
        public BAKeyInRepository(MarketDataDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        
    }
}
