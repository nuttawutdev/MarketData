using MarketData.Repositories.Repo;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Repositories
{
    public class Repository
    {
        public MasterDataRepository masterData;

        public Repository(MarketDataDBContext dbContext)
        {
            masterData = new MasterDataRepository(dbContext);
        }
    }
}
