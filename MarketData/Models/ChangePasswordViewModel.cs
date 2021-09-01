using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        public Guid userID { get; set; }

        [Required(ErrorMessage = "Please input password.")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Must be between 8 and 20 characters", MinimumLength = 8)]
        public string password { get; set; }

        [Required(ErrorMessage = "Please input confirm password.")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Your password and confirm password do not match")]
        public string confirmPassword { get; set; }

        public Guid urlID { get; set; }
    }
}
