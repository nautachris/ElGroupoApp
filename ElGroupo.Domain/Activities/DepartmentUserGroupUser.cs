using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class DepartmentUserGroupUser:ClassBase
    {
        public long UserGroupId { get; set; }
        public DepartmentUserGroup UserGroup { get; set; }
        public long UserId { get; set; }
        public DepartmentUser User { get; set; }
        public bool IsOwner { get; set; }
    }
}
