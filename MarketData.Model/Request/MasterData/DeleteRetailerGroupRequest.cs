using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class DeleteRetailerGroupRequest
    {
        public Guid retailerGroupID { get; set; }
        public string userID { get; set; }
    }
}
