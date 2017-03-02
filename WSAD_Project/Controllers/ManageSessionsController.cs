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

            // update list of available seats for each session
            collectionOfSessionVM = UpdateAvailableSeatsForEachSessionFromDatabase(collectionOfSessionVM);

            // send ViewModel collection to view
            return View(collectionOfSessionVM);
        }



        public List<ManageSessionViewModel> GetListOfAvailableSessionsFromDatabase()
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



        public List<ManageSessionViewModel> UpdateAvailableSeatsForEachSessionFromDatabase(List<ManageSessionViewModel> collectionOfSessionVM)
        {
            using (WSADDbContext context = new WSADDbContext())
            {
                // get all sessions users have signed up for
                var dbUserSessions = context.UserSessions;

                // update the number of seats available for each session
                for (int i = 0; i < collectionOfSessionVM.Count(); i++)
                {
                    int totalSeats = collectionOfSessionVM[i].Occupancy;
                    int currentSessionId = collectionOfSessionVM[i].Id;
                    int seatsTaken = dbUserSessions.Where(x => x.SessionId == currentSessionId).Count();

                    collectionOfSessionVM[i].RemainingSeats = totalSeats - seatsTaken;
                }
            }

            return collectionOfSessionVM;
        }






        [HttpGet]
        public ActionResult Schedule()
        {
            // make sure user is logged in
            int userId = GetUserIdForUsernameFromDatabase(this.User.Identity.Name);
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // get list of sessions user is signed up for
            List<SessionScheduleViewModel> collectionUserSessionVM = GetListOfScheduledUserSessionsFromDatabase(userId);

            return View(collectionUserSessionVM);
        }



        [HttpPost]
        [Authorize]
        public ActionResult RemoveSession(List<SessionScheduleViewModel> collectionsOfSessions)
        {
            // filter collectionOfSessions to find the Selected Items only
            var vmSessionsSelected = collectionsOfSessions.Where(x => x.IsSelected == true).ToList();

            // stop if no sessions have been selected
            if (vmSessionsSelected.Count == 0)
            {
                TempData["SessionMessage"] = "";
                return Redirect("Index");
            }

            // Capture logged in User
            int userId = GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            // update database with selected items
            if (UpdateSessionsUserCanceledInDatabase(vmSessionsSelected, userId))
            {
                TempData["SessionMessage"] = "SUCCESS";
            }
            else
            {
                TempData["SessionMessage"] = "PROBLEM";
            }

            return Redirect("Index");
        }



        public List<SessionRegistrationViewModel> GetListOfSessionsUserNotSignedUpForFromDatabase(int userId)
        {
            try
            {
                // setup a DbContext
                List<SessionRegistrationViewModel> collectionOfUserSessionVM = new List<SessionRegistrationViewModel>();

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
                                new SessionRegistrationViewModel(sessionDTO)
                                );
                        }
                    }
                }

                return collectionOfUserSessionVM;
            }
            catch
            {
                return new List<SessionRegistrationViewModel>();
            }
        }



        public List<SessionScheduleViewModel> GetListOfScheduledUserSessionsFromDatabase(int userId)
        {
            try
            {
                // setup a DbContext
                List<SessionScheduleViewModel> collectionOfUserSessionVM = new List<SessionScheduleViewModel>();

                using (WSADDbContext context = new WSADDbContext())
                {
                    // get all sessions user signed up for
                    var dbUserSessions = context.UserSessions.Where(x => x.UserId == userId).ToList();

                    // move sessions to a ViewModel object
                    foreach (var userSessionDTO in dbUserSessions)
                    {
                        var sessionDTO = context.Sessions.Find(userSessionDTO.SessionId);

                        collectionOfUserSessionVM.Add(
                            new SessionScheduleViewModel(sessionDTO)
                            );
                    }
                }

                return collectionOfUserSessionVM;
            }
            catch
            {
                return new List<SessionScheduleViewModel>();
            }
        }




        [HttpGet]
        [Authorize]
        public ActionResult Registration()
        {
            // make sure user is logged in
            int userId = GetUserIdForUsernameFromDatabase(this.User.Identity.Name);
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // create list of current sessions user not signed up for
            List<SessionRegistrationViewModel> collectionOfSessionVM = GetListOfSessionsUserNotSignedUpForFromDatabase(userId);

            return View(collectionOfSessionVM);
        }



        [HttpPost]
        [Authorize]
        public ActionResult Registration(List<SessionRegistrationViewModel> collectionsOfSessions)
        {
            // filter collectionOfSessions to find the Selected Items only
            var vmSessionsSelected = collectionsOfSessions.Where(x => x.IsSelected == true).ToList();

            // stop if no sessions have been selected
            if (vmSessionsSelected.Count == 0)
            {
                TempData["SessionMessage"] = "";
                return Redirect("Index");
            }

            // Capture logged in User
            int userId = GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            // update database with selected items
            if (UpdateSessionsUserRegisteredForInDatabase(vmSessionsSelected, userId))
            {
                TempData["SessionMessage"] = "SUCCESS";
            }
            else
            {
                TempData["SessionMessage"] = "PROBLEM";
            }

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



        private bool UpdateSessionsUserRegisteredForInDatabase(IEnumerable<SessionRegistrationViewModel> vmSessionsSelected, int userId)
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
                sessionsUpdated = true;
            }

            // 
            return sessionsUpdated;
        }



        private bool UpdateSessionsUserCanceledInDatabase(IEnumerable<SessionScheduleViewModel> vmSessionsSelected, int userId)
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
                sessionsUpdated = true;
            }

            // 
            return sessionsUpdated;
        }
    }
}
