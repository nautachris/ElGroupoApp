using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class SendRSVPRequestModel
    {
        public long EventId { get; set; }
        public bool UpdateRecurring { get; set; }
    }
}
