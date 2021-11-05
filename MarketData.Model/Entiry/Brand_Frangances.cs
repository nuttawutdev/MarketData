using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class Brand_Frangances
    {
        [Key]
        public Guid Brand_ID { get; set; }
        public string Brand_Name { get; set; }
    }
}
