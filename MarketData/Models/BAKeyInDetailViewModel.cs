using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class BAKeyInDetailViewModel
    {
        public Guid BAKeyInID { get; set; }
        public List<BAKeyInDetailData> BAKeyInDetailList { get; set; }
        public List<KeyInRemark> remarkList { get; set; }
        public string departmentStore { get; set; }
        public string channel { get; set; }
        public string brand { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
        public string status { get; set; }
    }

    public class BAKeyInDetailData
    {
        public Guid ID { get; set; }
        public Guid keyInID { get; set; }
        public Guid counterID { get; set; }
        public Guid departmentStoreID { get; set; }
        public Guid channelID { get; set; }
        public Guid brandID { get; set; }
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
    }

    public class KeyInRemark
    {
        public Guid ID { get; set; }
        public string remark { get; set; }
    }
}
