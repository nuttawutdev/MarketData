using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Data
{
    public class DistributionChannelData
    {
        public Guid distributionChannelID { get; set; }
        public string distributionChannelName { get; set; }
        public bool active { get; set; }
        public DateTime? createdDate { get; set; }
    }
}
