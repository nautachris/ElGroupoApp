using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElGroupo.Domain
{
    public class EventAttendeeNotification: ClassBase
    {
        public long NotificationId { get; set; }
        public long AttendeeId { get; set; }


        [Required]
        public virtual EventNotification Notification { get; set; }

        [Required]
        public virtual EventAttendee Attendee { get; set; }

        public bool Viewed { get; set; }
    }
}
