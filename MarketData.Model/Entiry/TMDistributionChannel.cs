using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMDistributionChannel
    {
        public Guid Distribution_Channel_ID { get; set; }
        public string Distribution_Channel_Name { get; set; }
        public bool Active_Flag { get; set; }
        public DateTime? Created_Date { get; set; }
        public string Created_By { get; set; }
        public DateTime? Updated_Date { get; set; }
        public string Updated_By { get; set; }
    }
}
