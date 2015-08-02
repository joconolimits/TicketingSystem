using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEDC.TicketingSystem.Models;

namespace SEDC.TicketingSystem.ViewModels
{
    public class TicketAndRepliesViewModel
    {

        public Ticket Ticket { get; set; }
        public System.Linq.IQueryable<Reply> Replies { get; set; }

        private SEDCTicketingSystemContext db = new SEDCTicketingSystemContext();

        public string GetUsername(int UserID)
        {

            return db.Users.Find(UserID).Username;
        }
        public string GetCategory(int CategoryID)
        {

            return db.Categories.Find(CategoryID).Name;
        }
 
    }

            
}
