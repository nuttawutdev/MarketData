using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Request
{
    public class SaveBrandTypeRequest
    {
        public Guid? brandTypeID { get; set; }
        public string brandTypeName { get; set; }
        public bool active { get; set; }
        public DateTime? createdDate { get; set; }
        public string userID { get; set; }
    }
}
