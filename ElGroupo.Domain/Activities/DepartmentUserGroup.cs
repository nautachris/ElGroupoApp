using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class DepartmentUserGroup: ClassBase
    {
        public string Name { get; set; }
        public long DepartmentId { get; set; }
        public Department Department { get; set; }
        public virtual ICollection<DepartmentUserGroupUser> Users { get; set; }
        public virtual ICollection<DepartmentUserGroupActivityGroup> ActivityGroups { get; set; }
    }
}
