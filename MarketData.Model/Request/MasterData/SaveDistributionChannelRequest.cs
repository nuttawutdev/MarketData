using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class SaveDistributionChannelRequest
    {
        public Guid? distributionChannelID { get; set; }
        public string distributionChannelName { get; set; }
        public bool active { get; set; }
        public string userID { get; set; }
    }
}
