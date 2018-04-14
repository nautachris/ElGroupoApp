using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class DepartmentUser:ClassBase
    {
        public long UserId { get; set; }
        public User User { get; set; }
        public long DepartmentId { get; set; }
        public Department Department { get; set; }
        public virtual ICollection<DepartmentUserGroupUser> UserGroupUsers { get; set; }
    }
}
