using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain
{
    public class Conference: ClassBase
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<CMEActivity> CMEActivities { get; set; }
        public virtual ICollection<CMEActivity> NonCMEActivities { get; set; }
        
    }
}
