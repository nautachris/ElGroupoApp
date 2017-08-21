using System;
using System.Collections.Generic;
using System.Text;

namespace ElGroupo.Domain
{
    public class UnregisteredEventAttendee: ClassBase
    {
        public Guid RegisterToken { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public long EventId { get; set; }
        public Event Event { get; set; }
    }
}
