using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSAD_Project.Models.Data;
using WSAD_Project.Models.ViewModels.UserSearch;

namespace WSAD_Project.Controllers
{
    public class UserSearchController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<UserSearchViewModel> Get(string term)
        {
            IQueryable<User> matches;
            List<UserSearchViewModel> userSearchVM = new List<UserSearchViewModel>();

            using (WSADDbContext context = new WSADDbContext())
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    // show all users
                    matches = context.Users.AsQueryable();
                }
                else
                {
                    // show only users matching query
                    matches = context.Users.Where(row => row.Username.StartsWith(term));
                }

                // load ViewModel
                foreach (var userDTO in matches)
                {
                    userSearchVM.Add(new UserSearchViewModel(userDTO));
                }

                return userSearchVM;
            }
        }



        /*
        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
        */
    }
}