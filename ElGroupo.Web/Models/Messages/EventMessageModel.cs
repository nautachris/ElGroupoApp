using ElGroupo.Web.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Messages
{
    public class EventMessageContainerModel
    {
        public List<EventMessageBoardTopicModel> Topics { get; set; } = new List<EventMessageBoardTopicModel>();
        
    }
    public class EventMessagePageModel
    {
        public EventMessageContainerModel Messages { get; set; } = new EventMessageContainerModel();
        public string EventName { get; set; }

        public long EventId { get; set; }
        public string Description { get; set; }
        public List<EventOrganizerModel> Organizers { get; set; } = new List<EventOrganizerModel>();
    }
    public class EventMessageBoardTopicModel
    {
        public List<EventMessageModel> Messages { get; set; } = new List<EventMessageModel>();
        public string Subject { get; set; }
        public string StartedBy { get; set; }
        public long Id { get; set; }
    }


    public class EventMessageModel
    {
        public string TopicName { get; set; }
        public bool CanEdit { get; set; }
        public string PostedBy { get; set; }
        public long PostedById { get; set; }
        public DateTime PostedDate { get; set; }
        public string MessageText { get; set; }
        public string DateText { get; set; }
        public bool IsNew { get; set; }
        public long Id { get; set; }
    }
}
