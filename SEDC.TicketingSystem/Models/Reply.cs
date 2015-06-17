using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEDC.TicketingSystem.Models
{
    public class Reply
    {
        [Key]
        public int ID { get; set; }

        public int TicketID { get; set; }

        [Required]
        public string ReplyBody { get; set; }

        public int UserID { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
