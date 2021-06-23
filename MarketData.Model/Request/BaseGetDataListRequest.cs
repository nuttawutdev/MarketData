using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request
{
    public class BaseGetDataListRequest
    {
        public string active { get; set; }
        public int pageNo { get; set; }
        public int pageSize { get; set; }
        public string sortBy { get; set; }
        public string sortColumn { get; set; }
        public string textSearch { get; set; }
    }
}
