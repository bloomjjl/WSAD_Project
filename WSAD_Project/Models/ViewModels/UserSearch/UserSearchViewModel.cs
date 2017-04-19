using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WSAD_Project.Models.ViewModels.UserSearch
{
    [DataContract]
    public class UserSearchViewModel
    {
        public UserSearchViewModel() { }

        public UserSearchViewModel(Data.User userDTO)
        {
            UserId = userDTO.Id;
            UserName = userDTO.Username;
            FirstName = userDTO.FirstName;
            LastName = userDTO.LastName;
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