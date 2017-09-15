using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElGroupo.Domain
{
    public class EventNotification : ClassBase
    {
        public EventNotification(){
            this.Attendees = new HashSet<EventAttendeeNotification>();
            }
        public long EventId { get; set; }
        public long EventOrganizerId { get; set; }
        //public long EventId { get; set; }

        [Required]
        public EventOrganizer PostedBy { get; set; }

        [Required]
        public Event Event { get; set; }
        public DateTime PostedDate { get; set; }
        public string MessageText { get; set; }
        public string Subject { get; set; }
        public Enums.NotificationImportanceTypes Importance { get; set; }

        public virtual ICollection<EventAttendeeNotification> Attendees { get; set; }
    }
}
