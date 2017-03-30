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
            try
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
            catch
            {
                return RedirectToAction("Logout");
            }
        }



        /// <summary>
        /// create a new account for application
        /// </summary>
        /// <returns>ViewResult for the Create</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        public ActionResult Create(CreateUserViewModel newUser)
        {
            // Validate the New User
            // Validate Required fields (no empties)
            // FirstName, LastName, UserName, Email, Password
            if (!ModelState.IsValid)
            {
                return View(newUser);
            }

            // Validate Password vs PasswordConfirm match
            if (!newUser.Password.Equals(newUser.PasswordConfirm))
            {
                ModelState.AddModelError("", "Password and Confirm Password must match");
                return View(newUser);
            }

            // Create instance of our DbContext
            using (WSADDbContext context = new WSADDbContext())
            {
                // make sure username is unique (not already taken)
                if (context.Users.Any(row => row.Username.Equals(newUser.UserName)))
                {
                    ModelState.AddModelError("", "Username '" + newUser.UserName + "' already exists. Try again.");
                    newUser.UserName = "";
                    return View(newUser);
                }

                #region Entity Framework example

                #endregion

                // Create user DTO
                User newUserDTO = new Models.Data.User()
                {
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    EmailAddress = newUser.EmailAddress,
                    IsActive = true,
                    IsAdmin = false,
                    Username = newUser.UserName,
                    Password = newUser.Password,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Gender = newUser.Gender
                };

                // add to DbContext
                newUserDTO = context.Users.Add(newUserDTO);

                // Save user data to database
                context.SaveChanges();
            }

            // Redirect to login
            return RedirectToAction("login");
        }



        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }




        public ActionResult UserNavPartial()
        {
            try
            {
            // Capture logged in User
            string username = this.User.Identity.Name;

            // Get User information from database
            UserNavPartialViewModel userNavVM;

            using (WSADDbContext context = new WSADDbContext())
            {
                // Search for user
                Models.Data.User userDTO = context.Users.FirstOrDefault(x => x.Username == username);

                if (userDTO == null) { return Content(""); }

                // Build our UserNavPartialViewModel
                userNavVM = new UserNavPartialViewModel()
                {
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    Id = userDTO.Id
                };
            }

            // Send the view model to the Partial View
            return PartialView(userNavVM);
            }
            catch
            {
                return Content("");
            }
        }



        public ActionResult UserProfile(int? id = null)
        {
            // Capture logged in User
            string username = User.Identity.Name;

            // Retrieve user from database
            UserProfileViewModel profileVM;

            using (WSADDbContext context = new WSADDbContext())
            {
                // Get user from database
                User userDTO;

                if (id.HasValue)
                {
                    userDTO = context.Users.Find(id.Value);
                }
                else
                {
                    userDTO = context.Users.FirstOrDefault(row => row.Username == username);
                }

                if (userDTO == null)
                {
                    return Content("Invalid Username");
                }

                // Populate our UserProfileViewModel
                profileVM = new UserProfileViewModel()
                {
                    Id = userDTO.Id,
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    UserName = userDTO.Username,
                    DateCreated = userDTO.DateCreated,
                    EmailAddress = userDTO.EmailAddress,
                    //Gender = userDTO.Gender,
                    IsAdmin = userDTO.IsAdmin
                };
            }

            // Return View with ViewModel
            return View(profileVM);
        }



        [HttpGet]
        public ActionResult Edit(string id)
        {
            // make sure id is an integer
            int intId = ConvertStringIdToValidIntegerId(id);

            if (intId == 0)
            {
                return Redirect("Login");
            }

            // Get user id
            EditViewModel editVM;

            using (WSADDbContext context = new WSADDbContext())
            {
                // Get user from database
                User userDTO = context.Users.Find(intId);

                if (userDTO == null)
                {
                    return Content("Login");
                }

                // create EditViewModel
                editVM = new EditViewModel()
                {
                    Id = userDTO.Id,
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    UserName = userDTO.Username,
                    EmailAddress = userDTO.EmailAddress,
                    Gender = userDTO.Gender
                };
            }

            // Send the viewModel to the View
            return View(editVM);
        }




        /// <summary>
        /// convert a string value to an integer value
        /// </summary>
        /// <returns>zero if not integer</returns>
        public int ConvertStringIdToValidIntegerId(string strId)
        {
            int intId = 0;

            if(int.TryParse(strId, out intId))
            {
                intId = int.Parse(strId);
            }

            return intId;
        }





        [HttpPost]
        public ActionResult Edit(EditViewModel editVM)
        {
            // Variables
            bool needsPasswordReset = false;
            bool usernameHasChanged = false;

            // Validate Model
            if (!ModelState.IsValid)
            {
                return View(editVM);
            }

            // Check for password change
            if (!string.IsNullOrWhiteSpace(editVM.Password))
            {
                // Compare Password with PasswordConfirm
                if (editVM.Password != editVM.PasswordConfirm)
                {
                    ModelState.AddModelError("", "Password and Password Confirm must match.");
                    return View(editVM);
                }
                else
                {
                    needsPasswordReset = true;
                }
            }

            // Get user from database
            using (WSADDbContext context = new WSADDbContext())
            {
                // Get our DTO
                User userDTO = context.Users.Find(editVM.Id);
                if (userDTO == null) { return Content("Invalid user Id"); }

                // check if username changed
                if (userDTO.Username != editVM.UserName)
                {
                    userDTO.Username = editVM.UserName;
                    usernameHasChanged = true;
                }

                // Set/Update values from ViewModel
                userDTO.FirstName = editVM.FirstName;
                userDTO.LastName = editVM.LastName;
                userDTO.EmailAddress = editVM.EmailAddress;
                userDTO.DateModified = DateTime.Now;

                // check if password needs updated
                if (needsPasswordReset)
                {
                    userDTO.Password = editVM.Password;
                }

                // Save Changes
                context.SaveChanges();
            }

            if (usernameHasChanged || needsPasswordReset)
            {
                // does not persist when Logout redirects to Login page
                //ViewBag.LogoutMessage = "After a username or password change. Please log in with the new credentials.";
                TempData["LogoutMessage"] = "After a username or password change. Please log in with the new credentials.";
                return RedirectToAction("Logout");
            }
            else
            {
                return RedirectToAction("UserProfile");
            }
        }



        public int GetUserIdForUsernameFromDatabase(string username)
        {
            try
            {
                using (WSADDbContext context = new WSADDbContext())
                {
                    // Search for user
                    WSAD_Project.Models.Data.User userDTO = context.Users.FirstOrDefault(x => x.Username == username);
                    if (userDTO == null) { return 0; }
                    else { return userDTO.Id; }
                }
            }
            catch
            {
                return 0;
            }
        }


    }
}