using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class AddRegisteredEventAttendeeModel
    {
        public bool isOwner { get; set; }
        public long userId { get; set; }
        public long eventId { get; set; }
    }
}
