using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Data
{
    public class UserCounterData
    {
        public Guid? userCounterID { get; set; }
        public string departmentStoreName { get; set; }
        public Guid departmentStoreID { get; set; }
        public string brandName { get; set; }
        public Guid brandID { get; set; }
        public string channelName { get; set; }
        public Guid channelID { get; set; }
    }
}
