using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.KeyIn
{
    public class SaveBAKeyInDetailRequest
    {
        public Guid BAKeyInID { get; set; }
        public List<BAKeyInDetailData> BAKeyInDetailList { get; set; }
        public Guid userID { get; set; }
    }
}
