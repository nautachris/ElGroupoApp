using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Home
{
    public class UserDashboardModel
    {
        public string FirstName { get; set; }
        public long UserId { get; set; }
        public bool TimeZoneChanged { get; set; }
    }
}
