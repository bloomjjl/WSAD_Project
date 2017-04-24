using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSAD_Project.Areas.Admin.Models.ViewModels.ManageSession
{
    public class EditManageSessionViewModel
    {
        public EditManageSessionViewModel() { }

        public EditManageSessionViewModel(WSAD_Project.Models.Data.Session sessionDTO, List<EditSessionPresenter> sessionPresenters)
        {
            Id = sessionDTO.Id;
            Title = sessionDTO.Title;
            Description = sessionDTO.Description;
            Address = sessionDTO.Address;
            Room = sessionDTO.Room;
            StartDateTime = sessionDTO.StartDateTime;
            Occupancy = sessionDTO.Occupancy;
            Presenters = sessionPresenters;
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

        [Display(Name = "Capacity")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please enter a number greater than zero.")]
        public int Occupancy { get; set; }

        public List<EditSessionPresenter> Presenters { get; set; }
    }


    public class EditSessionPresenter
    {
        public int PresenterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool AddPresenter { get; set; }
        public bool RemovePresenter { get; set; }
    }

}