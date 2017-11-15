using ElGroupo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Domain;

namespace ElGroupo.Web.Models.Events
{
    public class ViewEventDetailsModel
    {
        public ViewEventDetailsModel(Event e)
        {
            this.Name = e.Name;
            this.Description = e.Description;
            this.Location = e.LocationName;
            this.StartTime = e.StartTime.DayOfWeek.ToString() + " " + e.StartTime.ToString("d") + " " + e.StartTime.ToString("t");
            this.EndTime = e.EndTime.DayOfWeek.ToString() + " "  +e.EndTime.ToString("d") + " " + e.EndTime.ToString("t");
            this.Status = e.Status;
            this.IsRecurring = e.Recurrence != null;
            if (this.IsRecurring) this.RecurrenceText = GetRecurrenceText(e.Recurrence);









        }

        private string GetNthDay(int day)
        {
            var dayChar = day.ToString()[day.ToString().Length - 1].ToString();
            switch (dayChar)
            {
                case "1":
                    return dayChar + "st";
                case "2":
                    return dayChar + "nd";
                case "3":
                    return dayChar + "rd";
                default:
                    return dayChar + "th";
                
            }
           
        }

        private string GetDaysText(DaysOfWeek days)
        {
            var dayList = new List<string>();
            foreach(DaysOfWeek d in Enum.GetValues(typeof(DaysOfWeek)))
            {
                if (d == DaysOfWeek.None) continue;
                if (days.HasFlag(d)) dayList.Add(d.ToString());
            }
            return string.Join(", ", dayList);
        }

        private string GetDaysInMonthText(string days)
        {
            var uniqueDays = days.Split(',').Distinct().ToList();
            var intDays = new List<int>();
            foreach(var d in uniqueDays)
            {
                intDays.Add(Convert.ToInt32(d));
            }
            intDays.Sort();

            var daysText = new List<string>();
            foreach (var d in intDays) daysText.Add(GetNthDay(d));
            return string.Join(", ", daysText);
        }

        private string GetRecurrenceText(RecurringEvent re)
        {
            string text = null;
            if (re.Pattern == RecurrencePatterns.Daily)
            {
                if (re.RecurrenceInterval == 1) text = "Every day";
                else text = "Every " + GetNthDay(re.RecurrenceInterval) + " days";
            }
            else if (re.Pattern == RecurrencePatterns.Monthly)
            {
                text = "The " + GetDaysInMonthText(re.DaysInMonth) + " of every ";
                if (re.RecurrenceInterval == 1) text += " month";
                else text += GetNthDay(re.RecurrenceInterval) + " month";

            }
            else if (re.Pattern == RecurrencePatterns.Weekly)
            {
                text = GetDaysText(re.RecurrenceDays);
                if (re.RecurrenceInterval == 1) text += " every week";
                else text += ", every " + GetNthDay(re.RecurrenceInterval) + " week";
            }
            text += ".";
            return text;
        }
        public EventStatus Status { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool IsRecurring { get; set; }
        public string RecurrenceText { get; set; }
    }
}
