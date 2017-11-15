using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Web.Models.Events;
namespace ElGroupo.Web.Models.Home
{
    public class DashboardModel
    {
        public List<EventInformationModel> RSVPRequestedEvents { get; set; } = new List<EventInformationModel>();
        public List<EventInformationModel> PastEvents { get; set; } = new List<EventInformationModel>();
        public List<EventInformationModel> FutureEvents { get; set; } = new List<EventInformationModel>();
        public List<EventInformationModel> Drafts { get; set; } = new List<EventInformationModel>();
    }
}
