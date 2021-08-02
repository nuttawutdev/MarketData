using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TTBAKeyInDetail
    {
        [Key]
        public Guid ID { get; set; }
        public Guid BAKeyIn_ID { get; set; }
        public Guid Counter_ID { get; set; }
        public Guid DepartmentStore_ID { get; set; }
        public Guid Brand_ID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
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
    }
}
