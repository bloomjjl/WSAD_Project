using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSAD_Project.Models.Data;
using WSAD_Project.Models.ViewModels.PresesnterSearch;

namespace WSAD_Project.Controllers
{
    public class PresenterSearchController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<PresenterSearchViewModel> Get(string term)
        {
            IQueryable<User> matches;
           List<PresenterSearchViewModel> presenterSearchVM = new List<PresenterSearchViewModel>();

            using (WSADDbContext context = new WSADDbContext())
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    // show all presenters
                    matches = context.Users
                        .Where(x => x.IsPresenter == true)
                        .AsQueryable();
                }
                else
                {
                    // show only presenters matching query
                    matches = context.Users
                        .Where(row => row.Username.StartsWith(term))
                        .Where(x => x.IsPresenter == true);
                }

                // load ViewModel
                foreach (var presenterDTO in matches)
                {
                    presenterSearchVM.Add(new PresenterSearchViewModel(presenterDTO));
                }

                return presenterSearchVM;
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