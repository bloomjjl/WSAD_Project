﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WSAD_Project.Models.Data
{
    [Table("tblUserSession")]
    public class UserSession
    {
        [Key]
        public int Id { get; set; }

        [Column("User_Id")] // connects foreign key below
        public int UserId { get; set; }

        [Column("Session_Id")] // connects foreign key below
        public int SessionId { get; set; }


        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }
    }
}