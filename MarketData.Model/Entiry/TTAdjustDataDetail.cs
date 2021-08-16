using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TTAdjustDataDetail
    {
        [Key]
        public Guid ID { get; set; }
        public Guid AdjustData_ID { get; set; }
        public Guid Brand_ID { get; set; }
        public decimal? Amount_PreviousYear { get; set; }
        public decimal? Admin_AmountSale { get; set; }
        public decimal? Adjust_AmountSale { get; set; }
        public int? Rank { get; set; }
        public decimal? SK { get; set; }
        public decimal? MU { get; set; }
        public decimal? FG { get; set; }
        public decimal? OT { get; set; }
        public string Remark { get; set; }
        public decimal? Percent_Growth { get; set; }
    }
}
