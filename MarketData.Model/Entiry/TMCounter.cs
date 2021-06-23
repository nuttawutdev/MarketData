using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMCounter
    {
        [Key]
        public Guid Counter_ID { get; set; }
        public Guid Department_Store_ID { get; set; }
        public Guid Brand_ID { get; set; }
        public Guid Distribution_Channel_ID { get; set; }
        public bool Active_Flag { get; set; }
        public DateTime? Created_Date { get; set; }
        public string Created_By { get; set; }
        public DateTime? Updated_Date { get; set; }
        public string Updated_By { get; set; }
    }
}
