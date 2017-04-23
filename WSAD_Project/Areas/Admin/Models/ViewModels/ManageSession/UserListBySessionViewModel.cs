using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Areas.Admin.Models.ViewModels.ManageSession
{
    public class UserListBySessionViewModel
    {
        public UserListBySessionViewModel() { }

        public UserListBySessionViewModel(Session sessionDTO)
        {
            SessionId = sessionDTO.Id;
            SessionTitle = sessionDTO.Title;
        }


        public int SessionId { get; private set; }
        public string SessionTitle { get; private set; }
        public List<SessionUserItem> SessionUserItems { get; set; }
    }

    public class SessionUserItem
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime DateRegistered { get; set; }
    }

}