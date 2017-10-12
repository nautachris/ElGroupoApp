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
        private readonly IEmailService mailService;
        private UserManager<User> userManager;
        public EventService(ElGroupoDbContext ctx, IEmailService service, UserManager<User> manager)
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

                //var fff =  await dbContext.Set<Event>().Include(x => x.MessageBoardItems).ThenInclude(y => y.Attendees).FirstOrDefaultAsync();
                var e = await dbContext.Set<Event>().Include("Attendees").Include("UnregisteredAttendees").Include("MessageBoardItems.Attendees").Include("Notifications.Attendees").FirstOrDefaultAsync();
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

        public async Task<EventContactsModel> GetEventContacts(long eventId, long userId)
        {

            var e = await dbContext.Events.Include("Attendees.User").FirstOrDefaultAsync(x => x.Id == eventId);
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
            if (model.LocationTolerance.HasValue) e.CheckInLocationTolerance = model.LocationTolerance.Value;
            e.VerificationCode = model.VerificationCode;
            e.VerificationMethod = model.AttendanceVerificationMethod;
            e.MustRSVP = model.RSVPRequired;
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
            var e = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eid);
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
        public async Task<ViewEventDetailsModel> ViewEventDetails(long eventId)
        {
            var e = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            var model = new ViewEventDetailsModel(e);
            return model;
        }
        public async Task<ViewEventLocationModel> ViewEventLocationDetails(long eventId)
        {
            var e = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            var model = new ViewEventLocationModel(e);
            return model;
        }

        public async Task<EditEventDetailsModel> EditEventDetails(long eventId)
        {
            var e = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            var model = new EditEventDetailsModel(e);
            return model;
        }

        public async Task<EditEventLocationModel> EditEventLocation(long eventId)
        {
            var e = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            var model = new EditEventLocationModel(e);
            return model;
        }


        private List<string> FindChangedEventDetails(EditEventDetailsModel model, Event e)
        {
            var changes = new List<string>();
            if (e.Name != model.Name) changes.Add("Name");
            if (model.AttendanceVerificationMethod != e.VerificationMethod) changes.Add("Verification Method");
            if (model.Description != e.Description) changes.Add("Description");
            if (model.RSVPRequired != e.MustRSVP) changes.Add("RSVP Required");

            int startHour, endHour;
            if (model.StartHour == 12) startHour = model.StartAMPM == Models.Enums.AMPM.AM ? 0 : 12;
            else startHour = model.StartAMPM == Models.Enums.AMPM.AM ? model.StartHour : model.StartHour + 12;

            if (model.EndHour == 12) endHour = model.EndAMPM == Models.Enums.AMPM.AM ? 0 : 12;
            else endHour = model.EndAMPM == Models.Enums.AMPM.AM ? model.EndHour : model.EndHour + 12;


            if (model.StartAMPM == Models.Enums.AMPM.AM && model.StartHour == 12) startHour = 0;
            else if (model.StartAMPM == Models.Enums.AMPM.AM) startHour = model.StartHour;
            else if (model.StartAMPM == Models.Enums.AMPM.PM && startHour == 12) startHour = 12;
            else startHour = model.StartHour + 12;

            var modelStartTime = new DateTime(model.EventDate.Year, model.EventDate.Month, model.EventDate.Day, startHour, model.StartMinute, 0);
            var modelEndTime = new DateTime(model.EventDate.Year, model.EventDate.Month, model.EventDate.Day, endHour, model.EndMinute, 0);

            if (modelStartTime != e.StartTime) changes.Add("Start Time");
            if (modelEndTime != e.EndTime) changes.Add("End Time");

            return changes;
        }
        public async Task<bool> UpdateEventDetails(EditEventDetailsModel model)
        {
            var e = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == model.EventId);
            var changes = FindChangedEventDetails(model, e);
            var draftChange = !model.IsDraft && e.SavedAsDraft;

            e.Name = model.Name;
            e.Description = model.Description;
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

            dbContext.Update(e);
            await dbContext.SaveChangesAsync();


            if (draftChange)
            {
                //send out all emails
                foreach (var att in dbContext.EventAttendees.Where(x => x.EventId == e.Id))
                {

                }

            }
            else if (!e.SavedAsDraft && changes.Count > 0)
            {
                //details have changed, must send out emails
            }

            return true;
        }

        public async Task<bool> UpdateEventLocation(EditEventLocationModel model)
        {
            var e = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == model.EventId);
            e.Address1 = model.Address1;
            e.Address2 = model.Address2;
            e.City = model.City;
            e.State = model.State;
            e.Zip = model.ZipCode;
            e.GooglePlaceId = model.GooglePlaceId;
            e.CoordinateX = model.XCoord;
            e.CoordinateY = model.YCoord;
            dbContext.Update(e);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<EventViewModel> GetEventViewModel(long eventId, long userId, EditAccessTypes accessLevel)
        {

            var e = await dbContext.Events.Include("Attendees.User").Include("Attendees.MessageBoardItems.MessageBoardItem.PostedBy").FirstOrDefaultAsync(x => x.Id == eventId);
            var thisAttendee = e.Attendees.FirstOrDefault(x => x.User.Id == userId);
            //var isOrganizer = e.Organizers.Any(x => x.User.Id == user.Id);
            var model = new EventViewModel(e);
            model.IsOrganizer = accessLevel == EditAccessTypes.Edit;
            model.Attendees = new List<EventAttendeeModel>();
            foreach (var att in e.Attendees)
            {
                model.Attendees.Add(new EventAttendeeModel
                {
                    Id = att.Id,
                    UserId = att.User.Id,
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

        public async Task AddEventAttendee(AddRegisteredEventAttendeeModel model)
        {
            var e = dbContext.Events.FirstOrDefault(x => x.Id == model.eventId);
            var u = dbContext.Users.FirstOrDefault(x => x.Id == model.userId);
            var ea = new EventAttendee
            {
                Event = e,
                User = u,
                DateCreated = DateTime.Now,
                UserCreated = u.UserName,
                AllowEventUpdates = true,
                Viewed = false,
                IsOrganizer = model.isOwner
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

        public async Task<List<EventInformationModel>> SearchEvents(string search, long userId)
        {

            IQueryable<Event> events = null;
            if (search == "*")
            {
                events = dbContext.Events.Include("Attendees.User");
            }
            else
            {
                events = dbContext.Events.Include("Attendees.User").Where(x => x.Name.ToUpper().Contains(search.ToUpper()));
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
