using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class SaveCounterRequest
    {
        public Guid? counterID { get; set; }
        public Guid distributionChannelID { get; set; }
        public string distributionChannelName { get; set; }
        public Guid departmentStoreID { get; set; }
        public string departmentStoreName { get; set; }
        public List<Guid> brandID { get; set; }
        public string brandName { get; set; }
        public string userID { get; set; }
        public bool active { get; set; }
        public bool alwayShow { get; set; }
        public bool alwayShowKeyIn { get; set; }
        public int row { get; set; }
    }
}
