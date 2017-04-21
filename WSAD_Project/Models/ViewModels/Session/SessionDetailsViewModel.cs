using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Models.ViewModels.Session
{
    public class SessionDetailsViewModel
    {
        public SessionDetailsViewModel()
        {

        }

        public SessionDetailsViewModel(Models.Data.Session sessionDTO, int remainingSeats, List<string> presenterNames)
        {
            Id = sessionDTO.Id;
            Title = sessionDTO.Title;
            Description = sessionDTO.Description;
            Address = sessionDTO.Address;
            Room = sessionDTO.Room;
            Time = sessionDTO.Time;
            PresenterNames = presenterNames;
            Occupancy = sessionDTO.Occupancy;
            RemainingSeats = remainingSeats;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Room { get; set; }
        public DateTime Time { get; set; }
        [Display(Name = "Presenters")]
        public List<string> PresenterNames { get; set; }
        public int Occupancy { get; set; }
        [Display(Name = "Available Seats")]
        public int RemainingSeats { get; set; }
        public bool IsSelected { get; set; }
    }
}