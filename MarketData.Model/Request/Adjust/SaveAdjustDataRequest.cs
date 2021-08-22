﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request.Adjust
{
    public class SaveAdjustDataRequest
    {
        public Guid retailerGroupID { get; set; }
        public Guid departmentStoreID { get; set; }
        public Guid distributionChannelID { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string week { get; set; }
        public string status { get; set; }
        public string universe { get; set; }
    }

    public class SaveAdjustDetailData
    {
        public Guid brandID { get; set; }
        public decimal? amountPreviousYear { get; set; }
        public decimal? adminAmountSale { get; set; }
        public decimal? adjustAmountSale { get; set; }
        public decimal? adjustWholeSale { get; set; }
        public int rank { get; set; }
        public decimal? sk { get; set; }
        public decimal? mu { get; set; }
        public decimal? fg { get; set; }
        public decimal? ot { get; set; }
        public string remark { get; set; }
        public decimal? percentGrowth { get; set; }
        public Dictionary<string, decimal?> brandKeyInAmount { get; set; }
        public Dictionary<string, string> brandKeyInRank { get; set; }
    }
}