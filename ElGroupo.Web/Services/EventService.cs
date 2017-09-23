using ElGroupo.Domain;
using ElGroupo.Domain.Data;
using ElGroupo.Web.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ElGroupo.Web.Classes;
using ElGroupo.Web.Models.Messages;
using ElGroupo.Web.Models.Notifications;

namespace ElGroupo.Web.Services
{
    public class EventService
    {
        private readonly ElGroupoDbContext dbContext;
        private readonly MailService mailService;
        private UserManager<User> userManager;
        public EventService(ElGroupoDbContext ctx, MailService service, UserManager<User> manager)
        {
            this.dbContext = ctx;
            this.mailService = service;
            this.userManager = manager;
        }

        public async Task<Guid> AddUnregisteredAttendee(User u, Event e, UnregisteredEventAttendeeModel model)
        {
            var uea = new UnregisteredEventAttendee
            {
                Event = e,
                Name = model.Name,
                Email = model.Email,
                DateCreated = DateTime.Now,
                UserCreated = u.UserName,
                RegisterToken = Guid.NewGuid(),

            };
            dbContext.UnregisteredEventAttendees.Add(uea);
            await dbContext.SaveChangesAsync();
            return uea.RegisterToken;
        }
        public async Task<bool> DeleteEvent(long eventId)
        {
            try
            {
                var e = await dbContext.Set<Event>().Include("Attendees").Include("UnregisteredAttendees").Include("MessageBoardItems.Attendees").Include("Organizers").Include("Notifications.Attendees").FirstOrDefaultAsync();
                if (e == null) return false;
                dbContext.Events.Remove(e);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        //public async Task<List<EventOrganizerModel>> GetEventOrganizers(long eventId)
        //{
        //    var orgs = await dbContext.Events.Include("Organizers.User").FirstOrDefaultAsync(x => x.Id == eventId);
        //    if (orgs == null) throw new Exception("Event Not Found");
        //    var model = new List<EventOrganizerModel>();
        //    orgs.Organizers.ToList().ForEach(x => model.Add(new EventOrganizerModel { Id = x.Id, Name = x.User.Name, Owner = x.Owner, PhoneNumber = x.User.PhoneNumber, UserId = x.User.Id }));
        //    return model;
        //}

        public async Task<EventContactsModel> GetEventContacts(long eventId, int userId)
        {

            var e = await dbContext.Events.Include("Organizers.User").Include("Attendees.User").FirstOrDefaultAsync(x => x.Id == eventId);
            var model = new EventContactsModel();
            model.Event.Name = e.Name;
            model.Event.EndDate = e.EndTime;
            model.Event.StartDate = e.StartTime;
            model.Event.Id = e.Id;


            //foreach (var att in e.Organizers.Where(x => x.User.Id != userId))
            //{
            //    model.Organizers.Add(new EventOrganizerModel
            //    {
            //        Name = att.User.Name,
            //        Id = att.Id,
            //        Owner = att.Owner,
            //        UserId = att.User.Id,
            //        PhoneNumber = att.User.PhoneNumber
            //    });
            //}

            foreach (var att in e.Attendees)
            {
                model.Attendees.Add(new EventAttendeeModel
                {
                    Name = att.User.Name,
                    Id = att.Id,
                    RSVPStatus = att.ResponseStatus,
                    UserId = att.User.Id,
                    PhoneNumber = att.User.PhoneNumber
                });
            }
            return model;
        }

        public async Task<long> CreateEvent(CreateEventModel model, User user)
        {

            var e = new Event();


            //we need this for the owners of the event to get messages
            e.Attendees.Add(new EventAttendee
            {
                User = user,
                Event = e,
                IsOrganizer = true
            });
            e.Name = model.Name;
            e.Description = model.Description;
            e.LocationName = model.LocationName;
            e.CheckInTimeTolerance = 30;
            e.CheckInLocationTolerance = 500;
            e.CoordinateX = model.XCoord;
            e.CoordinateY = model.YCoord;
            e.GooglePlaceId = model.GooglePlaceId;
            e.Address1 = model.Address1;
            e.Address2 = model.Address2;
            e.City = model.City;
            e.SavedAsDraft = true;
            e.State = model.State;
            e.Zip = model.ZipCode;

            int startHour = 0;
            int endHour = 0;

            if (model.StartHour == 12) startHour = model.StartAMPM == Models.Enums.AMPM.AM ? 0 : 12;
            else startHour = model.StartAMPM == Models.Enums.AMPM.AM ? model.StartHour : model.StartHour + 12;

            if (model.EndHour == 12) endHour = model.EndAMPM == Models.Enums.AMPM.AM ? 0 : 12;
            else endHour = model.EndAMPM == Models.Enums.AMPM.AM ? model.EndHour : model.EndHour + 12;


            if (model.StartAMPM == Models.Enums.AMPM.AM && model.StartHour == 12) startHour = 0;
            else if (model.StartAMPM == Models.Enums.AMPM.AM) startHour = model.StartHour;
            else if (model.StartAMPM == Models.Enums.AMPM.PM && startHour == 12) startHour = 12;
            else startHour = model.StartHour + 12;


            e.StartTime = new DateTime(model.EventDate.Year, model.EventDate.Month, model.EventDate.Day, startHour, model.StartMinute, 0);
            e.EndTime = new DateTime(model.EventDate.Year, model.EventDate.Month, model.EventDate.Day, endHour, model.EndMinute, 0);

            dbContext.Events.Add(e);

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var fff = 4;
            }

            return e.Id;
        }

        public async Task<EventEditModel> GetEventEditModel(long eid)
        {
            var e = await dbContext.Events.Include("Organizers.User").FirstOrDefaultAsync(x => x.Id == eid);
            var model = new EventEditModel();
            model.Id = e.Id;
            model.Address1 = e.Address1;
            model.Address2 = e.Address2;
            model.City = e.City;
            model.Description = e.Description;
            model.EndAMPM = e.EndTime.Hour >= 12 ? Models.Enums.AMPM.PM : Models.Enums.AMPM.AM;
            model.EndHour = e.EndTime.Hour;
            model.EndMinute = e.EndTime.Minute;
            model.EventDate = e.StartTime.Date;
            model.GooglePlaceId = e.GooglePlaceId;
            model.LocationName = e.LocationName;
            model.Name = e.Name;
            model.StartAMPM = e.StartTime.Hour >= 12 ? Models.Enums.AMPM.PM : Models.Enums.AMPM.AM;
            model.StartHour = e.StartTime.Hour;
            model.StartMinute = e.StartTime.Minute;
            model.State = e.State;
            model.XCoord = e.CoordinateX;
            model.YCoord = e.CoordinateY;
            model.ZipCode = e.Zip;
            return model;
        }


        public async Task<EventViewModel> GetEventViewModel(long eventId, int userId, EditAccessTypes accessLevel)
        {

            var e = await dbContext.Events.Include("Attendees.User").Include("Attendees.MessageBoardItems.MessageBoardItem.PostedBy").FirstOrDefaultAsync(x => x.Id == eventId);
            var thisAttendee = e.Attendees.FirstOrDefault(x => x.User.Id == userId);
            //var isOrganizer = e.Organizers.Any(x => x.User.Id == user.Id);
            var model = new EventViewModel();
            model.EventId = e.Id;
            model.Address1 = e.Address1;
            model.Address2 = e.Address2;
            model.City = e.City;
            model.Description = e.Description;
            model.StartDateText = e.StartTime.ToString("d") + " " + e.StartTime.ToString("t");
            model.EndDateText = e.EndTime.ToString("d") + " " + e.EndTime.ToString("t");
            model.GooglePlaceId = e.GooglePlaceId;
            model.LocationName = e.LocationName;
            model.Name = e.Name;
            model.State = e.State;
            model.ZipCode = e.Zip;
            model.IsOrganizer = accessLevel == EditAccessTypes.Edit;
            model.Attendees = new List<EventAttendeeModel>();
            foreach (var att in e.Attendees)
            {
                model.Attendees.Add(new EventAttendeeModel
                {
                    Id = att.Id,
                    UserId = att.UserId,
                    RSVPStatus = att.ResponseStatus,
                    Name = att.User.Name
                });
            }

            model.Messages = new List<EventMessageModel>();

            foreach (var mba in thisAttendee.MessageBoardItems)
            {
                model.Messages.Add(new EventMessageModel
                {
                    PostedBy = mba.MessageBoardItem.PostedBy.Name,
                    PostedById = mba.MessageBoardItem.PostedBy.Id,
                    PostedDate = mba.MessageBoardItem.PostedDate,
                    Subject = mba.MessageBoardItem.Subject,
                    MessageText = mba.MessageBoardItem.MessageText,
                    IsNew = !mba.Viewed,
                    Id = mba.Id
                });
            }
            model.Messages = model.Messages.OrderByDescending(x => x.PostedBy).ToList();


            model.Notifications = new List<EventNotificationModel>();
            foreach (var ean in this.dbContext.EventAttendeeNotifications.Include("Notification.PostedBy.User").Where(x => x.AttendeeId == thisAttendee.Id))
            {
                model.Notifications.Add(new EventNotificationModel
                {
                    OrganizerName = ean.Notification.PostedBy.User.Name,
                    OrganizerId = ean.Notification.PostedBy.User.Id,
                    CanEdit = accessLevel == EditAccessTypes.Edit,
                    PostedDate = ean.Notification.PostedDate,
                    Subject = ean.Notification.Subject,
                    NotificationText = ean.Notification.MessageText,
                    IsNew = !ean.Viewed,
                    Id = ean.Id
                });
            }

            return model;
        }

        //public async Task AddEventAttendee(long eventId, int userId, bool asOwner)
        //{
        //    var e = dbContext.Events.FirstOrDefault(x => x.Id == eventId);
        //    var u = dbContext.Users.FirstOrDefault(x => x.Id == userId);
        //    var eo = new EventOrganizer { User = u, Event = e, Owner = asOwner };
        //    dbContext.EventOrganizers.Add(eo);
        //    await dbContext.SaveChangesAsync();
        //}

        public async Task AddEventAttendee(long eventId, long userId)
        {
            var e = dbContext.Events.FirstOrDefault(x => x.Id == eventId);
            var u = dbContext.Users.FirstOrDefault(x => x.Id == userId);
            var ea = new EventAttendee
            {
                Event = e,
                User = u,
                DateCreated = DateTime.Now,
                UserCreated = u.UserName,
                AllowEventUpdates = true,
                Viewed = false
            };
            dbContext.EventAttendees.Add(ea);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<EventAttendeeModel>> GetEventAttendees(long eventId)
        {
            var atts = await dbContext.Events.Include("Attendees.User").Include("UnregisteredAttendees").FirstOrDefaultAsync(x => x.Id == eventId);
            if (atts == null) throw new Exception("Event Not Found");
            var model = new List<EventAttendeeModel>();
            atts.Attendees.ToList().ForEach(x => model.Add(new EventAttendeeModel { Id = x.Id, Name = x.User.Name, PhoneNumber = x.User.PhoneNumber, UserId = x.User.Id, RSVPStatus = x.ResponseStatus }));
            atts.UnregisteredAttendees.ToList().ForEach(x => model.Add(new EventAttendeeModel { Name = x.Name, Email = x.Email, RSVPStatus = Domain.Enums.RSVPTypes.PendingRegistration }));
            return model;
        }

        public async Task<bool> DeleteEventAttendee(long attendee, long eventId)
        {
            var eo = dbContext.EventAttendees.FirstOrDefault(x => x.Id == attendee);
            if (eo.EventId != eventId) return false;
            dbContext.EventAttendees.Remove(eo);
            await dbContext.SaveChangesAsync();
            return true;
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
                OrganizerName = x.Attendees.First(y => y.IsOrganizer).User.Name
            }));
            return list;
        }
    }
}
