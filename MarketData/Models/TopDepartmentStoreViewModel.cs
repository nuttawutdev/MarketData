using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class TopDepartmentStoreViewModel
    {
        public List<DepartmentStoreViewModel> departmentStoreList { get; set; }
        public List<TopDepartmentStoreData> data { get; set; }
    }

    public class TopDepartmentStoreData
    {
        public int topNumber { get; set; }
        public Guid? departmentStoreID { get; set; }
    }
}
