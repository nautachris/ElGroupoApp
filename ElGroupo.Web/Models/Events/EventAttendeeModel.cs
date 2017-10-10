using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Domain.Enums;
namespace ElGroupo.Web.Models.Events
{
    public class EventAttendeeModel
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public long UserId { get; set; }
        public RSVPTypes RSVPStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsOrganizer { get; set; }
    }

    public class UnregisteredEventAttendeeModel
    {
        public string Name { get; set; }
        public long EventId { get; set; }
        public string Email { get; set; }
    }
}
