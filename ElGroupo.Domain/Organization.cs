using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain
{
    public class Organization:ClassBase
    {
                public string AccreditationId { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }


                public virtual ICollection<Conference> Conferences { get; set; }
        public virtual ICollection<CMEActivity> CMEActivities { get; set; }
        public virtual ICollection<NonCMEActivity> NonCMEActivities { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
