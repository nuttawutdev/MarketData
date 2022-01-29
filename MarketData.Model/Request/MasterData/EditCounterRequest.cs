using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class EditCounterRequest
    {
        public Guid? counterID { get; set; }
        public Guid distributionChannelID { get; set; }
        public string distributionChannelName { get; set; }
        public Guid departmentStoreID { get; set; }
        public string departmentStoreName { get; set; }
        public Guid brandID { get; set; }
        public string brandName { get; set; }
        public string userID { get; set; }
        public bool active { get; set; }
        public bool alwayShow { get; set; }
        public int row { get; set; }
    }
}
