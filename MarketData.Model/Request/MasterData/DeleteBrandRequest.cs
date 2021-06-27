using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class DeleteBrandRequest
    {
        public Guid brandID { get; set; }
        public string userID { get; set; }
    }
}
