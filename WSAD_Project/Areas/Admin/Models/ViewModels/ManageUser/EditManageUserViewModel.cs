using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Areas.Admin.Models.ViewModels.ManageUser
{
    public class EditManageUserViewModel
    {
        public EditManageUserViewModel() { }

        public EditManageUserViewModel(User userDTO)
        {
            Id = userDTO.Id;
            FirstName = userDTO.FirstName;
            LastName = userDTO.LastName;
            Username = userDTO.Username;
            EmailAddress = userDTO.EmailAddress;
            Company = userDTO.Company;
            IsActive = userDTO.IsActive;
            IsAdmin = userDTO.IsAdmin;
            IsPresenter = userDTO.IsPresenter;
            Gender = userDTO.Gender;
            DateCreated = userDTO.DateCreated;
        }


        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        public string Company { get; set; }

        [Display(Name = "User Name")]
        public string Username { get; set; }

        [Display(Name = "Active User")]
        public bool IsActive { get; set; }

        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Presenter")]
        public bool IsPresenter { get; set; }

        public string Gender { get; set; }

        [Display(Name = "Date Created")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateCreated { get; set; }
    }
}