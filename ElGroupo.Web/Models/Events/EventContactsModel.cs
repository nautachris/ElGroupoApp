using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class EventContactsModel
    {
        public List<EventAttendeeModel> Attendees { get; set; } = new List<EventAttendeeModel>();
        //public List<EventOrganizerModel> Organizers { get; set; } = new List<EventOrganizerModel>();
        public EventInformationModel Event { get; set; } = new EventInformationModel();


    }
}
