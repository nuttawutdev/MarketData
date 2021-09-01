using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMUrl
    {
        [Key]
        public Guid ID { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Type_Url { get; set; }
        public bool Flag_Active { get; set; }
        public DateTime Expire_Date { get; set; }
    }
}
