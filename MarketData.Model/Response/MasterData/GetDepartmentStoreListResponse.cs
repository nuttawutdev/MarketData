﻿using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.MasterData
{
    public class GetDepartmentStoreListResponse : BaseResponse
    {
        public List<DepartmentStoreData> data { get; set; }
    }
}
