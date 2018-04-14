using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class ActivityGroup : ClassBase
    {
        public long DepartmentId { get; set; }
        public Department Department { get; set; }
        public ICollection<Activity> Activities { get; set; }
        public virtual ICollection<DepartmentUserGroupActivityGroup> UserGroups { get; set; }
        public ICollection<ActivityGroupOrganizer> Organizers { get; set; }
        public string Name { get; set; }
    }
}
