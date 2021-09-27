using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TTApproveKeyIn
    {
        [Key]
        public Guid ID { get; set; }
        public Guid BAKeyIn_ID { get; set; }
        public Guid Status_ID { get; set; }
        public string Remark { get; set; }
        public string BA_Remark { get; set; }
        public Guid? Action_By { get; set; }
        public DateTime? Action_Date { get; set; }
    }
}
