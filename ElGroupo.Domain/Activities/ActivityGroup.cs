using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class ActivityGroup : ClassBase
    {

        public long? UserId { get; set; }
        public User User { get; set; }
        //do we really need to directly relate department and activity group?
        //if private event, department will not be populated
        public long? DepartmentId { get; set; }
        public Department Department { get; set; }
        public ICollection<Activity> Activities { get; set; }
        public virtual ICollection<DepartmentUserGroupActivityGroup> UserGroups { get; set; }
        public ICollection<ActivityGroupOrganizer> Organizers { get; set; }
        public string Name { get; set; }

        
    }
}
