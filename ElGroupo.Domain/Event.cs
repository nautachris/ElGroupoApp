using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ElGroupo.Domain.Enums;
using System.Text;

namespace ElGroupo.Domain
{
    public class Event : ClassBase
    {

        public string GetDateText(string timeZoneId)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            var localStartTime = TimeZoneInfo.ConvertTimeFromUtc(this.StartTime, tz);
            var startTimezoneName = tz.IsDaylightSavingTime(localStartTime) ? tz.DaylightName : tz.StandardName;
            var localEndTime = TimeZoneInfo.ConvertTimeFromUtc(this.EndTime, tz);
            var endTimezoneName = tz.IsDaylightSavingTime(localEndTime) ? tz.DaylightName : tz.StandardName;
            if (localStartTime.Date == localEndTime.Date)
            {
                //ev.ev.StartTime.ToLocalTime().DayOfWeek.ToString() + " " + ev.ev.StartTime.ToLocalTime().ToString("d") + " " + ev.ev.StartTime.ToLocalTime().ToString("t")
                return localStartTime.DayOfWeek.ToString() + " " + localStartTime.ToString("d") + " " + localStartTime.ToString("t") + " - " + localEndTime.ToString("t") + " " + startTimezoneName;
            }
            else
            {
                if (startTimezoneName == endTimezoneName) return localStartTime.DayOfWeek.ToString() + " " + localStartTime.ToString("d") + " " + localStartTime.ToString("t") + " - " + localEndTime.DayOfWeek.ToString() + localEndTime.ToString("d") + " " + localEndTime.ToString("t") + " " + endTimezoneName;
                else return localStartTime.DayOfWeek.ToString() + " " + localStartTime.ToString("d") + " " + localStartTime.ToString("t") + " " + startTimezoneName + " - " + localEndTime.DayOfWeek.ToString() + localEndTime.ToString("d") + " " + localEndTime.ToString("t") + " " + endTimezoneName;

            }

        }

        public string GetStartTimeText(string timeZoneId)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var localStartTime = TimeZoneInfo.ConvertTimeFromUtc(this.StartTime, tz);
            var tzName = tz.IsDaylightSavingTime(localStartTime) ? tz.DaylightName : tz.StandardName;
            return localStartTime.DayOfWeek.ToString() + " " + localStartTime.ToString("d") + " " + localStartTime.ToString("t") + " - " + tzName;
        }
        public string GetEndTimeText(string timeZoneId)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var localEndTime = TimeZoneInfo.ConvertTimeFromUtc(this.EndTime, tz);
            var tzName = tz.IsDaylightSavingTime(localEndTime) ? tz.DaylightName : tz.StandardName;
            return localEndTime.DayOfWeek.ToString() + " " + localEndTime.ToString("d") + " " + localEndTime.ToString("t") + " - " + tzName;
        }
        public Event()
        {
            this.Attendees = new HashSet<EventAttendee>();
            this.UnregisteredAttendees = new HashSet<UnregisteredEventAttendee>();
        }
        public string Name { get; set; }
        public string Description { get; set; }

        public AttendanceVerificationMethods VerificationMethod { get; set; }

        [MaxLength(10)]
        public string VerificationCode { get; set; }

        public bool MustRSVP { get; set; }
        public string LocationName { get; set; }

        [MaxLength(50)]
        public string GooglePlaceId { get; set; }

        [MaxLength(100)]
        public string Address1 { get; set; }
        [MaxLength(100)]
        public string Address2 { get; set; }
        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(10)]
        public string State { get; set; }

        [MaxLength(10)]
        public string Zip { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public int CheckInTimeTolerance { get; set; }
        public double CheckInLocationTolerance { get; set; }
        public EventStatus Status { get; set; }

        public long? GroupId { get; set; }

        public virtual RecurringEvent Recurrence { get; set; }
        public long? RecurrenceId { get; set; }

        public virtual ICollection<EventAttendee> Attendees { get; set; }
        public virtual ICollection<MessageBoardItem> MessageBoardItems { get; set; }

        public virtual ICollection<EventNotification> Notifications { get; set; }

        public virtual ICollection<UnregisteredEventAttendee> UnregisteredAttendees { get; set; }
    }
}
