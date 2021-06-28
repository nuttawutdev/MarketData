﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Data
{
    public class DepartmentStoreData
    {
        public Guid departmentStoreID { get; set; }
        public string departmentStoreName { get; set; }
        public string retailerGroupName { get; set; }
        public string distributionChannelName { get; set; }
        public string region { get; set; }
        public int? rank { get; set; }
        public bool active { get; set; }
    }
}
