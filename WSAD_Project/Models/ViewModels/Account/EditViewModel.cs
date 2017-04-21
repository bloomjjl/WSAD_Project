using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSAD_Project.Models.ViewModels.Account
{
    public class EditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        public string Company { get; set; }

        public string Gender { get; set; }

        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        public string PasswordConfirm { get; set; }
    }
}