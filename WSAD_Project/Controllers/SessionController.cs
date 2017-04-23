using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSAD_Project.Models.Data;
using WSAD_Project.Models.ViewModels.Session;
using WSAD_Project.Models.ViewModels.ShoppingCart;

namespace WSAD_Project.Controllers
{
    [Authorize]
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
                List<string> presenterNames = GetListOfPresenterNamesForSessionFromDatabase(sessionDTO);

                return View(new SessionDetailsViewModel(sessionDTO, remainingSeats, presenterNames));
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
            AccountController account = new AccountController();
            int userId = account.GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            // update database with selected items
            if (UpdateSingleSessionUserRegisteredForInDatabase(sessionVM, userId))
            {
                TempData["RegisterSessionMessage"] = "SUCCESS";
            }
            else
            {
                TempData["RegisterSessionMessage"] = "SESSION WAS NOT UPDATED";
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
                IEnumerable<Session> dbSessions = context.Sessions;

                // move sessions to a ViewModel object
                foreach (Session sessionDTO in dbSessions)
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



        public List<string> GetListOfPresenterNamesForSessionFromDatabase(Models.Data.Session sessionDTO)
        {
            using (WSADDbContext context = new WSADDbContext())
            {
                // get session presenters from database
                List<Models.Data.PresenterSession> dbPresenters = context.PresenterSessions.Where(x => x.SessionId == sessionDTO.Id).ToList();

                if (dbPresenters == null) { return new List<string>(); }

                // store the names of each presenter for session
                List<string> presenterNames = new List<string>();
                foreach (var presenterDTO in dbPresenters)
                {
                    presenterNames.Add(presenterDTO.User.FirstName + " " + presenterDTO.User.LastName);
                }

                // return list of presenter names
                return presenterNames;
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
            // Capture logged in User
            AccountController account = new AccountController();
            int userId = account.GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            if (userId == 0)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "~/Session/Schedule" });
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
                TempData["ScheduleSessionMessage"] = "";
                return Redirect("Schedule");
            }

            // Capture logged in User
            AccountController account = new AccountController();
            int userId = account.GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            // update database with selected items
            if (UpdateSingleSessionUserCanceledInDatabase(sessionVM, userId))
            {
                TempData["ScheduleSessionMessage"] = "SUCCESS";
            }
            else
            {
                TempData["ScheduleSessionMessage"] = "SESSION WAS NOT UPDATED";
            }

            return Redirect("Schedule");
        }



        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveSessions(List<SessionScheduleViewModel> collectionsOfSessions)
        {
            // validate parameters
            if (collectionsOfSessions == null) { return RedirectToAction("Index"); }
            // filter collectionOfSessions to find the Selected Items only
            var vmSessionsSelected = collectionsOfSessions.Where(x => x.IsSelected == true).ToList();

            // stop if no sessions have been selected
            if (vmSessionsSelected.Count == 0)
            {
                TempData["ScheduleSessionMessage"] = "NO SESSIONS SELECTED";
                return Redirect("Schedule");
            }

            // Capture logged in User
            AccountController account = new AccountController();
            int userId = account.GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            // update database with selected items
            if (UpdateSessionsUserCanceledInDatabase(vmSessionsSelected, userId))
            {
                TempData["ScheduleSessionMessage"] = "SUCCESS";
            }
            else
            {
                TempData["ScheduleSessionMessage"] = "SESSION WAS NOT UPDATED";
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
                List<string> presenterNames = GetListOfPresenterNamesForSessionFromDatabase(sessionDTO);

                return View(new ScheduledSessionDetailsViewModel(sessionDTO, remainingSeats, presenterNames));
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
                    IEnumerable<Session> dbSession = context.Sessions;

                    // get sessions user signed up for
                    IEnumerable<UserSession> dbUserSessions = context.UserSessions.Where(x => x.UserId == userId).ToList();

                    // move sessions to a ViewModel object
                    foreach (Session sessionDTO in dbSession)
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
                    IEnumerable<UserSession> dbUserSessions = context.UserSessions.Where(x => x.UserId == userId).ToList();

                    // move sessions to a ViewModel object
                    foreach (UserSession userSessionDTO in dbUserSessions)
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
            // Capture logged in User
            AccountController account = new AccountController();
            int userId = account.GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

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
        public ActionResult Registration(List<ShoppingCartViewModel> collectionsOfSessions)
        {
            // validate parameters
            if (collectionsOfSessions == null) { return RedirectToAction("Index"); }

            // filter collectionOfSessions to find the Selected Items only
            var vmSessionsSelected = collectionsOfSessions.Where(x => x.IsSelected == true).ToList();

            // stop if no sessions have been selected
            if (vmSessionsSelected.Count == 0)
            {
                TempData["ShoppingCartMessage"] = "NO SESSIONS SELECTED";
                return RedirectToAction("Index", "ShoppingCart");
            }

            // Capture logged in User
            AccountController account = new AccountController();
            int userId = account.GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            // update database with selected items
            List<string> sessionsNotUpdated = UpdateSessionsUserRegisteredForInDatabase(vmSessionsSelected, userId);
            if (sessionsNotUpdated.Count == 0)
            {
                TempData["ShoppingCartMessage"] = "SUCCESS";
            }
            else
            {
                TempData["ShoppingCartMessage"] = "SOME SESSIONS HAVE ALREADY BEEN REGISTERED AND WERE NOT UPDATED";
            }

            return RedirectToAction("Index", "ShoppingCart");
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



        private List<string> UpdateSessionsUserRegisteredForInDatabase(IEnumerable<ShoppingCartViewModel> vmSessionsSelected, int userId)
        {
            int count = 0;
            List<string> sessionsNotUpdated = new List<string>();

            using (WSADDbContext context = new WSADDbContext())
            {
                // make sure user found in database
                foreach (var vmSession in vmSessionsSelected)
                {
                    // update database with selected items
                    int sessionId = context.ShoppingCarts.Where(x=>x.Id==vmSession.ShoppingCartId).Select(x=>x.SessionId).FirstOrDefault();
                    var dtoUserSession = context.UserSessions.Where(x => x.UserId == userId).FirstOrDefault(x => x.SessionId == sessionId);

                    // make sure user is not already signed up for session
                    if (dtoUserSession == null)
                    {
                        context.UserSessions.Add(new UserSession
                        {
                            UserId = userId,
                            SessionId = sessionId,
                            CreateDate = DateTime.Now
                        });

                        // remove session from shopping cart in database
                        ShoppingCart dtoShoppintCart = context.ShoppingCarts.FirstOrDefault(x => x.Id == vmSession.ShoppingCartId);
                        if (dtoShoppintCart != null) { context.ShoppingCarts.Remove(dtoShoppintCart); }

                        // keep track of number of sessions updated
                        count += 1;
                    }
                    else
                    {
                        sessionsNotUpdated.Add(dtoUserSession.Session.Title);
                    }
                 }

                context.SaveChanges();
            }

            // were any records updated
            if (count > 0)
            {
                return sessionsNotUpdated;
            }

            return sessionsNotUpdated;
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