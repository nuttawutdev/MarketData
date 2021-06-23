using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response
{
    public class BaseGetDataListResponse
    {
        public int totalRecord { get; set; }
        public int totalPage { get; set; }
    }
}
