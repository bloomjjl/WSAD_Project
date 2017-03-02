using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSAD_Project.Areas.Admin.Models.ViewModels.ManageUser;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Areas.Admin.Controllers
{
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
                var dbUsers = context.Users;

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
        [Authorize]
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
    }
}