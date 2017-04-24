using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Models.ViewModels.Session
{
    public class SessionRegistrationViewModel
    {
        public SessionRegistrationViewModel()
        {

        }

        public SessionRegistrationViewModel(Models.Data.Session sessionDTO)
        {
            Id = sessionDTO.Id;
            Title = sessionDTO.Title;
            Description = sessionDTO.Description;
            Address = sessionDTO.Address;
            Room = sessionDTO.Room;
            StartDateTime = sessionDTO.StartDateTime;
            Occupancy = sessionDTO.Occupancy;
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string Room { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime StartDateTime { get; set; }

        [Display(Name = "Capacity")]
        public int Occupancy { get; set; }

        public bool IsSelected { get; set; }

        public bool DatabaseUpdated { get; set; }
    }
}