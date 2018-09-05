using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Account
{
    public class TimeZoneModel
    {
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public double OffsetMinutes { get; set; }
    }
}
