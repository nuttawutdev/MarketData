using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class ImportDataRequest
    {
        public MemoryStream fileStream { get; set; }
        public string filePath { get; set; }
        public string userID { get; set; }
    }
}
