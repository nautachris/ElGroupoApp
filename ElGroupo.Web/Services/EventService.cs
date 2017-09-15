using ElGroupo.Domain;
using ElGroupo.Domain.Data;
using ElGroupo.Web.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace ElGroupo.Web.Services
{
    public class EventService
    {
        private readonly ElGroupoDbContext dbContext;
        public EventService(ElGroupoDbContext ctx)
        {
            this.dbContext = ctx;
        }

        public async Task<List<EventInformationModel>> SearchEvents(string search, int userId)
        {
            IQueryable<Event> events = null;
            if (search == "*")
            {
                events = dbContext.Events.Include("Organizers.User");
            }
            else
            {
                events = dbContext.Events.Include("Organizers.User").Where(x => x.Name.ToUpper().Contains(search.ToUpper()));
            }

            var list = new List<EventInformationModel>();
            await events.ForEachAsync(x => list.Add(new EventInformationModel
            {
                Draft = x.SavedAsDraft,
                StartDate = x.StartTime,
                EndDate = x.EndTime,
                Id = x.Id,
                Name = x.Name,
                OrganizerName = x.Organizers.First(y => y.Owner).User.Name
            }));
            return list;
        }
    }
}
