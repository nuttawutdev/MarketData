using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TTBAKeyIn
    {
        [Key]
        public Guid ID { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Week { get; set; }
        public Guid DistributionChannel_ID { get; set; }
        public Guid RetailerGroup_ID { get; set; }
        public Guid DepartmentStore_ID { get; set; }
        public Guid Brand_ID { get; set; }
        public string Universe { get; set; }
        public Guid KeyIn_Status_ID { get; set; }
        public Guid? Created_By { get; set; }
        public DateTime? Created_Date { get; set; }
        public Guid? Updated_By { get; set; }
        public DateTime? Updated_Date { get; set; }
        public Guid? Approved_By { get; set; }
        public DateTime? Approved_Date { get; set; }
        public DateTime? Submited_Date { get; set; }
        public string Remark { get; set; }
    }
}
