using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ElGroupo.Web.Models.Events
{
    public class ViewEventModel
    {



        public string Name { get; set; }

        [Display(Description = "Event Description")]
        public string Description { get; set; }

        public string LocationName { get; set; }


        public string StartDateText { get; set; }
        public string EndDateText { get; set; }



        [Display(Description = "Address 1")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        [Display(Description = "City")]
        public string City { get; set; }


        [Display(Description = "State")]
        public string State { get; set; }
        public string ZipCode { get; set; }


        public string GooglePlaceId { get; set; }


        public List<EventAttendeeModel> Attendees { get; set; }
    }
}
