using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Web.Models.Enums;
using ElGroupo.Domain.Enums;

namespace ElGroupo.Web.Models.Events
{
    public class EditEventDetailsModel
    {
        public EditEventDetailsModel() { }
        public EditEventDetailsModel(ElGroupo.Domain.Event e)
        {
            this.EventId = e.Id;
            this.Description = e.Description;
            this.EndAMPM = e.EndTime.Hour >= 12 ? Models.Enums.AMPM.PM : Models.Enums.AMPM.AM;
            this.EndHour = e.EndTime.Hour;
            this.EndMinute = e.EndTime.Minute;
            this.EventDate = e.StartTime.Date;
            this.Name = e.Name;
            this.StartAMPM = e.StartTime.Hour >= 12 ? Models.Enums.AMPM.PM : Models.Enums.AMPM.AM;
            this.StartHour = e.StartTime.Hour;
            this.StartMinute = e.StartTime.Minute;

            this.AttendanceVerificationMethod = e.VerificationMethod;
            this.LocationTolerance = e.CheckInLocationTolerance;
            this.VerificationCode = e.VerificationCode;
            this.Status = e.Status;

        }

        public bool UpdateRecurring { get; set; }

        public EventRecurrenceModel Recurrence { get; set; }

        public EventStatus Status { get; set; }
        public long EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public AMPM StartAMPM { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public AMPM EndAMPM { get; set; }
        public bool RSVPRequired { get; set; }
        public AttendanceVerificationMethods AttendanceVerificationMethod { get; set; }
        public string VerificationCode { get; set; }
        public double LocationTolerance { get; set; }
    }
}
