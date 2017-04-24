using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Areas.Admin.Models.ViewModels.ManageSession
{
    public class ManageSessionsViewModel
    {
        public ManageSessionsViewModel()
        {

        }

        public ManageSessionsViewModel(Session sessionDTO)
        {
            SessionId = sessionDTO.Id;
            Title = sessionDTO.Title;
            Description = sessionDTO.Description;
            Address = sessionDTO.Address;
            Room = sessionDTO.Room;
            StartDateTime = sessionDTO.StartDateTime;
            Occupancy = sessionDTO.Occupancy;
            RemainingSeats = sessionDTO.Occupancy;
        }

        public int SessionId { get; set; }

        [Display(Name = "Session Title")]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string Room { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime StartDateTime { get; set; }

        public int Occupancy { get; set; }

        [Display(Name = "Available")]
        public int RemainingSeats { get; set; }

        public bool IsSelected { get; set; }
    }
}
