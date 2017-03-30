using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WSAD_Project.Areas.Admin.Controllers
{
    public class ManageUserSessionsController : Controller
    {
        // GET: Admin/ManageUserSessions
        public ActionResult Index()
        {
            // get list of available sessions 
            // get list of users in each session

            return View();
        }
    }
}