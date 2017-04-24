using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSAD_Project.Areas.Admin.Models.ViewModels.ManageSession
{
    public class CreateManageSessionViewModel
    {
        public CreateManageSessionViewModel () { }

        public CreateManageSessionViewModel (WSAD_Project.Models.Data.Session sessionDTO)
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

        [Required]
        [Display(Name = "Session Title")]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Room { get; set; }

        [Required]
        [Display(Name = "Date/Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime StartDateTime { get; set; }

        [Required]
        [Display(Name = "Capacity")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please enter a number greater than zero.")]
        public int Occupancy { get; set; }
    }
}