using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElGroupo.Domain
{
    public class Event:ClassBase
    {
        public Event()
        {
            this.Organizers = new HashSet<EventOrganizer>();
            this.Attendees = new HashSet<EventAttendee>();
            this.UnregisteredAttendees = new HashSet<UnregisteredEventAttendee>();
        }
        public string Name { get; set; }
        public string Description { get; set; }

        public string LocationName { get; set; }

        [MaxLength(50)]
        public string GooglePlaceId { get; set; }

        [MaxLength(100)]
        public string Address1 { get; set; }
        [MaxLength(100)]
        public string Address2 { get; set; }
        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(10)]
        public string State { get; set; }

        [MaxLength(10)]
        public string Zip { get; set; }
        
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public int CheckInTimeTolerance { get; set; }
        public double CheckInLocationTolerance { get; set; }
        public bool SavedAsDraft { get; set; }

        public long? GroupId { get; set; }

        public virtual ICollection<EventOrganizer> Organizers { get; set; }
        public virtual EventGroup Group { get; set; }
        public virtual ICollection<EventAttendee> Attendees { get; set; }
        public virtual ICollection<MessageBoardItem> MessageBoardItems { get; set; }

        public virtual ICollection<EventNotification> Notifications { get; set; }

        public virtual ICollection<UnregisteredEventAttendee> UnregisteredAttendees { get; set; }
    }
}
