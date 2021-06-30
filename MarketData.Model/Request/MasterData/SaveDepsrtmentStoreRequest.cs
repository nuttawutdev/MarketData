using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class SaveDepsrtmentStoreRequest
    {
        public Guid? departmentStoreID { get; set; }
        [Required]
        public string departmentStoreName { get; set; }
        public Guid retailerGroupID { get; set; }
        public string retailerGroupName { get; set; }
        public Guid distributionChannelID { get; set; }
        public string distributionChannelName { get; set; }
        public Guid? regionID { get; set; }
        public string region { get; set; }
        public int? rank { get; set; }
        public bool active { get; set; }
        public string userID { get; set; }
    }
}
