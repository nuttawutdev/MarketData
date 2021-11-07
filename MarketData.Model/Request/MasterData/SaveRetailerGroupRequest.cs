using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class SaveRetailerGroupRequest
    {
        public Guid? retailerGroupID { get; set; }
        public string retailerGroupName { get; set; }
        public bool active { get; set; }
        public string userID { get; set; }
    }
}
