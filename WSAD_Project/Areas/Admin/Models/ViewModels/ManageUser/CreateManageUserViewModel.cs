using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Areas.Admin.Models.ViewModels.ManageUser
{
    public class CreateManageUserViewModel
    {
        public CreateManageUserViewModel() { }

        public CreateManageUserViewModel(User userDTO)
        {
            FirstName = userDTO.FirstName;
            LastName = userDTO.LastName;
            UserName = userDTO.Username;
            EmailAddress = userDTO.EmailAddress;
            IsAdmin = userDTO.IsAdmin;
            IsPresenter = userDTO.IsPresenter;
            Gender = userDTO.Gender;
        }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string PasswordConfirm { get; set; }

        [Display(Name = "Check To Receive Emails")]
        public bool ReceiveEmails { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsPresenter { get; set; }
    }
}