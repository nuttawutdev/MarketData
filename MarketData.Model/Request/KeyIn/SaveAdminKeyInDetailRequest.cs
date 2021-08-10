using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.KeyIn
{
    public class SaveAdminKeyInDetailRequest
    {
        public List<AdminKeyInDetailData> data { get; set; }
        public Guid userID { get; set; }
    }
}
