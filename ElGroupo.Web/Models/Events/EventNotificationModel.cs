using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Domain.Enums;
namespace ElGroupo.Web.Models.Events
{
    public class EventNotificationModel
    {
        public long Id { get; set; }
        public string OrganizerName { get; set; }
        public int OrganizerId { get; set; }
        public NotificationImportanceTypes Importance { get; set; }
        public DateTime PostedDate { get; set; }
        public string NotificationText { get; set; }
        public string Subject { get; set; }
        public bool CanEdit { get; set; }
    }
}
