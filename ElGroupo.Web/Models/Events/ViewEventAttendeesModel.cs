using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class ViewEventAttendeesModel
    {
        public ViewEventAttendeesModel()
        {
            this.Attendees = new List<EventAttendeeModel>();

        }
        public bool IsOrganizer { get; set; }
        public List<EventAttendeeModel> Attendees { get; set; }
    }
}
