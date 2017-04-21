using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Areas.Admin.Models.ViewModels.ManageUser
{
    public class ManageUserViewModel
    {
        public ManageUserViewModel()
        {

        }

        public ManageUserViewModel(User userDTO)
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Company { get; set; }
        public string Username { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsPresenter { get; set; }
        public string Gender { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsSelected { get; set; }
    }
}