using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Data
{
    public class CounterData
    {
        public Guid counterID { get; set; }
        public Guid distributionChannelID { get; set; }
        public string distributionChannelName { get; set; }
        public Guid retailerGroupID { get; set; }
        public string retailerGroupName { get; set; }
        public Guid departmentStoreID { get; set; }
        public string departmentStoreName { get; set; }
        public Guid brandID { get; set; }
        public string brandName { get; set; }
        public bool active { get; set; }
        public bool alwayShow { get; set; }
        public bool alwayShowKeyIn { get; set; }
        public DateTime? createDate { get; set; }
    }
}
