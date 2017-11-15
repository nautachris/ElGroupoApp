using ElGroupo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class UpdateEventStatusModel
    {
        public long EventId { get; set; }
        public bool UpdateRecurring { get; set; }
        public EventStatus Status { get; set; }
    }
}
