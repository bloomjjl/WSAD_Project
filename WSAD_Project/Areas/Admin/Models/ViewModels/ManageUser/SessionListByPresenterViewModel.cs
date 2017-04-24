using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Areas.Admin.Models.ViewModels.ManageUser
{
    public class SessionListByPresenterViewModel
    {
        public SessionListByPresenterViewModel() { }

        public SessionListByPresenterViewModel(User presenterDTO)
        {
            UserId = presenterDTO.Id;
            UserName = presenterDTO.Username;
            FirstName = presenterDTO.FirstName;
            LastName = presenterDTO.LastName;
        }


        public int UserId { get; private set; }
        public string UserName { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<PresenterSessionItem> PresenterSessionItems { get; set; }
    }

    public class PresenterSessionItem
    {
        public int SessionId { get; set; }

        [Display(Name = "Session Title")]
        public string SessionTitle { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime DateRegistered { get; set; }
    }
}