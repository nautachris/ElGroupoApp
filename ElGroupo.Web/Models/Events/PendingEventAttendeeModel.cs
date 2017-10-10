using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Events
{
    public class PendingEventAttendeeModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Registered { get; set; }
        public bool Owner { get; set; }
    }
}
