using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMTopDepartmentStore
    {
        [Key]
        public Guid ID { get; set; }
        public int TopNumber { get; set; }
        public Guid? DepartmentStore_ID { get; set; }
    }
}
