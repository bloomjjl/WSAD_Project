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
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart
        public ActionResult Index()
        {
            List<ShoppingCartViewModel> shoppingCartItems;

            using (WSADDbContext context = new WSADDbContext())
            {
                // Get user info
                string userName = User.Identity.Name;

                // Get userId from database
                int userId = context.Users
                    .Where(x => x.Username == userName)
                    .Select(x => x.Id)
                    .FirstOrDefault();

                // Get shopping cart items
                // generate shopping cart viewmodel
                shoppingCartItems = context.ShoppingCarts
                    .Where(x => x.UserId == userId)
                    .ToArray()
                    .Select(x => new ShoppingCartViewModel(x))
                    .ToList();
            }

            return View(shoppingCartItems);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSessionListToOrder(List<SessionViewModel> sessionsToAdd)
        {
            // verify sessions to add is not null
            if (sessionsToAdd == null) { return RedirectToAction("Index", "Session"); }

            // capture sessions to add (filter by isSelected)
            sessionsToAdd = sessionsToAdd.Where(x => x.IsSelected == true).ToList();

            // if no products to Add, redirect to ShoppingCart index
           if (sessionsToAdd.Count <= 0)
            {
                TempData["RegisterSessionMessage"] = "NO SESSIONS SELECTED";
                return RedirectToAction("Index", "Session");
            }

            // Get user from User.Identity.Name
            string userName = User.Identity.Name;

            using (WSADDbContext context = new WSADDbContext())
            {
                // Get user from database (userId)
                int userId = context.Users
                    .Where(x => x.Username == userName)
                    .Select(x => x.Id)
                    .FirstOrDefault();

                foreach (SessionViewModel sessionVM in sessionsToAdd)
                {
                    // make sure not to duplicate sessions in shopping cart
                    if (context.ShoppingCarts.Any(row =>
                        row.UserId == userId && row.SessionId == sessionVM.Id))
                    {
                        // STOP!! Move to next index
                    }
                    else
                    {
                        // Create ShoppingCart DTO
                        ShoppingCart shoppingCartDTO = new ShoppingCart()
                        {
                            // Add the SessionId, UserId, Quantity to the DTO
                            UserId = userId,
                            SessionId = sessionVM.Id
                        };

                        // Add DTO to DbContext
                        context.ShoppingCarts.Add(shoppingCartDTO);
                    }
                }

                // Save DbContext
                context.SaveChanges();
            }

            // Redirect to ShoppingCart Index
            return RedirectToAction("Index");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSessionToOrder(SessionDetailsViewModel sessionToAdd)
        {
            // verify session to add is not null
            if (sessionToAdd == null) { return RedirectToAction("Index"); }

            // Capture logged in User
            AccountController account = new AccountController();
            int userId = account.GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            using (WSADDbContext context = new WSADDbContext())
            {
                // make sure not to duplicate session in shopping cart
                if (context.ShoppingCarts.Any(row =>
                    row.UserId == userId && row.SessionId == sessionToAdd.Id))
                {
                    // STOP!!
                }
                else
                {
                    // Create ShoppingCart DTO
                    ShoppingCart shoppingCartDTO = new ShoppingCart()
                    {
                        // Add the SessionId, UserId, Quantity to the DTO
                        UserId = userId,
                        SessionId = sessionToAdd.Id
                    };

                    // Add DTO to DbContext
                    context.ShoppingCarts.Add(shoppingCartDTO);

                    // Save DbContext
                    context.SaveChanges();
                }
            }

            // Redirect to ShoppingCart Index
            return RedirectToAction("Index");
        }



        public ActionResult AddSelectedSessionToOrder(int? sessionId)
        {
            // verify session provided
            int intSessionId = ValidateAndReturnInteger(sessionId);
            if (intSessionId == 0) { return RedirectToAction("Index"); }

            // Capture logged in User
            AccountController account = new AccountController();
            int userId = account.GetUserIdForUsernameFromDatabase(this.User.Identity.Name);

            using (WSADDbContext context = new WSADDbContext())
            {
                // make sure not to duplicate session in shopping cart
                if (context.ShoppingCarts.Any(row =>
                    row.UserId == userId && row.SessionId == intSessionId))
                {
                    // STOP!!
                }
                else
                {
                    // Create ShoppingCart DTO
                    ShoppingCart shoppingCartDTO = new ShoppingCart()
                    {
                        // Add the SessionId, UserId, Quantity to the DTO
                        UserId = userId,
                        SessionId = intSessionId
                    };

                    // Add DTO to DbContext
                    context.ShoppingCarts.Add(shoppingCartDTO);

                    // Save DbContext
                    context.SaveChanges();
                }
            }

            // Redirect to ShoppingCart Index
            return RedirectToAction("Index");
        }



        public int ValidateAndReturnInteger(int? id)
        {
            int intId;

            if (id == null)
            {// no value provided
                // default to zero.
                return 0;
            }
            else if (int.TryParse(id.ToString(), out intId))
            {// value is an integer
                // return supplied integer
                return int.Parse(id.ToString());
            }
            else
            {// value provided is not an integer
                // default to zero
                return 0;
            }
        }



        public ActionResult Delete(int? shoppingCartId)
        {
            // validate parameter
            if (shoppingCartId == null) { return RedirectToAction("Index"); }
            if (shoppingCartId <= 0) { return RedirectToAction("Index"); }

            int intShoppingCartId = 0;
            if (int.TryParse(shoppingCartId.ToString(), out intShoppingCartId))
            {
                intShoppingCartId = int.Parse(shoppingCartId.ToString());
            }
            else
            {
                return RedirectToAction("Index");
            }

            using (WSADDbContext context = new WSADDbContext())
            {
                // make sure user/session exists in database
                ShoppingCart shoppintCartDTO = context.ShoppingCarts.FirstOrDefault(x => x.Id == intShoppingCartId);

                if(shoppintCartDTO == null) { return RedirectToAction("Index"); }

                // remove user/session from database
                context.ShoppingCarts.Remove(shoppintCartDTO);

                // save changes
                context.SaveChanges();
            }

            // redirect to shoppingcart
            return RedirectToAction("Index");
        }

    }
}