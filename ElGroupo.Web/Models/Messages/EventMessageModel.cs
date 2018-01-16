using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Messages
{
    public class EventMessageModel
    {
        public bool CanEdit { get; set; }
        public string PostedBy { get; set; }
        public long PostedById { get; set; }
        public DateTime PostedDate { get; set; }
        public string Subject { get; set; }
        public string MessageText { get; set; }
        public string DateText { get; set; }
        public bool IsNew { get; set; }
        public long Id { get; set; }
    }
}
