using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class Activity : ClassBase
    {
        //whether the event should be visible to others or if it's a one-off only meant to be seen by myself
        public bool IsPublic { get; set; } = true;
        public long ActivityGroupId { get; set; }

        public string Notes { get; set; }
        //if not related to a conference, or if h
        public virtual ActivityGroup ActivityGroup { get; set; }

        /// <summary>
        /// these are the types/categories of credits OFFERED by the activity - not the number of hours of each
        /// </summary>
        public virtual ICollection<ActivityCredit> Credits { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<UserActivity> Users { get; set; }
        public ICollection<ActivityOrganizer> Organizers { get; set; }
    }
}
