using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain
{
    public class UserActivityDocument:ClassBase
    {
        public UserActivity UserActivity { get; set; }
        public long UserActivityId { get; set; }
        public AccreditationDocument Document { get; set; }
        public long AccreditationDocumentId { get; set; }
    }
}
