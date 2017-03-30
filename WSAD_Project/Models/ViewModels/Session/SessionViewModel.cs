﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WSAD_Project.Models.Data;

namespace WSAD_Project.Models.ViewModels.Session
{
    public class SessionViewModel
    {
        public SessionViewModel()
        {

        }

        public SessionViewModel(Models.Data.Session sessionDTO)
        {
            Id = sessionDTO.Id;
            Title = sessionDTO.Title;
            Description = sessionDTO.Description;
            Address = sessionDTO.Address;
            Room = sessionDTO.Room;
            Time = sessionDTO.Time;
            Occupancy = sessionDTO.Occupancy;
            RemainingSeats = sessionDTO.Occupancy;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Room { get; set; }
        public DateTime Time { get; set; }
        public int Occupancy { get; set; }
        public int RemainingSeats { get; set; }
        public bool IsSelected { get; set; }
    }
}