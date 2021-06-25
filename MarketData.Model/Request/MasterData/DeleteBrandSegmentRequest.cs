using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class DeleteBrandSegmentRequest
    {
        public Guid brandSegmentID { get; set; }
        public string userID { get; set; }
    }
}
