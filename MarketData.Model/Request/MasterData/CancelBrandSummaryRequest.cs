using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class CancelBrandSummaryRequest
    {
        public Guid brandID { get; set; }
        public string userID { get; set; }
    }
}
