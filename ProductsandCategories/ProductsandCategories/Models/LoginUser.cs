using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginAndRegistration.Models
{
    public class LoginUser
    {
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; internal set; }
        [EmailAddress]
        [Required]
        public string Email { get; internal set; }
    }
}
