﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Data
{
    public class BrandTypeData
    {
        public Guid brandTypeID { get; set; }
        public string brandTypeName { get; set; }
        public bool active { get; set; }
        public DateTime? createdDate { get; set; }
    }
}
