using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElGroupo.Domain
{
    public class EventNotification : ClassBase
    {
        public EventNotification() { }
        public long EventId { get; set; }

        //public long EventId { get; set; }

        [Required]
        public EventAttendee PostedBy { get; set; }

        [Required]
        public Event Event { get; set; }
        public DateTime PostedDate { get; set; }
        public string MessageText { get; set; }
        public string Subject { get; set; }
        public Enums.NotificationImportanceTypes Importance { get; set; }

        //public virtual ICollection<EventAttendeeNotification> Attendees { get; set; }
    }
}
