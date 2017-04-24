using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSAD_Project.Models.ViewModels.Session
{
    public class ScheduledSessionDetailsViewModel
    {
        public ScheduledSessionDetailsViewModel() { }

        public ScheduledSessionDetailsViewModel(Models.Data.Session sessionDTO, int remainingSeats, List<string> presenterNames)
        {
            Id = sessionDTO.Id;
            Title = sessionDTO.Title;
            Description = sessionDTO.Description;
            Address = sessionDTO.Address;
            Room = sessionDTO.Room;
            StartDateTime = sessionDTO.StartDateTime;
            PresenterNames = presenterNames;
            Occupancy = sessionDTO.Occupancy;
            RemainingSeats = remainingSeats;
        }

        public int Id { get; set; }

        [Display(Name = "Session Title")]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string Room { get; set; }

        [Display(Name = "Date/Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime StartDateTime { get; set; }

        [Display(Name = "Presenters")]
        public List<string> PresenterNames { get; set; }

        [Display(Name = "Capacity")]
        public int Occupancy { get; set; }

        [Display(Name = "Available Seats")]
        public int RemainingSeats { get; set; }

        public bool IsSelected { get; set; }
    }

}