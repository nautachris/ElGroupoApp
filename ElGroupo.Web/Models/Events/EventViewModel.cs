using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ElGroupo.Web.Models.Messages;
using ElGroupo.Web.Models.Notifications;
using ElGroupo.Domain.Enums;

namespace ElGroupo.Web.Models.Events
{
    public class EventViewModel
    {
        public EventViewModel(ElGroupo.Domain.Event e)
        {
            this.EventId = e.Id;
            this.Details = new ViewEventDetailsModel(e);
            this.Location = new ViewEventLocationModel(e);
            this.IsRecurring = e.Recurrence != null;
        }

        public bool IsRecurring { get; set; }

        public EventAttendeeRSVPModel RSVPResponse { get; set; }
        public ViewEventDetailsModel Details { get; set; }
        public ViewEventLocationModel Location { get; set; }

        public long EventId { get; set; }

        public bool IsOrganizer { get; set; }

        public string OrganizerName { get; set; }




        public List<EventNotificationModel> Notifications { get; set; }
        public List<EventMessageModel> Messages { get; set; }






        public ViewEventAttendeesModel Attendees { get; set; }
    }
}
