using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class SaveBrandSegmentRequest
    {
        public Guid? brandSegmentID { get; set; }
        [Required]
        public string brandSegmentName { get; set; }
        public bool active { get; set; }
        public DateTime? createdDate { get; set; }
        public string userID { get; set; }
    }
}
