using System;
using System.Collections.Generic;
using System.Text;

namespace ElGroupo.Domain
{
    public class Event:ClassBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        
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
    }
}
