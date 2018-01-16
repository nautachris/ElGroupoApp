using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Web.Models.Enums;
using ElGroupo.Domain.Enums;
using ElGroupo.Web.Classes;
namespace ElGroupo.Web.Models.Events
{
    public class EditEventDetailsModel: EventDateModel
    {
        public EditEventDetailsModel() { }
        public EditEventDetailsModel(ElGroupo.Domain.Event e, string tzId)
        {
            var localStart = e.EndTime.FromUTC(TimeZoneInfo.FindSystemTimeZoneById(tzId));
            this.EventId = e.Id;
            this.Description = e.Description;
            this.EndDate = e.EndTime.Date;
            this.EndAMPM = e.EndTime.Hour >= 12 ? Models.Enums.AMPM.PM : Models.Enums.AMPM.AM;
            this.EndHour = e.EndTime.Hour;
            this.EndMinute = e.EndTime.Minute;
            this.StartDate = e.StartTime.Date;
            this.Name = e.Name;
            this.StartAMPM = e.StartTime.Hour >= 12 ? Models.Enums.AMPM.PM : Models.Enums.AMPM.AM;
            this.StartHour = e.StartTime.Hour;
            this.StartMinute = e.StartTime.Minute;

            this.AttendanceVerificationMethod = e.VerificationMethod;
            this.LocationTolerance = e.CheckInLocationTolerance;
            this.VerificationCode = e.VerificationCode;
            this.Status = e.Status;

        }

        //public bool UpdateRecurring { get; set; }

        public EventRecurrenceModel Recurrence { get; set; }

        public EventStatus Status { get; set; }
        //public long EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool RSVPRequired { get; set; }
        public AttendanceVerificationMethods AttendanceVerificationMethod { get; set; }
        public string VerificationCode { get; set; }
        public double LocationTolerance { get; set; }
    }
}
