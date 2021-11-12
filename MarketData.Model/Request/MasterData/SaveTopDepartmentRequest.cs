using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class SaveTopDepartmentRequest
    {
        public List<TopDepartmentStore> departmentStoreList { get; set; }
    }

    public class TopDepartmentStore
    {
        public int topNumber { get; set; }
        public Guid? departmentStoreID { get; set; }
    }
}
