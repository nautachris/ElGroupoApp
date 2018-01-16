using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ElGroupo.Web.Models.Events
{
    public abstract class EventDateModel : EventModelBase
    {
        [Required]
        [Display(Description = "Start Hour")]
        public int StartHour { get; set; }

        [Required]

        [Display(Description = "Start Minute")]
        public int StartMinute { get; set; }
        [Required]
        public Enums.AMPM StartAMPM { get; set; }

        [Required]
        [Display(Description = "End Hour")]
        public int EndHour { get; set; }
        [Required]
        [Display(Description = "End Minute")]
        public int EndMinute { get; set; }
        [Required]
        public Enums.AMPM EndAMPM { get; set; }

        [Required]
        [Display(Description = "Event Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Description = "Event Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; } = DateTime.Now;

        public DateTime GetEventStartTimeUTC(string tzId)
        {
            int startHour = GetAdjustedHour(this.StartHour, StartAMPM);
            var startDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, startHour, StartMinute, 0);
            if (tzId == null) return startDate;
            return TimeZoneInfo.ConvertTimeToUtc(startDate, TimeZoneInfo.FindSystemTimeZoneById(tzId));
        }
        public DateTime GetEventEndTimeUTC(string tzId)
        {
            int endHour = GetAdjustedHour(this.EndHour, EndAMPM);
            var endDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, endHour, EndMinute, 0);
            return TimeZoneInfo.ConvertTimeToUtc(endDate, TimeZoneInfo.FindSystemTimeZoneById(tzId));
        }
        public DateTime GetEventStartTimeLocal()
        {
            int startHour = GetAdjustedHour(this.StartHour, StartAMPM);
            var startDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, startHour, StartMinute, 0);
            return startDate;

        }
        public DateTime GetEventEndTimeLocal()
        {
            int endHour = GetAdjustedHour(this.EndHour, EndAMPM);
            var endDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, endHour, EndMinute, 0);
            return endDate;
        }

        private int GetAdjustedHour(int hour, Enums.AMPM ampm)
        {
            var newHour = 0;
            if (hour == 12) newHour = ampm == Models.Enums.AMPM.AM ? 0 : 12;
            else newHour = ampm == Models.Enums.AMPM.AM ? hour : hour + 12;
            return newHour;
        }
    }
}
