using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please input email.")]
        [EmailAddress]
        public string email { get; set; }

        [Required(ErrorMessage = "Please input password.")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Password must be between 8 and 20 characters", MinimumLength = 8)]
        public string password { get; set; }
    }
}
