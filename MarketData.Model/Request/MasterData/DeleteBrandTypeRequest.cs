using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class DeleteBrandTypeRequest
    {
        public Guid brandTypeID { get; set; }
        public string userID { get; set; }
    }
}
