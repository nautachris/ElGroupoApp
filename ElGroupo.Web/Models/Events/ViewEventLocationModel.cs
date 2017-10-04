using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class ViewEventLocationModel
    {
        public ViewEventLocationModel(ElGroupo.Domain.Event e)
        {
            this.Address1 = e.Address1;
            this.Address2 = e.Address2;
            this.City = e.City;
            this.ZipCode = e.Zip;
            this.State = e.State;
            this.LocationName = e.LocationName;
            this.GooglePlaceId = e.GooglePlaceId;
        }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string LocationName { get; set; }
        public string GooglePlaceId { get; set; }

    }
}
