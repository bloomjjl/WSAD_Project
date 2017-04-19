using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WSAD_Project.Models.ViewModels.SessionSearch
{
    [DataContract]
    public class SessionSearchViewModel
    {
        public SessionSearchViewModel() { }

        public SessionSearchViewModel(Data.Session sessionDTO)
        {
            SessionId = sessionDTO.Id;
            SessionTitle = sessionDTO.Title;
        }

        [DataMember]
        public int SessionId { get; set; }

        [DataMember]
        public string SessionTitle { get; set; }

    }
}