﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.MasterData
{
    public class GetBrandDetailResponse : BaseResponse
    {
        public Guid brandID { get; set; }
        public string brandName { get; set; }
        public string brandShortName { get; set; }
        public Guid brandGroupID { get; set; }
        public Guid brandSegmentID { get; set; }
        public Guid brandTypeID { get; set; }
        public string brandColor { get; set; }
        public int? lorealBrandRank { get; set; }
        public string universe { get; set; }
        public bool active { get; set; }
    }
}
