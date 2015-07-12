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
    }
}
