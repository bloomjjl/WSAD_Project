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
        public ActionResult Edit (int? id = 0)
        {
            int intUserId = GetUserIdAsInteger(id);

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
        public ActionResult Edit (EditManageUserViewModel model)
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



        public int GetUserIdAsInteger(int? id)
        {
            int intId = 0;
            if(int.TryParse(id.ToString(), out intId))
            {
                intId = int.Parse(id.ToString());
            }

            return intId;
        }
    }
}