using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ElGroupo.Domain
{
    public class EventGroup:ClassBase
    {
        [Required]
        public string Name { get; set; }
        public ICollection<Event> Events { get; set; }

        public int OwnerId { get; set; }

        [Required]
        public User Owner { get; set; }
    }
}
