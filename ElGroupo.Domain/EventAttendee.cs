using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElGroupo.Domain
{
    public class EventAttendee:ClassBase
    {
        //public int UserId { get; set; }
        public long EventId { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Event Event { get; set; }
        public bool Viewed { get; set; }

        public bool IsOrganizer { get; set; }

        public Enums.RSVPTypes ResponseStatus { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string ResponseText { get; set; }
        public bool? AllowEventUpdates { get; set; }

        public bool? ShowRSVPReminder { get; set; }


        public DateTime? CheckInTime { get; set; }
        public double? CheckInCoordinateX { get; set; }
        public double? CheckInCoordinateY { get; set; }

        public virtual ICollection<EventNotification> PostedNotifications { get; set; }

        public virtual ICollection<EventAttendeeNotification> Notifications { get; set; }
        public virtual ICollection<MessageBoardItemAttendee> MessageBoardItems { get; set; }
    }
}
