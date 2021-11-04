using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class Loreal_Store
    {
        [Key]
        public Guid Store_Id { get; set; }
    }
}
