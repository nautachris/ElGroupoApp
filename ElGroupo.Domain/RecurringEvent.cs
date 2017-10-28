using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElGroupo.Domain.Enums;
namespace ElGroupo.Domain
{
    public class RecurringEvent:ClassBase
    {
        public ICollection<Event> Events { get; set; }

        //corresponds to the pattern, i.e. every 5 days
        public int RecurrenceInterval { get; set; }

        //end after x
        public int RecurrenceLimit { get; set; }
        public RecurrencePatterns Pattern { get; set; }

        public DaysOfWeek RecurrenceDays { get; set; }
    }
}
