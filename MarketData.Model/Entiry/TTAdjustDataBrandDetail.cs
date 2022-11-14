using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TTAdjustDataBrandDetail
    {
        [Key]
        public Guid ID { get; set; }
        public Guid BrandCounter_ID { get; set; }
        public Guid AdjustData_ID { get; set; }
        public Guid? Brand_ID { get; set; }
        public decimal? Amount_Sale { get; set; }
        public int? Rank { get; set; }
        public Guid? Previous_BrandID { get; set; }
    }
}
