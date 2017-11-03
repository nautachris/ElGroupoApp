using ElGroupo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class EventRecurrenceModel
    {
        public int RecurrenceInterval { get; set; } = 0;
        public int RecurrenceLimit { get; set; } = 10;
        public RecurrencePatterns Pattern { get; set; } = RecurrencePatterns.Daily;



        public bool[] Days { get; set; } = new bool[7];
    }
}
