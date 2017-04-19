using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WSAD_Project.Models.ViewModels.PresesnterSearch
{
    [DataContract]
    public class PresenterSearchViewModel
    {
        public PresenterSearchViewModel() { }

        public PresenterSearchViewModel(Data.User presenterDTO)
        {
            UserId = presenterDTO.Id;
            UserName = presenterDTO.Username;
            FirstName = presenterDTO.FirstName;
            LastName = presenterDTO.LastName;
        }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }
    }
}