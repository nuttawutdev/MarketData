using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class BrandTypeListViewModel
    {
        public List<BrandTypeViewModel> brandTypeList { get; set; }
        public int totalRecord { get; set; }
        public int totalPage { get; set; }
    }
}
