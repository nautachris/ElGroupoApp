using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElGroupo.Domain
{
    public class EventOrganizer:ClassBase
    {
        public int UserId { get; set; }
        public long EventId { get; set; }

        [Required]
        public virtual Event Event { get; set; }

        [Required]
        public virtual User User { get; set; }
        public bool Owner { get; set; }
    }
}
