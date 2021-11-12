using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.MasterData
{
    public class GetTopDepartmentStoreResponse : BaseResponse
    {
        public List<TopDepartmentStoreData> data { get; set; }
    }

    public class TopDepartmentStoreData
    {
        public int topNumber { get; set; }
        public Guid? departmentStoreID { get; set; }
    }
}
