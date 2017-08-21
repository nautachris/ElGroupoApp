
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class EventOrganizerModel
    {
        //https://developers.google.com/maps/documentation/javascript/examples/places-autocomplete-addressform


        public string Name { get; set; }
        public long Id { get; set; }
        public int UserId { get; set; }
        public bool Owner { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class AddEventOrganizerModel
    {
        public bool Owner { get; set; }
        public long EventId { get; set; }
        public int UserId { get; set; }
    }
}
