using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMUserToken
    {
        [Key]
        public Guid ID { get; set; }
        public Guid User_ID { get; set; }
        public string Token_ID { get; set; }
        public DateTime Token_ExpireTime { get; set; }
        public bool FlagActive { get; set; }
    }
}
