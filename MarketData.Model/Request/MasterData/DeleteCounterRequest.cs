using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class DeleteCounterRequest
    {
        public Guid counterID { get; set; }
        public string userID { get; set; }
    }
}
