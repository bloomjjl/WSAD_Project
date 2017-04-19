using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSAD_Project.Areas.Admin.Models.ViewModels.ManageSession
{
    public class EditManageSessionViewModel
    {
        public EditManageSessionViewModel() { }

        public EditManageSessionViewModel(WSAD_Project.Models.Data.Session sessionDTO, List<WSAD_Project.Models.Data.User> presenterList)
        {
            Id = sessionDTO.Id;
            Title = sessionDTO.Title;
            Description = sessionDTO.Description;
            Address = sessionDTO.Address;
            Room = sessionDTO.Room;
            Time = sessionDTO.Time;
            Occupancy = sessionDTO.Occupancy;
            Presenters = presenterList;
        }


        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Room { get; set; }
        public DateTime Time { get; set; }
        public int Occupancy { get; set; }
        public List<WSAD_Project.Models.Data.User> Presenters { get; set; }
    }

}