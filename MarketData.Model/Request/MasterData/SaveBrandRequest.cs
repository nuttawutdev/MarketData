using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class SaveBrandRequest
    {
        public Guid? brandID { get; set; }
        [Required]
        public string brandName { get; set; }
        public string brandShortName { get; set; }
        public Guid brandGroupID { get; set; }
        public string brandGroupName { get; set; }
        public Guid brandSegmentID { get; set; }
        public string brandSegmentName { get; set; }
        public Guid brandTypeID { get; set; }
        public string brandTypeName { get; set; }
        public string brandColor { get; set; }
        public int? lorealBrandRank { get; set; }
        public string universe { get; set; }
        public bool active { get; set; }
        public string userID { get; set; }
    }
}
