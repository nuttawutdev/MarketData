using MarketData.Repositories.Repo;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Repositories
{
    public class Repository
    {
        public MasterDataRepository masterData;
        public BAKeyInRepository baKeyIn;
        public UserRepository user;
        public AdminKeyInRepository adminKeyIn;
        public ApproveRepository approve;
        public AdjustDataRepository adjust;
        public UrlRepository url;

        public Repository(MarketDataDBContext dbContext)
        {
            masterData = new MasterDataRepository(dbContext);
            baKeyIn = new BAKeyInRepository(dbContext);
            user = new UserRepository(dbContext);
            adminKeyIn = new AdminKeyInRepository(dbContext);
            approve = new ApproveRepository(dbContext);
            adjust = new AdjustDataRepository(dbContext);
            url = new UrlRepository(dbContext);
        }
    }
}
