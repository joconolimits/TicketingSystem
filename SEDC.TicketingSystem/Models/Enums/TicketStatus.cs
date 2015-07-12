using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEDC.TicketingSystem.Models.Enums
{
    public enum TicketStatus : byte
    {
        Pending = 1,
        WaitReply = 2,
        Closed = 3,
    }
}
