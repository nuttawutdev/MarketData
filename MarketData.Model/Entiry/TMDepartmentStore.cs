using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMDepartmentStore
    {
        public Guid Department_Store_ID { get; set; }
        public string Department_Store_Name { get; set; }
        public Guid Retailer_Group_ID { get; set; }
        public Guid Distribution_Channel_ID { get; set; }
        public Guid Region_ID { get; set; }
        public int? Rank { get; set; }
        public bool Active_Flag { get; set; }
        public DateTime? Created_Date { get; set; }
        public string Created_By { get; set; }
        public DateTime? Updated_Date { get; set; }
        public string Updated_By { get; set; }
    }
}
