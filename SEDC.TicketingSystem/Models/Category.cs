using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEDC.TicketingSystem.Models
{
    public class Category
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string Name { get; set; }

        [ForeignKey("Moderator")]
        public int ModeratorID { get; set; }
        public User Moderator { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } 
    }
}
