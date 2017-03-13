using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSAD_Project.Models.Data;
using WSAD_Project.Models.ViewModels.Session;

namespace WSAD_Project.Controllers
{
    public class SessionController : Controller
    {
        // GET: Session
        public ActionResult Index()
        {
            // create list of current sessions
            List<SessionViewModel> collectionOfSessionVM = GetListOfAvailableSessionsFromDatabase();

            // update list of available seats for each session
            collectionOfSessionVM = UpdateAvailableSeatsForListOfSessionsFromDatabase(collectionOfSessionVM);

            // send ViewModel collection to view
            return View(collectionOfSessionVM);
        }



        public ActionResult Details(SessionViewModel sessionVM)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            // make sure session selected to display
            if(sessionVM.Id == 0)
            {
                return RedirectToAction("Index");
            }

            // get session information from database
            using (WSADDbContext context = new WSADDbContext())
            {
                Models.Data.Session sessionDTO = context.Sessions.FirstOrDefault(x => x.Id == sessionVM.Id);

                if (sessionDTO == null) { return Redirect("Index"); }

                int remainingSeats = GetAvailableSeatsForSessionFromDatabase(sessionDTO);

                return View(new SessionDetailsViewModel(sessionDTO, remainingSeats));
            }
        }



        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Details(SessionDetailsViewModel sessionVM)
        {
            // stop if no sessions has been selected
            if (sessionVM.Id == 0)
            {
                TempData["RegisterSessionMessage"] = "";
                return Redirect("Index");
            }

            // Capture logged in User
            int userId = GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            // update database with selected items
            if (UpdateSingleSessionUserRegisteredForInDatabase(sessionVM, userId))
            {
                TempData["RegisterSessionMessage"] = "SUCCESS";
            }
            else
            {
                TempData["RegisterSessionMessage"] = "PROBLEM";
            }

            return Redirect("Index");
        }



        public List<SessionViewModel> GetListOfAvailableSessionsFromDatabase()
        {
            // setup a DbContext
            List<SessionViewModel> collectionOfSessionVM = new List<SessionViewModel>();

            using (WSADDbContext context = new WSADDbContext())
            {
                // get all sessions
                var dbSessions = context.Sessions;

                // move sessions to a ViewModel object
                foreach (var sessionDTO in dbSessions)
                {
                    collectionOfSessionVM.Add(
                        new SessionViewModel(sessionDTO)
                        );
                }
            }

            return collectionOfSessionVM;
        }



        public int GetAvailableSeatsForSessionFromDatabase(Models.Data.Session sessionDTO)
        {
            using (WSADDbContext context = new WSADDbContext())
            {
                List<Models.Data.UserSession> dbSessions = context.UserSessions.Where(x => x.SessionId == sessionDTO.Id).ToList();

                if(dbSessions == null) { return 0; }
                
                return sessionDTO.Occupancy - dbSessions.Count();
            }
        }



        public List<SessionViewModel> UpdateAvailableSeatsForListOfSessionsFromDatabase(List<SessionViewModel> collectionOfSessionVM)
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
        [ValidateAntiForgeryToken]
        public ActionResult RemoveSingleSession(ScheduledSessionDetailsViewModel sessionVM)
        {
            // stop if no sessions have been selected
            if (sessionVM.Id == 0)
            {
                TempData["RemoveSessionMessage"] = "";
                return Redirect("Schedule");
            }

            // Capture logged in User
            int userId = GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            // update database with selected items
            if (UpdateSingleSessionUserCanceledInDatabase(sessionVM, userId))
            {
                TempData["RemoveSessionMessage"] = "SUCCESS";
            }
            else
            {
                TempData["RemoveSessionMessage"] = "PROBLEM";
            }

            return Redirect("Schedule");
        }



        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveSessions(List<SessionScheduleViewModel> collectionsOfSessions)
        {
            // filter collectionOfSessions to find the Selected Items only
            var vmSessionsSelected = collectionsOfSessions.Where(x => x.IsSelected == true).ToList();

            // stop if no sessions have been selected
            if (vmSessionsSelected.Count == 0)
            {
                TempData["RemoveSessionMessage"] = "";
                return Redirect("Schedule");
            }

            // Capture logged in User
            int userId = GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            // update database with selected items
            if (UpdateSessionsUserCanceledInDatabase(vmSessionsSelected, userId))
            {
                TempData["RemoveSessionMessage"] = "SUCCESS";
            }
            else
            {
                TempData["RemoveSessionMessage"] = "PROBLEM";
            }

            return Redirect("Schedule");
        }



        public ActionResult ScheduledSessionDetails(SessionScheduleViewModel sessionVM)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Schedule");
            }

            // make sure session selected to display
            if (sessionVM.Id == 0)
            {
                return RedirectToAction("Schedule");
            }

            // get session information from database
            using (WSADDbContext context = new WSADDbContext())
            {
                Models.Data.Session sessionDTO = context.Sessions.FirstOrDefault(x => x.Id == sessionVM.Id);

                if (sessionDTO == null) { return Redirect("Schedule"); }

                int remainingSeats = GetAvailableSeatsForSessionFromDatabase(sessionDTO);

                return View(new ScheduledSessionDetailsViewModel(sessionDTO, remainingSeats));
            }
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
        [ValidateAntiForgeryToken]
        public ActionResult Registration(List<SessionViewModel> collectionsOfSessions)
        {
            // filter collectionOfSessions to find the Selected Items only
            var vmSessionsSelected = collectionsOfSessions.Where(x => x.IsSelected == true).ToList();

            // stop if no sessions have been selected
            if (vmSessionsSelected.Count == 0)
            {
                TempData["RegisterSessionMessage"] = "";
                return Redirect("Index");
            }

            // Capture logged in User
            int userId = GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            // update database with selected items
            if (UpdateSessionsUserRegisteredForInDatabase(vmSessionsSelected, userId))
            {
                TempData["RegisterSessionMessage"] = "SUCCESS";
            }
            else
            {
                TempData["RegisterSessionMessage"] = "PROBLEM";
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



        private bool UpdateSingleSessionUserRegisteredForInDatabase(SessionDetailsViewModel sessionVM, int userId)
        {
            bool sessionsUpdated = false;

            using (WSADDbContext context = new WSADDbContext())
            {
                // make sure user found in database
                int sessionId = sessionVM.Id;
                var dtoUserSession = context.UserSessions.Where(x => x.UserId == userId).FirstOrDefault(x => x.SessionId == sessionVM.Id);

                // make sure user is not already signed up for session
                if (dtoUserSession == null)
                {
                    // update database with selected items
                    context.UserSessions.Add(new UserSession
                    {
                        UserId = userId,
                        SessionId = sessionId
                    });
                }

                context.SaveChanges();
                sessionsUpdated = true;
            }

            // 
            return sessionsUpdated;
        }



        private bool UpdateSessionsUserRegisteredForInDatabase(IEnumerable<SessionViewModel> vmSessionsSelected, int userId)
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



        private bool UpdateSingleSessionUserCanceledInDatabase(ScheduledSessionDetailsViewModel sessionVM, int userId)
        {
            bool sessionsUpdated = false;

            using (WSADDbContext context = new WSADDbContext())
            {
                // make sure user found in database
                int sessionId = sessionVM.Id;
                var dtoUserSessionToDelete = context.UserSessions.Where(x => x.UserId == userId).FirstOrDefault(x => x.SessionId == sessionVM.Id);

                // make sure user was signed up for session
                if (dtoUserSessionToDelete != null)
                {
                    context.UserSessions.Remove(dtoUserSessionToDelete);
                }

                // update database with selected items

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