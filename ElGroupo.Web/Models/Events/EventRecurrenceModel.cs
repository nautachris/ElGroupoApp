using ElGroupo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class EventRecurrenceModel
    {

        public int RecurrenceInterval { get; set; } = 1;
        public int RecurrenceLimit { get; set; } = 10;
        public RecurrencePatterns Pattern { get; set; } = RecurrencePatterns.Daily;

        //this would be too tricky to deal with as a bool array
        public string DaysOfMonth { get; set; } = DateTime.Today.Day.ToString();

        public bool[] Days { get; set; } = new bool[7];
    }
}
