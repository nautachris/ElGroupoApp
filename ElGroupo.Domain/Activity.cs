using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain
{
    public abstract class Activity : ClassBase
    {
        //if not related to a conference, or if h
        public virtual Organization Organization { get; set; }
        public long? OrganizationId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<UserActivity> Users { get; set; }
    }
}
