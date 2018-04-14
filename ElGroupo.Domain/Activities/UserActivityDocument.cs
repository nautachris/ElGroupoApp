using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class UserActivityDocument:ClassBase
    {
        public long UserActivityId { get; set; }
        public UserActivity UserActivity { get; set; }
        public AccreditationDocument Document { get; set; }
        public long DocumentId { get; set; }

    }
}
