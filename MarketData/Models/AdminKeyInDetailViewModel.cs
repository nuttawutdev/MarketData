﻿using System;
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
        public string brandName { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
        public int? rank { get; set; }
        public decimal? amountSalePreviousYear { get; set; }
        public decimal? amountSale { get; set; }
        public decimal? wholeSale { get; set; }
        public decimal? sk { get; set; }
        public decimal? mu { get; set; }
        public decimal? fg { get; set; }
        public decimal? ot { get; set; }
        public string remark { get; set; }
        public string universe { get; set; }
    }
}
