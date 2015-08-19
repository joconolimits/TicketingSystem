using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEDC.TicketingSystem.Models.Enums
{
    public enum AccessLevel : byte
    {
        Registered = 0,
        Moderator = 1,
        SuperAdmin = 2,
    }
}
