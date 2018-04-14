using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class Department:ClassBase
    {
        public string Name { get; set; }
        public long OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public virtual ICollection<DepartmentUser> Users { get; set; }
        public virtual ICollection<DepartmentUserGroup> UserGroups { get; set; }
        public virtual ICollection<ActivityGroup> ActivityGroups { get; set; }
    }
}
