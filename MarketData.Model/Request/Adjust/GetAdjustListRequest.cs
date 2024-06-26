﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.Adjust
{
    public class GetAdjustListRequest
    {
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
        public Guid? retailerGroupID { get; set; }
        public Guid? departmentStoreID { get; set; }
        public Guid? distributionChannelID { get; set; }
        public Guid? statusID { get; set; }
        public string universe { get; set; }
    }
}
