using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TTAdminKeyInDetail
    {
        [Key]
        public Guid ID { get; set; }
        public Guid? Counter_ID { get; set; }
        public Guid DistributionChannel_ID { get; set; }
        public Guid RetailerGroup_ID { get; set; }
        public Guid DepartmentStore_ID { get; set; }
        public Guid Brand_ID { get; set; }
        public string Universe { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Week { get; set; }
        public int? Rank { get; set; }
        public decimal? Amount_Sales { get; set; }
        public decimal? Whole_Sales { get; set; }
        public decimal? SK { get; set; }
        public decimal? MU { get; set; }
        public decimal? FG { get; set; }
        public decimal? OT { get; set; }
        public string Remark { get; set; }
        public Guid Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public Guid? Updated_By { get; set; }
        public DateTime? Updated_Date { get; set; }
        public Guid? Previous_BrandID { get; set; }
    }
}
