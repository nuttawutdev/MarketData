using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMBrand
    {
        [Key]
        public Guid Brand_ID { get; set; }
        public string Brand_Name { get; set; }
        public string Brand_Short_Name { get; set; }
        public Guid Brand_Group_ID { get; set; }
        public Guid Brand_Segment_ID { get; set; }
        public Guid Brand_Type_ID { get; set; }
        public string Brand_Color { get; set; }
        public int? Loreal_Brand_Rank { get; set; }
        public string Universe { get; set; }
        public bool Active_Flag { get; set; }
        public DateTime? Created_Date { get; set; }
        public string Created_By { get; set; }
        public DateTime? Updated_Date { get; set; }
        public string Updated_By { get; set; }
    }
}
