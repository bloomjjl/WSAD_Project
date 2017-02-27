using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSAD_Project.Models.Data;
using WSAD_Project.Models.ViewModels.ManageSession;

namespace WSAD_Project.Controllers
{
    public class ManageSessionsController : Controller
    {
        // GET: Session
        public ActionResult Index()
        {
            // create list of current sessions
            List<ManageSessionViewModel> collectionOfSessionVM = GetListOfAvailableSessionsFromDatabase();

            // send ViewModel collection to view
            return View(collectionOfSessionVM);
        }



        public List<ManageSessionViewModel> GetListOfAvailableSessionsFromDatabase()
        {
            try
            {
                // setup a DbContext
                List<ManageSessionViewModel> collectionOfSessionVM = new List<ManageSessionViewModel>();

                using (WSADDbContext context = new WSADDbContext())
                {
                    // get all sessions
                    var dbSessions = context.Sessions;

                    // move sessions to a ViewModel object
                    foreach (var sessionDTO in dbSessions)
                    {
                        collectionOfSessionVM.Add(
                            new ManageSessionViewModel(sessionDTO)
                            );
                    }
                }

                return collectionOfSessionVM;
            }
            catch
            {
                return new List<ManageSessionViewModel>();
            }
        }









        [HttpGet]
        public ActionResult Registered()
        {
            // make sure user is logged in
            int userId = GetUserIdForUsernameFromDatabase(this.User.Identity.Name);
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // get list of sessions user is signed up for
            List<SessionRegisteredViewModel> collectionUserSessionVM = GetListOfRegisteredUserSessionsFromDatabase(userId);

            return View(collectionUserSessionVM);
        }



        [HttpPost]
        [Authorize]
        public ActionResult RemoveSession(List<SessionRegisteredViewModel> collectionsOfSessions)
        {
            // filter collectionOfSessions to find the Selected Items only
            var vmSessionsSelected = collectionsOfSessions.Where(x => x.IsSelected == true).ToList();

            // Capture logged in User
            int userId = GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            // update database with selected items
            bool sessionUpdated = UpdateSessionsUserCanceledInDatabase(vmSessionsSelected, userId);

            return Redirect("Index");
        }



        public List<SessionSignUpViewModel> GetListOfSessionsUserNotSignedUpForFromDatabase(int userId)
        {
            try
            {
                // setup a DbContext
                List<SessionSignUpViewModel> collectionOfUserSessionVM = new List<SessionSignUpViewModel>();

                using (WSADDbContext context = new WSADDbContext())
                {
                    // get all sessions
                    var dbSession = context.Sessions;

                    // get sessions user signed up for
                    var dbUserSessions = context.UserSessions.Where(x => x.UserId == userId).ToList();

                    // move sessions to a ViewModel object
                    foreach (var sessionDTO in dbSession)
                    {
                        var userSessionDTO = dbUserSessions.FirstOrDefault(row => row.SessionId == sessionDTO.Id);

                        if (userSessionDTO == null)
                        {
                            collectionOfUserSessionVM.Add(
                                new SessionSignUpViewModel(sessionDTO)
                                );
                        }
                    }
                }

                return collectionOfUserSessionVM;
            }
            catch
            {
                return new List<SessionSignUpViewModel>();
            }
        }



        public List<SessionRegisteredViewModel> GetListOfRegisteredUserSessionsFromDatabase(int userId)
        {
            try
            {
                // setup a DbContext
                List<SessionRegisteredViewModel> collectionOfUserSessionVM = new List<SessionRegisteredViewModel>();

                using (WSADDbContext context = new WSADDbContext())
                {
                    // get all sessions user signed up for
                    var dbUserSessions = context.UserSessions.Where(x => x.UserId == userId).ToList();

                    // move sessions to a ViewModel object
                    foreach (var userSessionDTO in dbUserSessions)
                    {
                        var sessionDTO = context.Sessions.Find(userSessionDTO.SessionId);

                        collectionOfUserSessionVM.Add(
                            new SessionRegisteredViewModel(sessionDTO)
                            );
                    }
                }

                return collectionOfUserSessionVM;
            }
            catch
            {
                return new List<SessionRegisteredViewModel>();
            }
        }




        [HttpGet]
        [Authorize]
        public ActionResult SignUp()
        {
            // make sure user is logged in
            int userId = GetUserIdForUsernameFromDatabase(this.User.Identity.Name);
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // create list of current sessions user not signed up for
            List<SessionSignUpViewModel> collectionOfSessionVM = GetListOfSessionsUserNotSignedUpForFromDatabase(userId);

            return View(collectionOfSessionVM);
        }



        [HttpPost]
        [Authorize]
        public ActionResult SignUp(List<SessionSignUpViewModel> collectionsOfSessions)
        {
            // filter collectionOfSessions to find the Selected Items only
            var vmSessionsSelected = collectionsOfSessions.Where(x => x.IsSelected == true).ToList();

            // Capture logged in User
            int userId = GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            // update database with selected items
            bool sessionUpdated = UpdateSessionsUserSignedUpForInDatabase(vmSessionsSelected, userId);

            return Redirect("Index");
        }



        private int GetUserIdForUsernameFromDatabase(string username)
        {
            try
            {
                using (WSADDbContext context = new WSADDbContext())
                {
                    // Search for user
                    Models.Data.User userDTO = context.Users.FirstOrDefault(x => x.Username == username);

                    if (userDTO == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return userDTO.Id;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }



        private bool UpdateSessionsUserSignedUpForInDatabase(IEnumerable<SessionSignUpViewModel> vmSessionsSelected, int userId)
        {
            bool sessionsUpdated = false;

            using (WSADDbContext context = new WSADDbContext())
            {
                // make sure user found in database
                foreach (var vmSession in vmSessionsSelected)
                {
                    // update database with selected items
                    int sessionId = vmSession.Id;
                    var dtoUserSession = context.UserSessions.Where(x => x.UserId == userId).FirstOrDefault(x => x.SessionId == vmSession.Id);

                    // make sure user is not already signed up for session
                    if (dtoUserSession == null)
                    {
                        context.UserSessions.Add(new UserSession
                        {
                            UserId = userId,
                            SessionId = sessionId
                        });
                    }

                }

                context.SaveChanges();
            }

            // 
            return sessionsUpdated;
        }



        private bool UpdateSessionsUserCanceledInDatabase(IEnumerable<SessionRegisteredViewModel> vmSessionsSelected, int userId)
        {
            bool sessionsUpdated = false;

            using (WSADDbContext context = new WSADDbContext())
            {
                // make sure user found in database
                foreach (var vmSession in vmSessionsSelected)
                {
                    // update database with selected items
                    int sessionId = vmSession.Id;
                    var dtoUserSessionToDelete = context.UserSessions.Where(x => x.UserId == userId).FirstOrDefault(x => x.SessionId == vmSession.Id);

                    // make sure user was signed up for session
                    if (dtoUserSessionToDelete != null)
                    {
                        context.UserSessions.Remove(dtoUserSessionToDelete);
                    }

                }

                context.SaveChanges();
            }

            // 
            return sessionsUpdated;
        }
    }
}
