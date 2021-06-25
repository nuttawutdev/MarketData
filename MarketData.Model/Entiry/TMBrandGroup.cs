using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMBrandGroup
    {
        [Key]
        public Guid Brand_Group_ID { get; set; }
        public string Brand_Group_Name { get; set; }
        public bool Is_Loreal_Brand { get; set; }
        public bool Active_Flag { get; set; }
        public bool? Delete_Flag { get; set; }
        public DateTime? Created_Date { get; set; }
        public string Created_By { get; set; }
        public DateTime? Updated_Date { get; set; }
        public string Updated_By { get; set; }
    }
}
