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
        public ActionResult Edit(int? id = 0)
        {
            int intSessionId = GetUserIdAsInteger(id);

            // setup a DbContext
            EditManageSessionViewModel sessionVM;

            using (WSADDbContext context = new WSADDbContext())
            {
                // Get All sessions
                WSAD_Project.Models.Data.Session sessionDTO = context.Sessions.FirstOrDefault(x => x.Id == intSessionId);

                // Move user into a ViewModel object
                if (sessionDTO == null)
                {
                    sessionVM = new EditManageSessionViewModel();
                }
                else
                {
                    sessionVM = new EditManageSessionViewModel(sessionDTO);
                }

                return View(sessionVM);
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



        public int GetUserIdAsInteger(int? id)
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




    }
}
