﻿using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.KeyIn
{
    public class GetBAKeyInOptionResponse : BaseResponse
    {
        public List<DepartmentStoreData> departmentStore { get; set; }
        public List<RetailerGroupData> retailerGroup { get; set; }
        public List<DistributionChannelData> channel { get; set; }
        public List<BrandData> brand { get; set; }
    }
}
