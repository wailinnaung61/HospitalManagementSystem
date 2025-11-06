using System;
using System.ComponentModel.DataAnnotations;

namespace MSIS_HMS.Models
{
    public class AuthenticateRequest
    {
        public AuthenticateRequest()
        {
            RememberMe = false;
            LockoutOnFailure = false;
        }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public bool LockoutOnFailure { get; set; }
    }
}
