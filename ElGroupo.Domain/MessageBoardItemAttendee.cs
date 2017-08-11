using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElGroupo.Domain
{
    public class MessageBoardItemAttendee:ClassBase
    {
        public long MessageBoardItemId { get; set; }
        public long AttendeeId { get; set; }

        [Required]
        public virtual MessageBoardItem MessageBoardItem { get; set; }

        [Required]
        public virtual EventAttendee Attendee { get; set; }
        public bool Viewed { get; set; }
    }
}
