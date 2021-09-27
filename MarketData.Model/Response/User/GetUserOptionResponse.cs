using MarketData.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.User
{
    public class GetUserOptionResponse : BaseResponse
    {
        public List<DepartmentStoreData> departmentStore { get; set; }
        public List<BrandData> brand { get; set; }
    }
}
