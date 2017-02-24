using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSAD_Project.Models.ViewModels.Account
{
    public class LoginUserViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}