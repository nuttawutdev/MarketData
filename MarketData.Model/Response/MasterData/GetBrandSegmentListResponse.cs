﻿using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.MasterData
{
    public class GetBrandSegmentListResponse : BaseResponse
    {
        public List<BrandSegmentData> data { get; set; }
    }
}
