using ElGroupo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Web.Classes;
namespace ElGroupo.Web.Models.Events
{
    public class ViewEventDatesModel
    {
        public List<RecurrenceListModel> Recurrence { get; set; } = new List<RecurrenceListModel>();
        public string StartText { get; set; }
        public string EndText { get; set; }

        public ViewEventDatesModel(Event e, string timeZoneId)
        {
            this.StartText = e.GetStartTimeText(timeZoneId, true);
            this.EndText = e.GetEndTimeText(timeZoneId, true);
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
