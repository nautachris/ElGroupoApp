using ElGroupo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class EventInformationModel
    {
        public long EventAttendeeId { get; set; } = 0;
        public long? RecurrenceId { get; set; }        public List<EventInformationModel> Recurrences { get; set; } = new List<EventInformationModel>();

        public bool IsRecurrenceItem { get; set; }
        public CheckInStatuses CheckInStatus { get; set; }


        public bool IsRecurring { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DateText { get; set; }

        public long Id { get; set; }


        public bool IsNew { get; set; }
        public bool OrganizedByUser { get; set; }
        public EventStatus Status { get; set; }

        public RSVPTypes RSVPStatus { get; set; }

        public string OrganizerName { get; set; }
        public bool RSVPRequested { get; set; }

    }
}
