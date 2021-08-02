using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMUserCounter
    {
        [Key]
        public Guid ID { get; set; }
        public Guid User_ID { get; set; }
        public Guid DepartmentStore_ID { get; set; }
        public Guid Brand_ID { get; set; }
        public Guid DistributionChannel_ID { get; set; }
    }
}
