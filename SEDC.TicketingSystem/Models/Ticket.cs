using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SEDC.TicketingSystem.Models
{
    public class Ticket
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public int Status { get; set; }

        [ForeignKey("User")]
        public int OwnerID { get; set; }

        [ForeignKey("Passport")]
        public int ModeratorID { get; set; }

        public DateTime OpenDate { get; set; }

        public DateTime CloseDate { get; set; }

        public int WorkHours { get; set; }

        public virtual ICollection<Reply> Replies { get; set; } 
    }
}