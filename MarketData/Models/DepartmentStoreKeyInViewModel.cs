using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class DepartmentStoreKeyInViewModel
    {
        public Guid departmentStoreID { get; set; }
        public string departmentStoreName { get; set; }
        public Guid retailerGroupID { get; set; }
        public Guid distributionChannelID { get; set; }
    }
}
