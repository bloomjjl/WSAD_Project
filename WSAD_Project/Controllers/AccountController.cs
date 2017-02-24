using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WSAD_Project.Models.Data;
using WSAD_Project.Models.ViewModels.Account;

namespace WSAD_Project.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Logging users into the website
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// login user to website
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginUserViewModel loginUser)
        {
            // Validate username and password is passed (no empties)
            if (loginUser == null)
            {
                ModelState.AddModelError("", "Login is required");
                return View();
            }

            if (string.IsNullOrWhiteSpace(loginUser.UserName))
            {
                ModelState.AddModelError("", "Username is required");
                return View();
            }

            if (string.IsNullOrWhiteSpace(loginUser.Password))
            {
                ModelState.AddModelError("", "Password is required");
                return View();
            }

            // Open database connection
            bool isValid = false;
            using (WSADDbContext context = new WSADDbContext())
            {
                // Hash password

                // Query for user based on username and password hash
                if (context.Users.Any(
                    row => row.Username.Equals(loginUser.UserName)
                    && row.Password.Equals(loginUser.Password)
                    ))
                {
                    isValid = true;
                }
            }

            // If invalid, send error
            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid Username or Password");
                return View();
            }
            else
            {
                // Valid, redirect to user profile
                System.Web.Security.FormsAuthentication.SetAuthCookie(loginUser.UserName, loginUser.RememberMe);

                return Redirect(FormsAuthentication.GetRedirectUrl(loginUser.UserName, loginUser.RememberMe));
            }
        }
    }
}