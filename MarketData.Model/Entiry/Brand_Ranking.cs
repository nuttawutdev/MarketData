using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class Brand_Ranking
    {
        [Key]
        public Guid ID { get; set; }
        public Guid Brand_ID { get; set; }
        public string Brand_Name { get; set; }
        public Guid BrandG_Id { get; set; }
        public string Universe { get; set; }
        public string Report_Color { get; set; }
        public Guid Store_Id { get; set; }
        public string Sales_Week { get; set; }
        public string Sales_Month { get; set; }
        public string Sales_Year { get; set; }
        public decimal? Amount_Sales { get; set; }
        public decimal? Whole_Sales { get; set; }
        public decimal? Net_Sales { get; set; }
        public Guid Brand_Type_ID { get; set; }
        public bool Is_Loreal_Brand { get; set; }
    }
}
