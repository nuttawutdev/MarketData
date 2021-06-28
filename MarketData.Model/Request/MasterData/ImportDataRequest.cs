using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class ImportDataRequest
    {
        public string filePath { get; set; }
        public string userID { get; set; }
    }
}
