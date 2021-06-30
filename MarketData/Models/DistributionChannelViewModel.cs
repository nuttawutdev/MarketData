using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class DistributionChannelViewModel
    {
        public Guid distributionChannelID { get; set; }
        public string distributionChannelName { get; set; }
        public bool active { get; set; }
    }
}
