using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain
{
    public class UserActivity:ClassBase
    {
        public virtual Activity Activity { get; set; }
        public virtual User User { get; set; }
        public ICollection<UserActivityDocument> Documents { get; set; }

    }
}
