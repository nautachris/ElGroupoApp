using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain
{
    public class AttendeeGroupUser:ClassBase
    {
        public AttendeeGroup AttendeeGroup { get; set; }
        public User User { get; set; }
    }
}
