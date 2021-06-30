using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class BrandGroupViewModel
    {
        public Guid brandGroupID { get; set; }
        public string brandGroupName { get; set; }
        public bool isLoreal { get; set; }
        public bool active { get; set; }
    }
}
