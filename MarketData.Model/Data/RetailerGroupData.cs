using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Data
{
    public class RetailerGroupData
    {
        public Guid retailerGroupID { get; set; }
        public string retailerGroupName { get; set; }
        public bool active { get; set; }
        public DateTime? createdDate { get; set; }
    }
}
