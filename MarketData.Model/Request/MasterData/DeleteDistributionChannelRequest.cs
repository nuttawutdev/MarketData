using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class DeleteDistributionChannelRequest
    {
        public Guid distributionChannelID { get; set; }
        public string userID { get; set; }
    }
}
