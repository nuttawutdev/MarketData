using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class SaveBrandGroupRequest
    {
        public Guid? brandGroupID { get; set; }
        [Required]
        public string brandGroupName { get; set; }
        public bool isLoreal { get; set; }
        public bool active { get; set; }
        public string userID { get; set; }
    }
}
