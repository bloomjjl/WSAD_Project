using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSAD_Project.Models.Data;
using WSAD_Project.Models.ViewModels.SessionSearch;

namespace WSAD_Project.Controllers
{
    public class SessionSearchController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<SessionSearchViewModel> Get(string term)
        {
            IQueryable<Session> matches;
            List<SessionSearchViewModel> sessionSearchVM = new List<SessionSearchViewModel>();

            using (WSADDbContext context = new WSADDbContext())
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    // show all sessions
                    matches = context.Sessions.AsQueryable();
                }
                else
                {
                    // show only sessions matching query
                    matches = context.Sessions.Where(row => row.Title.StartsWith(term));
                }

                // load ViewModel
                foreach (var sessionDTO in matches)
                {
                    sessionSearchVM.Add(new SessionSearchViewModel(sessionDTO));
                }

                return sessionSearchVM;
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