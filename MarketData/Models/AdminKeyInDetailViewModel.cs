using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class AdminKeyInDetailViewModel
    {
        public List<AdminKeyInDetailData> data { get; set; } = new List<AdminKeyInDetailData>();
        public string totalAmountPreviosYear { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
    }

    public class AdminKeyInDetailData
    {
        public Guid ID { get; set; }
        public Guid counterID { get; set; }
        public Guid departmentStoreID { get; set; }
        public string departmentStoreName { get; set; }
        public Guid retailerGroupID { get; set; }
        public Guid channelID { get; set; }
        public Guid brandID { get; set; }
        public string brandColor { get; set; }
        public string brandName { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
        public int? rank { get; set; }
        public string amountSalePreviousYear { get; set; }
        public string amountSale { get; set; }
        public string wholeSale { get; set; }
        public string sk { get; set; }
        public string mu { get; set; }
        public string fg { get; set; }
        public string ot { get; set; }
        public string remark { get; set; }
        public string universe { get; set; }
    }
}
