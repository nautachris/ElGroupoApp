using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Domain.Enums;
namespace ElGroupo.Web.Models.Notifications
{
    public class EventNotificationModel
    {
        public long Id { get; set; }
        public string OrganizerName { get; set; }
        public long OrganizerId { get; set; }
        public NotificationImportanceTypes Importance { get; set; }
        public DateTime PostedDate { get; set; }
        public string NotificationText { get; set; }
        public string Subject { get; set; }

        public string DateText { get; set; }
        public bool IsNew { get; set; }
    }

    public class EventNotificationModelContainer
    {
        public List<EventNotificationModel> Notifications { get; set; }
        public bool IsOrganizer { get; set; }
    }
}
