using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain
{
    public class AttendeeGroup : ClassBase
    {
        public User User { get; set; }
        public string Name { get; set; }
        public ICollection<AttendeeGroupUser> Attendees { get; set; }
    }
}
