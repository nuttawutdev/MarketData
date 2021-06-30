using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class RetailerGroupViewModel
    {
        public Guid retailerGroupID { get; set; }
        public string retailerGroupName { get; set; }
        public bool active { get; set; }
    }
}
