using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEDC.TicketingSystem.Models
{
    public class Reply
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Ticket")]
        public int TicketID { get; set; }
        public Ticket Ticket { get; set; }

        [Required]
        public string ReplyBody { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
