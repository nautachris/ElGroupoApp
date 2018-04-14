using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class DepartmentUserGroupActivityGroup:ClassBase
    {
        public long UserGroupId { get; set; }
        public DepartmentUserGroup UserGroup { get; set; }
        public long ActivityGroupId { get; set; }
        public ActivityGroup ActivityGroup { get; set; }
    }
}
