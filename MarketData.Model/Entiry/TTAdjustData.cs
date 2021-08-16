using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TTAdjustData
    {
        [Key]
        public Guid ID { get; set; }
        public Guid DistributionChannel_ID { get; set; }
        public Guid RetailerGroup_ID { get; set; }
        public Guid DepartmentStore_ID { get; set; }
        public string Week { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Universe { get; set; }
        public Guid Status_ID { get; set; }
        public Guid Create_By { get; set; }
        public DateTime Create_Date { get; set; }
        public Guid? Update_By { get; set; }
        public DateTime? Update_Date { get; set; }
    }
}
