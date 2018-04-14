using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Activities
{
    public class UserActivityCredit:ClassBase
    {
        public long UserActivityId { get; set; }
        public UserActivity UserActivity { get; set; }
        public long CreditTypeCategoryId { get; set; }
        public CreditTypeCategory CreditTypeCategory { get; set; }
        public double CreditHours { get; set; } = 0;
    }
}
