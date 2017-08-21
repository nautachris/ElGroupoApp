using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class EventInformationModel
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long Id { get; set; }


        public bool IsNew { get; set; }
        public bool OrganizedByUser { get; set; }
        public bool Draft { get; set; }

    }
}
