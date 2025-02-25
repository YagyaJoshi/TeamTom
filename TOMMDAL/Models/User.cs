using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;
using System.Xml.Linq;

namespace TommDAL.Models
{
    public partial class User
    {
        public User()
        {
            UserHolidays = new HashSet<UserHolidays>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
        public string Ran { get; set; }
        public string Country { get; set; }
        public string ResetToken { get; set; }
        public int App_Version { get; set; }
        public int IsMultiprofile { get; set; }
        public Boolean DataSyn { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }
        public Boolean NotifyMe { get; set; }        

        public string Childrens { get; set; }   
        public virtual ICollection<UserHolidays> UserHolidays { get; set; }

    }
    public class Childrens
    {
        public int UserId { get; set; }
        public string Children { get; set; }
    }

    public class UserV5
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }           
    }
    public class UserData
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public int IsSave  { get; set; }
    }
   
}
