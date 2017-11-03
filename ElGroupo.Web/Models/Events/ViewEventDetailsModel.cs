using ElGroupo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class ViewEventDetailsModel
    {
        public ViewEventDetailsModel(ElGroupo.Domain.Event e)
        {
            this.Name = e.Name;
            this.Description = e.Description;
            this.Location = e.LocationName;
            this.StartTime = e.StartTime.ToString("d") + " " + e.StartTime.ToString("t");
            this.EndTime = e.EndTime.ToString("d") + " " + e.EndTime.ToString("t");
            this.Status = e.Status;
        }
        public EventStatus Status { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
