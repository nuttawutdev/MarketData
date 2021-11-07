using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class Data_Exporting
    {
        [Key]
        public Guid ID { get; set; }
        public string StoreG_Name { get; set; }
        public string Store_Name { get; set; }
        public int? Store_Rank { get; set; }
        public string Region_Name { get; set; }
        public string BrandG_Name { get; set; }
        public string Brand_Type_Name { get; set; }
        public string Brand_Segment_Name { get; set; }
        public string Brand_Name { get; set; }
        public string Sales_Week { get; set; }
        public string Sales_Month { get; set; }
        public string Sales_Year { get; set; }
        public decimal? Amount_Sales { get; set; }
        public decimal? Whole_Sales { get; set; }
        public string Universe { get; set; }
        public Guid Store_Id { get; set; }
        public int? Time_Keyin { get; set; }
    }
}
