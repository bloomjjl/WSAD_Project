using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Areas.Admin.Models.ViewModels.ManageUser
{
    public class SessionListByUserViewModel
    {
        public SessionListByUserViewModel() { }

        public SessionListByUserViewModel(User userDTO)
        {
            UserId = userDTO.Id;
            UserName = userDTO.Username;
            FirstName = userDTO.FirstName;
            LastName = userDTO.LastName;
        }


        public int UserId { get; private set; }
        public string UserName { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<UserSessionItem> UserSessionItems { get; set; }
    }

    public class UserSessionItem
    {
        public int SessionId { get; set; }
        public string SessionTitle { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime DateRegistered { get; set; }
    }
}