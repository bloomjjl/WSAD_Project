using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WSAD_Project.Models.Data;

namespace WSAD_Project
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }



        protected void Application_AuthenticateRequest()
        {
            // exit if no user set up
            if (Context.User == null) { return; }

            // Get current user username
            string username = Context.User.Identity.Name;

            // Setup a DbContext
            string[] roles = null;
            using (WSADDbContext context = new WSADDbContext())
            {
                // add roles to Principal Object
                User userDTO = context.Users.FirstOrDefault(x => x.Username == username);
                if (userDTO != null)
                {
                    roles = context.UserRoles.Where(x => x.UserId == userDTO.Id)
                                             .Select(row => row.Role.Name)
                                             .ToArray();
                }
            }

            // Build IPrincipal object
            IIdentity userIdentity = new GenericIdentity(username);
            IPrincipal newUserObj = new System.Security.Principal.GenericPrincipal(userIdentity, roles);

            // Update the Context.User with our IPrincipal
            Context.User = newUserObj;
        }

    }
}
