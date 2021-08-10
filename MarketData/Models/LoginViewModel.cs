using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class LoginViewModel
    {
        [Required]
        public string userName { get; set; }
        public string password { get; set; }
    }
}
