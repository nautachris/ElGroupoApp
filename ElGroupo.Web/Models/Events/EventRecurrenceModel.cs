using ElGroupo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class EventRecurrenceModel
    {
        public int RecurrenceInterval { get; set; }
        public int RecurrenceLimit { get; set; }
        public RecurrencePatterns Pattern { get; set; }

        public bool[] Days { get; set; } = new bool[7];
    }
}
