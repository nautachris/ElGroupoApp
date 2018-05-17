using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class UserActivity:ClassBase
    {
        public long AttendanceTypeId { get; set; }
        public string Notes { get; set; }
        public string PresentationName { get; set; }
        //public ActivityAttendanceType AttendanceType { get; set; }
        public bool IsPresenting { get; set; }
        public long ActivityId { get; set; }
        public virtual Activity Activity { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; }
        public ICollection<UserActivityDocument> Documents { get; set; }
        public ICollection<UserActivityCredit> Credits { get; set; }

    }
}
