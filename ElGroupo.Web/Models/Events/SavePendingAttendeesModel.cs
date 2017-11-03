using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class SavePendingAttendeesModel
    {
        public bool UpdateRecurring { get; set; }
        public long EventId { get; set; }
        public PendingEventAttendeeModel[] Attendees { get; set; }
    }
}
