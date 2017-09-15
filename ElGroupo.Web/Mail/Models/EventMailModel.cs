using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Mail.Models
{
    public abstract class EventMailModel : MailModelBase
    {
        public string EventName { get; set; }
        public string EventCreatedBy { get; set; }
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
