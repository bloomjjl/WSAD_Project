using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WSAD_Project.Models.Data
{
    [Table("tblPresenterSession")]
    public class PresenterSession
    {
        [Key]
        [Column("User_Id", Order = 0)] // connects foreign key below
        public int UserId { get; set; }

        [Key]
        [Column("Session_Id", Order = 1)] // connects foreign key below
        public int SessionId { get; set; }

        public DateTime CreateDate { get; set; }


        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }
    }
}