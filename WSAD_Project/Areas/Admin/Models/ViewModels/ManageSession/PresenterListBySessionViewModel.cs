using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Areas.Admin.Models.ViewModels.ManageSession
{
    public class PresenterListBySessionViewModel
    {
        public PresenterListBySessionViewModel() { }

        public PresenterListBySessionViewModel(Session sessionDTO)
        {
            SessionId = sessionDTO.Id;
            SessionTitle = sessionDTO.Title;
        }


        public int SessionId { get; private set; }
        public string SessionTitle { get; private set; }
        public List<SessionPresenterItem> SessionPresenterItems { get; set; }
    }

    public class SessionPresenterItem
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateRegistered { get; set; }
    }

}