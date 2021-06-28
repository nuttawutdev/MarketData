using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.MasterData
{
    public class DeleteDepartmentStoreRequest
    {
        public Guid departmentStoreID { get; set; }
        public string userID { get; set; }
    }
}
