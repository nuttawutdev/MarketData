using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Data
{
    public class BrandGroupData
    {
        public Guid brandGroupID { get; set; }
        public string brandGroupName { get; set; }
        public bool isLoreal { get; set; }
        public bool active { get; set; }
        public DateTime? createdDate { get; set; }
    }
}
