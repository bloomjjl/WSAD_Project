using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Areas.Admin.Models.ViewModels.ManageSession
{
    public class SessionListByUserViewModel
    {
        public SessionListByUserViewModel() { }

        public SessionListByUserViewModel(int userId, string userName)
        {
            UserId = userId;
            UserName = UserName;
        }


        public int UserId { get; private set; }
        public string UserName { get; private set; }
        public List<SessionItem> SessionItems { get; set; }
    }

    public class SessionItem
    {
        public int SessionId { get; set; }
        public string SessionTitle { get; set; }
    }
}