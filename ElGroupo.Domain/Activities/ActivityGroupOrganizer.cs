using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class ActivityGroupOrganizer:ClassBase
    {
        public ActivityGroup ActivityGroup { get; set; }
        public long ActivityGroupId { get; set; }

        //what if i am part of three user groups, and the activity is shared with all three
        //what if i am an admin for this group, then i'm removed from the group in which it is shared?
        public User User { get; set; }
        public long UserId { get; set; }

    }
}
