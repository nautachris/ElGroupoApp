using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Domain.Enums;
namespace ElGroupo.Web.Models.Events
{
    public class EventAttendeeModel
    {
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public string Name { get; set; }
        public long? EventAttendeeId { get; set; }
        public long? UserId { get; set; }
        public long? UserGroupId { get; set; }
        public RSVPTypes RSVPStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsOrganizer { get; set; }
        public bool IsRegistered { get; set; } = true;
        public bool IsEditable { get; set; } = true;
    }

    public class UpdateEventAttendeesModel
    {
        public long EventId { get; set; }
        public bool UpdateRecurring { get; set; }
        public EventAttendeeModel[] Attendees { get; set; }
    }


}
