using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class SaveCounterRequest
    {
        public Guid? counterID { get; set; }
        [Required]
        public Guid distributionChannelID { get; set; }
        public string distributionChannelName { get; set; }
        [Required]
        public Guid departmentStoreID { get; set; }
        public string departmentStoreName { get; set; }
        [Required]
        public Guid brandID { get; set; }
        public string brandName { get; set; }
        public string userID { get; set; }
        public bool active { get; set; }
    }
}
