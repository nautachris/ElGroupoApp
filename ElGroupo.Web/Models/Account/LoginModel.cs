using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Account
{
    public class LoginModel
    {
        [Required]
        [UIHint("email")]
        public string Email { get; set; }
        [Required]
        [UIHint("password")]
        public string Password { get; set; }

        public int UtcOffset { get; set; }

        [Display(Description = "Remember Me?")]
        public bool RememberMe { get; set; }

        public CreateAccountModel CreateModel { get; set; }
    }
}
