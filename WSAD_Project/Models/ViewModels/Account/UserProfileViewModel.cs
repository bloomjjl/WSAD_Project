using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSAD_Project.Models.ViewModels.Account
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        public string Company { get; set; }

        public string Gender { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Date Created")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Presenter")]
        public bool IsPresenter { get; set; }
    }
}