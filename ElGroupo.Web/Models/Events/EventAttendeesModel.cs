using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class EventAttendeesModel
    {
        public EventAttendeesModel(long eventId)
        {
            this.EventId = eventId;
            this.Attendees = new List<EventAttendeeModel>();

        }
        public long EventId { get; set; }
        public bool IsOrganizer { get; set; }
        public List<EventAttendeeModel> Attendees { get; set; }
    }
}
