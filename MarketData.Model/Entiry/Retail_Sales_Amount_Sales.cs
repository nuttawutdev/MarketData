using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class Retail_Sales_Amount_Sales
    {
        public string Sales_Week { get; set; }
        public string Sales_Month { get; set; }
        public string Sales_Year { get; set; }
        public Guid Store_Id { get; set; }
        public decimal Amount_Sales { get; set; }
        public string Universe { get; set; }
        public Guid BrandG_Id { get; set; }
        public Guid Brand_Type_ID { get; set; }
    }
}
