using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class EditEventLocationModel : EventModelBase
    {

        public EditEventLocationModel(ElGroupo.Domain.Event e)
        {
            this.EventId = e.Id;
            this.Address1 = e.Address1;
            this.Address2 = e.Address2;
            this.City = e.City;
            this.GooglePlaceId = e.GooglePlaceId;
            this.LocationName = e.LocationName;
            this.State = e.State;
            this.XCoord = e.CoordinateX;
            this.YCoord = e.CoordinateY;
            
        }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string LocationName { get; set; }
        public string GooglePlaceId { get; set; }
        public double YCoord { get; set; }
        public double XCoord { get; set; }
    }
}
