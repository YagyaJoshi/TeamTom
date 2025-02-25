using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace TommDAL.Models
{
    public class AdminUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
    }


    public class ChangePasswordAdmin
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
