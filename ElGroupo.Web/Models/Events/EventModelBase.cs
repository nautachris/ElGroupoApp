using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class EventModelBase
    {
        public long EventId { get; set; }
        public bool UpdateRecurring { get; set; }
        //public ElGroupo.Web.Classes.EditAccessTypes EditMode { get; set; }
    }
}
