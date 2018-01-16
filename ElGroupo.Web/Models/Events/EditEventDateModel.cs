using ElGroupo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Web.Classes;
namespace ElGroupo.Web.Models.Events
{
    public class EditEventDateModel : EventDateModel
    {
        public List<RecurrenceListModel> Recurrence { get; set; } = new List<RecurrenceListModel>();
        public EditEventDateModel(Event e, string timeZoneId)
        {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var start = e.StartTime.FromUTC(tzi);
            var end = e.EndTime.FromUTC(tzi);

            this.StartAMPM = start.Hour < 12 ? Enums.AMPM.AM : Enums.AMPM.PM;
            this.StartHour = start.Hour >= 12 ? start.Hour - 12 : start.Hour;
            this.StartMinute = start.Minute;
            this.EndAMPM = end.Hour >= 12 ? Enums.AMPM.AM : Enums.AMPM.AM;
            this.EndHour = end.Hour >= 12 ? end.Hour - 12 : end.Hour;
            this.EndMinute = end.Minute;

            this.StartDate = start.Date;
            this.EndDate = end.Date;

            this.Recurrence = e.Recurrence?.Events.Where(x => x.Id != e.Id).OrderBy(x => x.StartTime).Select(y => new RecurrenceListModel
            {
                Name = y.Name,
                StartDate = y.StartTime.FromUTC(timeZoneId),
                EndDate = y.EndTime.FromUTC(timeZoneId),
                Id = y.Id,
                DateText = y.GetDateText(timeZoneId)
            }).ToList();

        }
    }
}
