using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSAD_Project.Areas.Admin.Models.ViewModels.ManageUser;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersController : Controller
    {
        // GET: ManageUsers
        public ActionResult Index()
        {
            // setup a DbContext
            List<ManageUserViewModel> collectionOfUserVM = new List<ManageUserViewModel>();

            using (WSADDbContext context = new WSADDbContext())
            {
                // Get All users
                List<WSAD_Project.Models.Data.User> dbUsers = context.Users.ToList();

                // Move users into a ViewModel object
                foreach (var userDTO in dbUsers)
                {
                    collectionOfUserVM.Add(
                        new ManageUserViewModel(userDTO)
                    );
                }
            }

            // send ViewModel collection to view
            return View(collectionOfUserVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<ManageUserViewModel> collectionOfUsers)
        {
            // Filter collectionOfUsers to Find the Selected Items only
            var vmItemsToDelete = collectionOfUsers.Where(x => x.IsSelected == true);

            // Do the Delete
            using (WSADDbContext context = new WSADDbContext())
            {
                // Loop through ViewModel Items to Delete
                foreach (var vmItems in vmItemsToDelete)
                {
                    var dtoToDelete = context.Users.FirstOrDefault(row => row.Id == vmItems.Id);
                    context.Users.Remove(dtoToDelete);
                }

                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateManageUserViewModel model)
        {
            // Validate the New User
            // Validate Required fields (no empties)
            // FirstName, LastName, UserName, Email, Password
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate Password vs PasswordConfirm match
            if (!model.Password.Equals(model.PasswordConfirm))
            {
                ModelState.AddModelError("", "Password and Confirm Password must match");
                return View(model);
            }

            // Create instance of our DbContext
            using (WSADDbContext context = new WSADDbContext())
            {
                // make sure username is unique (not already taken)
                if (context.Users.Any(row => row.Username.Equals(model.UserName)))
                {
                    ModelState.AddModelError("", "Username '" + model.UserName + "' already exists. Try again.");
                    model.UserName = "";
                    return View(model);
                }

                // Create user DTO
                User newUserDTO = new User()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    IsActive = true,
                    IsAdmin = false,
                    IsPresenter = model.IsPresenter,
                    Username = model.UserName,
                    Password = model.Password,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Gender = model.Gender
                };

                // add to DbContext
                newUserDTO = context.Users.Add(newUserDTO);

                // Save user data to database
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Edit(int? id = 0)
        {
            int intUserId = ValidateAndGetNullableIntegerAsInteger(id);

            // setup a DbContext
            EditManageUserViewModel userVM;

            using (WSADDbContext context = new WSADDbContext())
            {
                // Get All users
                WSAD_Project.Models.Data.User userDTO = context.Users.FirstOrDefault(x => x.Id == intUserId);

                // Move user into a ViewModel object
                if (userDTO == null)
                {
                    userVM = new EditManageUserViewModel();
                }
                else
                {
                    userVM = new EditManageUserViewModel(userDTO);
                }
            }

            return View(userVM);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditManageUserViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }

            // Do the Delete
            using (WSADDbContext context = new WSADDbContext())
            {
                WSAD_Project.Models.Data.User userUpdateDTO = context.Users.FirstOrDefault(x => x.Id == model.Id);

                // transfer changes to DTO
                userUpdateDTO.FirstName = model.FirstName;
                userUpdateDTO.LastName = model.LastName;
                userUpdateDTO.EmailAddress = model.EmailAddress;
                userUpdateDTO.Username = model.Username;
                userUpdateDTO.IsActive = model.IsActive;
                userUpdateDTO.IsAdmin = model.IsAdmin;
                userUpdateDTO.DateModified = DateTime.Today;
                userUpdateDTO.IsPresenter = model.IsPresenter;

                if (userUpdateDTO != null)
                {
                    context.SaveChanges();
                }

                return RedirectToAction("Index");
            }
        }



        public int ValidateAndGetNullableIntegerAsInteger(int? id)
        {
            int intId = 0;
            if (int.TryParse(id.ToString(), out intId))
            {
                intId = int.Parse(id.ToString());
            }

            return intId;
        }



        public ActionResult AddSessionToUser(int? userId, int? sessionId)
        {
            // validate parameters
            int intUserId = ValidateAndGetNullableIntegerAsInteger(userId);
            int intSessionId = ValidateAndGetNullableIntegerAsInteger(sessionId);

            if (intUserId <= 0 || intSessionId <= 0)
            { return this.HttpNotFound("Invalid Input Parameters"); }

            using (WSADDbContext context = new WSADDbContext())
            {
                // get Session & User
                Session sessinoDTO = context.Sessions.FirstOrDefault(x => x.Id == intSessionId);
                User userDTO = context.Users.FirstOrDefault(x => x.Id == intUserId);

                // verify Session & User
                if (sessinoDTO == null) { return this.HttpNotFound("Invalid Input Paramenters"); }
                if (userDTO == null) { return this.HttpNotFound("Invalid Input Paramenters"); }

                // check for existing user for session
                UserSession userSessionDTO = context.UserSessions
                    .Where(x => x.UserId == intUserId)
                    .Where(x => x.SessionId == intSessionId)
                    .FirstOrDefault();

                if (userSessionDTO == null)
                {
                    // build new user session
                    UserSession newUserSession = new UserSession()
                    {
                        SessionId = intSessionId,
                        UserId = intUserId,
                        CreateDate = DateTime.Now
                    };

                    context.UserSessions.Add(newUserSession);

                    // save changes
                    context.SaveChanges();
                }
            }

            // update view
            return RedirectToAction("SessionListByUser", new { userId = intUserId });
        }



        public ActionResult AddSessionToPresenter(int? userId, int? sessionId)
        {
            // validate parameters
            int intUserId = ValidateAndGetNullableIntegerAsInteger(userId);
            int intSessionId = ValidateAndGetNullableIntegerAsInteger(sessionId);

            if (intUserId <= 0 || intSessionId <= 0)
            { return this.HttpNotFound("Invalid Input Parameters"); }

            using (WSADDbContext context = new WSADDbContext())
            {
                // get Session & Presenter
                Session sessinoDTO = context.Sessions.FirstOrDefault(x => x.Id == intSessionId);
                User presenterDTO = context.Users.FirstOrDefault(x => x.Id == intUserId);

                // verify Session & Presenter
                if (sessinoDTO == null) { return this.HttpNotFound("Invalid Input Paramenters"); }
                if (presenterDTO == null) { return this.HttpNotFound("Invalid Input Paramenters"); }

                // check for existing Presenter for session
                PresenterSession presenterSessionDTO = context.PresenterSessions
                    .Where(x => x.UserId == intUserId)
                    .Where(x => x.SessionId == intSessionId)
                    .FirstOrDefault();

                if (presenterSessionDTO == null)
                {
                    // build new user session
                    PresenterSession newPresenterSession = new PresenterSession()
                    {
                        SessionId = intSessionId,
                        UserId = intUserId,
                        CreateDate = DateTime.Now
                    };

                    context.PresenterSessions.Add(newPresenterSession);

                    // save changes
                    context.SaveChanges();
                }
            }

            // update view
            return RedirectToAction("SessionListByPresenter", new { userId = intUserId });
        }



        public ActionResult SessionListByUser(int? userId)
        {
            // validate parameters
            int intUserId = ValidateAndGetNullableIntegerAsInteger(userId);
            if (intUserId <= 0)
            {
                return this.HttpNotFound("Invalid Input Parameters");
            }

            SessionListByUserViewModel userSessionsVM;
            List<UserSession> dbUserSessions;

            // get list of sessions for current user
            using (WSADDbContext context = new WSADDbContext())
            {
                // store user information
                User userDTO = context.Users.FirstOrDefault(x => x.Id == intUserId);
                if (userDTO == null) { return RedirectToAction("Index"); }
                userSessionsVM = new SessionListByUserViewModel(userDTO);

                // get list of users
                userSessionsVM.UserSessionItems = new List<UserSessionItem>();
                dbUserSessions = context.UserSessions
                    .Include("Session")
                    .Where(row => row.UserId == intUserId)
                    .ToList();

                // convert to viewModel
                if (dbUserSessions != null && dbUserSessions.Count > 0)
                {
                    foreach (UserSession userSessionDTO in dbUserSessions)
                    {
                        userSessionsVM.UserSessionItems.Add(new UserSessionItem()
                        {
                            SessionId = userSessionDTO.SessionId,
                            SessionTitle = userSessionDTO.Session.Title,
                            DateRegistered = userSessionDTO.CreateDate
                        });
                    }

                }

                return View(userSessionsVM);
            }
        }




        public ActionResult SessionListByPresenter(int? userId)
        {
            // validate parameters
            int intUserId = ValidateAndGetNullableIntegerAsInteger(userId);
            if (intUserId <= 0)
            {
                return this.HttpNotFound("Invalid Input Parameters");
            }

            SessionListByPresenterViewModel presenterSessionsVM;
            List<PresenterSession> dbPresenterSessions;

            // get list of sessions for current user
            using (WSADDbContext context = new WSADDbContext())
            {
                // store user information
                User presenterDTO = context.Users.FirstOrDefault(x => x.Id == intUserId);
                if (presenterDTO == null) { return RedirectToAction("Index"); }
                presenterSessionsVM = new SessionListByPresenterViewModel(presenterDTO);

                // get list of users
                presenterSessionsVM.PresenterSessionItems = new List<PresenterSessionItem>();
                dbPresenterSessions = context.PresenterSessions
                    .Include("Session")
                    .Where(row => row.UserId == intUserId)
                    .ToList();

                // convert to viewModel
                if (dbPresenterSessions != null && dbPresenterSessions.Count > 0)
                {
                    foreach (var presenterSessionDTO in dbPresenterSessions)
                    {
                        presenterSessionsVM.PresenterSessionItems.Add(new PresenterSessionItem()
                        {
                            SessionId = presenterSessionDTO.SessionId,
                            SessionTitle = presenterSessionDTO.Session.Title,
                            DateRegistered = presenterSessionDTO.CreateDate
                        });
                    }

                }

                return View(presenterSessionsVM);
            }
        }




        public ActionResult RemoveSessionFromUser(int? userId, int? sessionId)
        {
            // validate parameters
            int intUserId = ValidateAndGetNullableIntegerAsInteger(userId);
            int intSessionId = ValidateAndGetNullableIntegerAsInteger(sessionId);

            // remove user from session
            using (WSADDbContext context = new WSADDbContext())
            {
                // get user session
                UserSession sessionUserDTO = context.UserSessions
                    .Where(x => x.UserId == intUserId)
                    .Where(x => x.SessionId == intSessionId)
                    .FirstOrDefault();

                // remove user from session
                context.UserSessions.Remove(sessionUserDTO);

                // update database
                context.SaveChanges();
            }

            // update view to user
            return RedirectToAction("SessionListByUser", new { userId = intUserId });
        }



        public ActionResult RemoveSessionFromPresenter(int? userId, int? sessionId)
        {
            // validate parameters
            int intUserId = ValidateAndGetNullableIntegerAsInteger(userId);
            int intSessionId = ValidateAndGetNullableIntegerAsInteger(sessionId);

            using (WSADDbContext context = new WSADDbContext())
            {
                // get presenter session
                PresenterSession sessionPresenterDTO = context.PresenterSessions
                    .Where(x => x.UserId == intUserId)
                    .Where(x => x.SessionId == intSessionId)
                    .FirstOrDefault();

                // remove presenter from session
                context.PresenterSessions.Remove(sessionPresenterDTO);

                // update database
                context.SaveChanges();
            }

            // update view to user
            return RedirectToAction("SessionListByPresenter", new { userId = intUserId });
        }



    }
}