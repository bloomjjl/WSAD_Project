using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Areas.Admin.Models.ViewModels.ManageSession
{
    public class UserListBySessionViewModel
    {
        public UserListBySessionViewModel() { }

        public UserListBySessionViewModel(int sessionId, string sessionTitle)
        {
            SessionId = sessionId;
            SessionTitle = sessionTitle;
        }


        public int SessionId { get; private set; }
        public string SessionTitle { get; private set; }
        public List<UserItem> UserItems { get; set; }
    }

    public class UserItem
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateRegistered { get; set; }
    }

}