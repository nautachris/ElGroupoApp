using ElGroupo.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class RecordsDashboardModel
    {
        public string FirstName { get; set; }
        public long UserId { get; set; }
        public List<IdValueModel> Categories { get; set; }
    }
}
