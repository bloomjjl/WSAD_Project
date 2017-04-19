using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSAD_Project.Models.Data;
using WSAD_Project.Areas.Admin.Models.ViewModels.ManageSession;

namespace WSAD_Project.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageSessionsController : Controller
    {
        // GET: Session
        public ActionResult Index()
        {
            // create list of current sessions
            List<ManageSessionsViewModel> collectionOfSessionVM = GetListOfAvailableSessionsFromDatabase();

            // update list of available seats for each session
            collectionOfSessionVM = UpdateAvailableSeatsForListOfSessionsFromDatabase(collectionOfSessionVM);

            // send ViewModel collection to view
            return View(collectionOfSessionVM);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete (List<ManageSessionsViewModel> collectionOfSessions)
        {
            // Filter collectionOfUsers to Find the Selected Items only
            var vmItemsToDelete = collectionOfSessions.Where(x => x.IsSelected == true);

            // Do the Delete
            using (WSADDbContext context = new WSADDbContext())
            {
                // Loop through ViewModel Items to Delete
                foreach (var vmItems in vmItemsToDelete)
                {
                    var dtoToDelete = context.Sessions.FirstOrDefault(row => row.Id == vmItems.Id);
                    context.Sessions.Remove(dtoToDelete);
                }

                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult Create ()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateManageSessionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Create instance of our DbContext
            using (WSADDbContext context = new WSADDbContext())
            {
                // Create session DTO
                Session newSessionDTO = new Session()
                {
                    Title = model.Title,
                    Description = model.Description,
                    Address = model.Address,
                    Room = model.Room,
                    Time = model.Time,
                    Occupancy = model.Occupancy
                };

                // add to DbContext
                newSessionDTO = context.Sessions.Add(newSessionDTO);

                // Save data to database
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult Edit(int? sessionId = 0)
        {
            int intSessionId = ValidateAndGetNullableIntegerAsInteger(sessionId);

            // Get session information for session ID

            // setup a DbContext
            EditManageSessionViewModel sessionVM;

            using (WSADDbContext context = new WSADDbContext())
            {
                // store information for specified session ID
                WSAD_Project.Models.Data.Session sessionDTO = context.Sessions.FirstOrDefault(x => x.Id == intSessionId);

                // Move user into a ViewModel object
                if (sessionDTO == null)
                {
                    sessionVM = new EditManageSessionViewModel();
                }
                else
                {
                    List<WSAD_Project.Models.Data.User> presenterList = GetListOfPresentersForSessionFromDatabase(sessionDTO);

                    // transfer information to ViewModel
                    sessionVM = new EditManageSessionViewModel(sessionDTO, presenterList);

                    /*
                    // get list of presenters for session
                    int sessionID = sessionDTO.Id;
                    List<WSAD_Project.Models.Data.PresenterSession> presenterSessionsDTO = context.PresenterSessions
                        .Include("User")
                        .Where(x => x.SessionId == sessionDTO.Id)
                        .ToList();

                    if (presenterSessionsDTO != null)
                    {
                        sessionVM.Presenters = new List<Presenter>();

                        for (int i = 0; i < presenterSessionsDTO.Count(); i++)
                        {
                            sessionVM.Presenters.Add(new Presenter()
                            {
                                PresenterUserId = presenterSessionsDTO[i].UserId,
                                PresenterUserName = presenterSessionsDTO[i].User.Username,
                                ToAdd = false,
                                ToRemove = false
                            });
                        }
                    }
                    */
                }

                return View(sessionVM);
            }
        }



        public List<WSAD_Project.Models.Data.User> GetListOfPresentersForSessionFromDatabase(WSAD_Project.Models.Data.Session sessionDTO)
        {
            using (WSADDbContext context = new WSADDbContext())
            {
                // get session presenters from database
                List<WSAD_Project.Models.Data.PresenterSession> dbPresenters = context.PresenterSessions
                    .Include("User")
                    .Where(x => x.SessionId == sessionDTO.Id)
                    .ToList();

                if (dbPresenters == null) { return new List<WSAD_Project.Models.Data.User>(); }

                // store the names of each presenter for session
                List<User> presenters = new List<User>();
                foreach (var presenterDTO in dbPresenters)
                {
                    presenters.Add( new User()
                    {
                        Id = presenterDTO.UserId,
                        FirstName = presenterDTO.User.FirstName,
                        LastName = presenterDTO.User.LastName
                    });
                }

                // return list of presenter names
                return presenters;
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditManageSessionViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }

            // Do the Delete
            using (WSADDbContext context = new WSADDbContext())
            {
                WSAD_Project.Models.Data.Session sessionUpdateDTO = context.Sessions.FirstOrDefault(x => x.Id == model.Id);

                if (sessionUpdateDTO != null)
                {
                    // transfer changes to DTO
                    sessionUpdateDTO.Title = model.Title;
                    sessionUpdateDTO.Description = model.Description;
                    sessionUpdateDTO.Address = model.Address;
                    sessionUpdateDTO.Room = model.Room;
                    sessionUpdateDTO.Time = model.Time;
                    sessionUpdateDTO.Occupancy = model.Occupancy;

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



        public List<ManageSessionsViewModel> GetListOfAvailableSessionsFromDatabase()
        {
            // setup a DbContext
            List<ManageSessionsViewModel> collectionOfSessionVM = new List<ManageSessionsViewModel>();

            using (WSADDbContext context = new WSADDbContext())
            {
                // get all sessions
                var dbSessions = context.Sessions;

                // move sessions to a ViewModel object
                foreach (var sessionDTO in dbSessions)
                {
                    collectionOfSessionVM.Add(
                        new ManageSessionsViewModel(sessionDTO)
                        );
                }
            }

            return collectionOfSessionVM;
        }



        public List<ManageSessionsViewModel> UpdateAvailableSeatsForListOfSessionsFromDatabase(List<ManageSessionsViewModel> collectionOfSessionVM)
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




        public ActionResult AddUserToSession (int? sessionId, int? userId)
        {
            // validate parameters
            int intUserId = ValidateAndGetNullableIntegerAsInteger(userId);
            int intSessionId = ValidateAndGetNullableIntegerAsInteger(sessionId);

            if (intUserId <= 0 || intSessionId <= 0)
                { return this.HttpNotFound("Invalid Input Parameters"); }

            // add user to session
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
                    .Where(x => x.SessionId == intSessionId)
                    .Where(x => x.UserId == intUserId)
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
            return RedirectToAction("UserListBySession", new { sessionId = intSessionId });
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
            return RedirectToAction("SessionListByUser", "ManageSessions", new { userId = intUserId });
        }



        public ActionResult RemoveUserFromSession (int? userId, int? sessionId)
        {
            // validate parameters
            int intUserId = ValidateAndGetNullableIntegerAsInteger(userId);
            int intSessionId = ValidateAndGetNullableIntegerAsInteger(sessionId);

            if (intUserId <= 0 || intSessionId <= 0)
                { return this.HttpNotFound("Invalid Input Parameters"); }

            // remove user from session
            using (WSADDbContext context = new WSADDbContext())
            {
                // get user session
                UserSession userSessionDTO = context.UserSessions
                    .Where(x => x.UserId == intUserId)
                    .Where(x => x.SessionId == intSessionId)
                    .FirstOrDefault();

                // remove user from session
                context.UserSessions.Remove(userSessionDTO);

                // update database
                context.SaveChanges();
            }

            // update view to user
            return RedirectToAction("SessionListByUser", new { userId = intUserId });
        }


        
        public ActionResult RemovePresenterFromSession(int? userId, int? sessionId)
        {
            // validate parameters
            int intUserId = ValidateAndGetNullableIntegerAsInteger(userId);
            int intSessionId = ValidateAndGetNullableIntegerAsInteger(sessionId);

            // remove user from session
/*            using (WSADDbContext context = new WSADDbContext())
            {
                // get presenter session
                WSAD_Project.Models.Data.PresenterSession presenterSessionDTO = context.PresenterSessions
                    .Where(x => x.UserId == intUserId)
                    .Where(x => x.SessionId == intSessionId)
                    .FirstOrDefault();

                // remove presenter from session
                context.PresenterSessions.Remove(presenterSessionDTO);

                // update database
                context.SaveChanges();
            }
*/
            // update view to user
            return RedirectToAction("Edit", new { sessionId = intSessionId });
        }



        public ActionResult UserListBySession(int? sessionId)
        {
            // validate parameters
            int intSessionId = ValidateAndGetNullableIntegerAsInteger(sessionId);
            if (intSessionId <= 0)
            {
                return this.HttpNotFound("Invalid Input Parameters");
            }

            UserListBySessionViewModel sessionUsersVM;
            List<UserSession> dbSessionUsers;
            
            using (WSADDbContext context = new WSADDbContext())
            {
                // store session information
                Session sessionDTO = context.Sessions.FirstOrDefault(x => x.Id == intSessionId);
                if (sessionDTO == null) { return RedirectToAction("Index"); }
                sessionUsersVM = new UserListBySessionViewModel(sessionDTO);

                // get list of sessions
                sessionUsersVM.UserItems = new List<UserItem>();
                dbSessionUsers = context.UserSessions
                    .Include("User")
                    .Where(row => row.SessionId == intSessionId)
                    .ToList();

                // convert to viewModel
                if (dbSessionUsers != null && dbSessionUsers.Count > 0)
                {
                    foreach (var sessionUserDTO in dbSessionUsers)
                    {
                        sessionUsersVM.UserItems.Add(new UserItem()
                        {
                            UserId = sessionUserDTO.UserId,
                            UserName = sessionUserDTO.User.Username,
                            FirstName = sessionUserDTO.User.FirstName,
                            LastName = sessionUserDTO.User.LastName,
                            DateRegistered = sessionUserDTO.CreateDate
                        });
                    }

                    return View(sessionUsersVM);
                }
            }

            return View(sessionUsersVM);
        }




    }
}
