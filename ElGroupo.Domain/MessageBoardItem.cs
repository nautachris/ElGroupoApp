using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElGroupo.Domain
{
    public class MessageBoardItem:ClassBase
    {
        public int UserId { get; set; }
        public long EventId { get; set; }

        [Required]
        public virtual User User { get; set; }

        [Required]
        public virtual Event Event { get; set; }
        public DateTime PostedDate { get; set; }

        [Required]
        public string MessageText { get; set; }

        public string Subject { get; set; }

        public virtual ICollection<MessageBoardItemAttendee> Attendees { get; set; }

    }
}
