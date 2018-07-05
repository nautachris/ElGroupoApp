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
using ElGroupo.Domain.Enums;
using ElGroupo.Web.Models.Messages;
using ElGroupo.Web.Models.Notifications;
using ElGroupo.Web.Models.Shared;
using ElGroupo.Web.Models.Configuration;
using Microsoft.Extensions.Options;

namespace ElGroupo.Web.Services
{
    public class EventService : BaseService
    {

        private readonly IEmailService mailService;
        private UserManager<User> userManager;
        private GoogleConfigOptions googleOptions;
        public EventService(ElGroupoDbContext ctx, IEmailService service, UserManager<User> manager, IOptions<GoogleConfigOptions> googConfig) : base(ctx)
        {
            this._dbContext = ctx;
            this.mailService = service;
            this.userManager = manager;
            this.googleOptions = googConfig.Value;
        }

        //public async Task<EditAccessTypes> CheckEventAccess(ClaimsPrincipal principal, long eventId)
        //{

        //    if (principal.IsInRole("admin")) return EditAccessTypes.Edit;

        //    var user = await this.userManager.GetUserAsync(principal);
        //    if (await this._dbContext.EventAttendees.AnyAsync(x => x.User.Id == user.Id && x.EventId == eventId && x.IsOrganizer)) return EditAccessTypes.Edit;
        //    if (await this._dbContext.EventAttendees.AnyAsync(x => x.User.Id == user.Id && x.EventId == eventId)) return EditAccessTypes.View;
        //    return EditAccessTypes.None;
        //}

        public CreateEventModel GetCreateEventModel()
        {
            var model = new CreateEventModel();
            model.GoogleApiKey = this.googleOptions.GoogleMapsApiKey;
            return model;
        }

        public EventDashboardModel GetDashboardModel(long userId, string userTimeZone)
        {
            var model = new EventDashboardModel();
            var allEvents = new List<EventInformationModel>();
            //do we want a third "tab" for events I'm organizing?

            var organizedEvents = _dbContext.EventAttendees.Include(x => x.User).Where(x => x.User.Id == userId && x.Active == true && x.IsOrganizer).Select(x =>
            new
            {
                ea = x,
                ev = x.Event,
                rec = x.Event.Recurrence
            }).ToList();
            //e.StartTime.ToLocalTime().DayOfWeek.ToString() + " " + e.StartTime.ToLocalTime().ToString("d") + " " + e.StartTime.ToLocalTime().ToString("t")
            var invitedEvents = _dbContext.EventAttendees.Include(x => x.User).Include(x => x.Event).ThenInclude(x => x.Attendees).ThenInclude(x => x.User).Include(x => x.Event).ThenInclude(x => x.Recurrence).Where(x => x.User.Id == userId && x.Active == true && !x.IsOrganizer).ToList();
            foreach (var ev in organizedEvents)
            {
                allEvents.Add(new EventInformationModel
                {
                    EventAttendeeId = ev.ea.Id,
                    EndDate = ev.ev.EndTime,
                    StartDate = ev.ev.StartTime,
                    DateText = ev.ev.GetSimpleDateText(userTimeZone),
                    Id = ev.ev.Id,
                    CheckInStatus = GetCheckInStatus(ev.ev, ev.ea),
                    OrganizedByUser = true,
                    IsNew = false,
                    RecurrenceId = ev.rec?.Id,
                    RSVPStatus = ev.ea.ResponseStatus,
                    Name = ev.ev.Name,
                    Status = ev.ev.Status,
                    IsRecurring = ev.rec != null

                });
            }
            foreach (var ev in invitedEvents.Where(x => !organizedEvents.Select(y => y.ev.Id).Contains(x.EventId)))
            {
                allEvents.Add(new EventInformationModel
                {
                    EventAttendeeId = ev.Id,
                    EndDate = ev.Event.EndTime,
                    StartDate = ev.Event.StartTime,
                    DateText = ev.Event.GetSimpleDateText(userTimeZone),
                    RecurrenceId = ev.Event.Recurrence?.Id,
                    Id = ev.Event.Id,
                    OrganizedByUser = false,
                    CheckInStatus = GetCheckInStatus(ev.Event, ev),
                    IsNew = !ev.Viewed,
                    Name = ev.Event.Name,
                    OrganizerName = ev.Event.Attendees.First(x => x.IsOrganizer).User.Name,
                    Status = ev.Event.Status,
                    RSVPStatus = ev.ResponseStatus,
                    IsRecurring = ev.Event.Recurrence != null,
                    RSVPRequested = ev.ShowRSVPReminder == true
                });
            }

            var drafts = allEvents.Where(x => x.OrganizedByUser && x.Status == EventStatus.Draft).OrderBy(x => x.StartDate).ToList();
            var pastEvents = allEvents.Where(x => x.Status != EventStatus.Draft && x.EndDate < DateTime.Now.ToUniversalTime()).OrderBy(x => x.StartDate).ToList();
            var futureEvents = allEvents.Where(x => x.Status != EventStatus.Draft && x.EndDate >= DateTime.Now.ToUniversalTime()).OrderBy(x => x.StartDate).ToList();
            var myEvents = allEvents.Where(x => x.OrganizedByUser).ToList();

            model.Drafts = GroupEventsByRecurrence(drafts);
            model.PastEvents = GroupEventsByRecurrence(pastEvents);
            model.FutureEvents = GroupEventsByRecurrence(futureEvents);
            model.MyEvents = GroupEventsByRecurrence(myEvents);

            model.RSVPRequestedEvents = allEvents.Where(x => x.RSVPRequested).ToList();
            model.RSVPRequestCount = allEvents.Count(x => x.RSVPRequested);
            return model;
        }
        private List<EventInformationModel> GroupEventsByRecurrence(List<EventInformationModel> events)
        {
            //var recurring = events.Where(x => x.RecurrenceId.HasValue);
            var outList = new List<EventInformationModel>();
            outList.AddRange(events.Where(x => !x.RecurrenceId.HasValue));
            foreach (var ev in events.Where(x => x.RecurrenceId.HasValue).GroupBy(x => x.RecurrenceId.Value))
            {
                var parentEvent = ev.OrderBy(x => x.StartDate).First();
                parentEvent.Recurrences = ev.OrderBy(x => x.StartDate).Skip(1).ToList();
                foreach (var r in parentEvent.Recurrences) r.IsRecurrenceItem = true;
                outList.Add(parentEvent);
            }

            return outList.OrderBy(x => x.StartDate).ToList();
        }
        public async Task<(EditAccessTypes accessType, long userId)> CheckEventAccess(ClaimsPrincipal principal, long eventId)
        {



            var user = await this.userManager.GetUserAsync(principal);
            if (principal.IsInRole("admin")) return (EditAccessTypes.Edit, user.Id);
            if (await this._dbContext.EventAttendees.AnyAsync(x => x.User.Id == user.Id && x.EventId == eventId && x.IsOrganizer)) return (EditAccessTypes.Edit, user.Id);
            if (await this._dbContext.EventAttendees.AnyAsync(x => x.User.Id == user.Id && x.EventId == eventId)) return (EditAccessTypes.View, user.Id);
            return (EditAccessTypes.None, user.Id);
        }


        public async Task<EditAccessTypes> CheckEventAccess(ClaimsPrincipal principal, long eventId, long userId)
        {
            if (principal.IsInRole("admin")) return EditAccessTypes.Edit;
            if (await this._dbContext.EventAttendees.AnyAsync(x => x.User.Id == userId && x.EventId == eventId && x.IsOrganizer)) return EditAccessTypes.Edit;
            if (await this._dbContext.EventAttendees.AnyAsync(x => x.User.Id == userId && x.EventId == eventId)) return EditAccessTypes.View;
            return EditAccessTypes.None;
        }
        public async Task<SaveDataResponse> SetEventAttendeesInactive(long[] id)
        {
            var ea = _dbContext.EventAttendees.Where(x => id.Contains(x.Id));
            if (ea.Count() == 0) return SaveDataResponse.FromErrorMessage("Event Attendee Id Not found");
            foreach (var e in ea)
            {
                e.Active = false;
                _dbContext.Update(e);
            }

            await _dbContext.SaveChangesAsync();
            return SaveDataResponse.Ok();
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

            var e = await _dbContext.Events.Include("Attendees.User").FirstOrDefaultAsync(x => x.Id == eventId);
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
                    Name = att.User.FirstName,
                    EventAttendeeId = att.Id,
                    RSVPStatus = att.ResponseStatus,
                    UserId = att.User.Id,
                    PhoneNumber = att.User.PhoneNumber
                });
            }
            return model;
        }

        public async Task<SaveDataResponse> DeleteRecurringEvent(long eventId)
        {

            var e = _dbContext.Events.Include(x => x.Recurrence).ThenInclude(x => x.Events).FirstOrDefault(x => x.Id == eventId);
            if (e == null) return SaveDataResponse.FromErrorMessage("Event Id " + eventId + " not found");
            var allids = e.Recurrence.Events.Select(x => x.Id).ToList();
            allids.Add(eventId);
            try
            {
                var toDelete = await _dbContext.Events.
                    Include(x => x.Recurrence).
                    Include(x => x.Attendees).ThenInclude(x => x.MessageBoardItems).
                    Include(x => x.MessageBoardTopics).ThenInclude(x => x.Messages).
                    Include(x => x.Notifications).
                    Include(x => x.UnregisteredAttendees).Where(x => allids.Contains(x.Id)).ToListAsync();
                foreach (var del in toDelete) _dbContext.Remove(del);


                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }



        }

        public async Task<SaveDataResponse> DeleteEvent(long eventId)
        {
            var toDelete = _dbContext.Events.
                    Include(x => x.Recurrence).
                    Include(x => x.Attendees).ThenInclude(x => x.MessageBoardItems).
                    Include(x => x.MessageBoardTopics).
                    Include(x => x.Notifications).
                    Include(x => x.UnregisteredAttendees).FirstOrDefault(x => x.Id == eventId);
            if (toDelete == null) return SaveDataResponse.FromErrorMessage("Event Id " + eventId + " not found");

            try
            {
                _dbContext.Remove(toDelete);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();

            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }

            //var e = _dbContext.Events.FirstOrDefault(x => x.Id == eventId);

            //if (e == null) return SaveDataResponse.FromErrorMessage("Event Id " + eventId + " not found");

            //try
            //{
            //    var unregisteredEventAttendees = _dbContext.UnregisteredEventAttendees.Where(x => x.EventId == eventId).ToList();
            //    foreach (var u in unregisteredEventAttendees) _dbContext.Remove(unregisteredEventAttendees);

            //    var mbias = _dbContext.MessageBoardItemAttendees.Include(x => x.Attendee).ThenInclude(x => x.Event).Where(x => x.Attendee.Event.Id == eventId).ToList();
            //    foreach (var mbia in mbias) _dbContext.Remove(mbia);

            //    var mbis = _dbContext.MessageBoardItems.Include(x => x.Event).Where(x => x.Event.Id == eventId).ToList();
            //    foreach (var mbi in mbis) _dbContext.Remove(mbi);

            //    var eatns = _dbContext.EventAttendeeNotifications.Include(x => x.Attendee).ThenInclude(x => x.Event).Where(x => x.Attendee.Event.Id == eventId).ToList();
            //    foreach (var eatn in eatns) _dbContext.Remove(eatn);

            //    var eans = _dbContext.EventAttendees.Include(x => x.Event).Where(x => x.Event.Id == eventId).ToList();
            //    foreach (var ean in eans) _dbContext.Remove(ean);

            //    await _dbContext.SaveChangesAsync();
            //    return SaveDataResponse.Ok();
            //}
            //catch (Exception ex)
            //{
            //    return SaveDataResponse.FromException(ex);
            //}



        }
        public async Task<SaveDataResponse> UpdateRecurringEventAttendees(UpdateEventAttendeesModel model)
        {
            try
            {
                var originalEvent = await _dbContext.Events.Include(x => x.Attendees).ThenInclude(x => x.User).Include(x => x.Recurrence).FirstOrDefaultAsync(x => x.Id == model.EventId);

                if (originalEvent == null) return new SaveDataResponse { Success = false, ErrorMessage = "Could not find event with id = " + model.EventId };
                bool changesMade = false;
                foreach (var recurringEvent in _dbContext.Events.Include(x => x.Attendees).ThenInclude(x => x.User).Include(x => x.UnregisteredAttendees).Include(x => x.Recurrence).Where(x => x.Recurrence.Id == originalEvent.Recurrence.Id))
                {
                    bool currentChangeMade = false;
                    if (recurringEvent.Id != model.EventId)
                    {
                        var updatedAttendees = new List<EventAttendeeModel>();
                        foreach (var att in model.Attendees)
                        {
                            if (att.EventAttendeeId.HasValue)
                            {
                                var originalUserId = originalEvent.Attendees.First(x => x.Id == att.EventAttendeeId.Value).User.Id;
                                if (recurringEvent.Attendees.Any(x => x.User.Id == originalUserId))
                                {
                                    updatedAttendees.Add(new EventAttendeeModel
                                    {
                                        EventAttendeeId = recurringEvent.Attendees.First(x => x.User.Id == originalUserId).Id,
                                        IsOrganizer = att.IsOrganizer
                                    });
                                }
                                //if there is no matching user in this recurreing event, we're not going to add it b/c the user didn't explicitly add
                                //the user
                            }
                            else
                            {
                                //adds 
                                updatedAttendees.Add(att);
                            }
                        }
                        currentChangeMade = await ProcessEventAttendeeChanges(recurringEvent, updatedAttendees.ToArray());

                    }
                    else
                    {
                        currentChangeMade = await ProcessEventAttendeeChanges(recurringEvent, model.Attendees);

                    }
                    if (!changesMade && currentChangeMade) changesMade = true;
                }
                if (changesMade)
                {
                    await _dbContext.SaveChangesAsync();
                }
                return new SaveDataResponse { Success = true };
            }
            catch (Exception ex)
            {
                return new SaveDataResponse { Success = false, ErrorMessage = ex.ToString() };
            }


        }

        public async Task<SaveDataResponse> RemoveRecurringEventAttendee(RemoveAttendeeModel model)
        {
            try
            {
                var e = await _dbContext.Events.FirstAsync(x => x.Id == model.EventId);
                var rec = _dbContext.RecurringEvents.Include(x => x.Events).First(x => x.Id == e.RecurrenceId.Value).Events.Select(x => x.Id).ToList();

                var eas = _dbContext.EventAttendees.Include(x => x.Event).Include(x => x.User).Where(x => rec.Contains(x.Event.Id) && x.User.Id == model.UserId).ToList();

                foreach (var ea in eas)
                {
                    _dbContext.Remove(ea);
                }
                await _dbContext.SaveChangesAsync();
                return new SaveDataResponse { Success = true };
            }
            catch (Exception ex)
            {
                return new SaveDataResponse { Success = false, ErrorMessage = ex.ToString() };
            }


        }
        public async Task<SaveDataResponse> RemoveEventAttendee(RemoveAttendeeModel model)
        {
            try
            {
                var ea = await _dbContext.EventAttendees.FirstOrDefaultAsync(x => x.Event.Id == model.EventId && x.User.Id == model.UserId);
                _dbContext.Remove(ea);
                await _dbContext.SaveChangesAsync();
                return new SaveDataResponse { Success = true };
            }
            catch (Exception ex)
            {
                return new SaveDataResponse { Success = false, ErrorMessage = ex.ToString() };
            }


        }

        public async Task<Event> GetEvent(long id)
        {
            return await _dbContext.Events.Include(x => x.Attendees).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="model"></param>
        /// <returns>returns true if any information is changed, false if no action is taken</returns>
        private async Task<bool> UpdateEventAttendee(Event e, EventAttendeeModel model)
        {
            if (model.EventAttendeeId.HasValue)
            {
                //what if we're passing in an event from the original event recurrence?  the eventAttendeeId will not be the same
                //only updating 
                var ea = e.Attendees.First(x => x.Id == model.EventAttendeeId.Value);
                if (ea.IsOrganizer != model.IsOrganizer)
                {
                    ea.IsOrganizer = model.IsOrganizer;
                    _dbContext.Update(ea);
                    return true;
                }
                return false;
            }
            else if (model.UserId.HasValue)
            {
                if (!e.Attendees.Any(x => x.User.Id == model.UserId.Value))
                {
                    var newAttendee = new EventAttendee
                    {
                        User = _dbContext.Find<User>(model.UserId.Value),
                        Event = e,
                        IsOrganizer = model.IsOrganizer,
                        ResponseStatus = Domain.Enums.RSVPTypes.None
                    };
                    _dbContext.Add(newAttendee);
                }
                return true;
            }
            else if (model.UserGroupId.HasValue)
            {
                //adding entire user group
                var attGroup = await _dbContext.AttendeeGroups.Include(x => x.Attendees).ThenInclude(x => x.User).FirstAsync(x => x.Id == model.UserGroupId.Value);
                foreach (var user in attGroup.Attendees)
                {
                    var newAttendee = new EventAttendee
                    {
                        User = user.User,
                        Event = e,
                        IsOrganizer = model.IsOrganizer,
                        ResponseStatus = Domain.Enums.RSVPTypes.None
                    };
                    _dbContext.Add(newAttendee);
                    if (e.Status == Domain.Enums.EventStatus.Active)
                    {
                        //send email
                    }
                }
                return true;
            }
            else if (!model.IsRegistered)
            {
                //new invitee - check if this has already been added
                if (!e.UnregisteredAttendees.Any(x => x.Email == model.Email && x.Name == model.Name))
                {
                    var emailCheck = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
                    if (emailCheck != null)
                    {
                        //email entered is already in the system
                        var newAttendee = new EventAttendee
                        {
                            User = emailCheck,
                            Event = e,
                            IsOrganizer = model.IsOrganizer,
                            ResponseStatus = Domain.Enums.RSVPTypes.None

                        };
                        _dbContext.Add(newAttendee);
                    }
                    else
                    {
                        var unregisteredAttendee = new UnregisteredEventAttendee
                        {
                            Name = model.Name,
                            Email = model.Email,
                            Event = e,
                            RegisterToken = Guid.NewGuid(),
                        };
                        _dbContext.Add(unregisteredAttendee);
                        if (e.Status == Domain.Enums.EventStatus.Active)
                        {
                            //send email
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }


        }


        private async Task<bool> ProcessEventAttendeeChanges(Event e, EventAttendeeModel[] attendees)
        {
            try
            {
                bool foundChanges = false;                
                var modelEventAttendeeIds = attendees.Where(x => x.EventAttendeeId.HasValue).Select(x => x.EventAttendeeId.Value).ToList();
                var eventAttendeeIdsToDelete = e.Attendees.Where(x => !modelEventAttendeeIds.Contains(x.Id)).Select(x => x.Id).ToList();
                if (eventAttendeeIdsToDelete.Count > 0)
                {
                    foundChanges = true;
                    foreach (var eaToDelete in _dbContext.EventAttendees.Where(x => eventAttendeeIdsToDelete.Contains(x.Id)))
                    {
                        _dbContext.Remove(eaToDelete);
                    }
                }

                foreach (var att in attendees)
                {
                    bool changeFound = await UpdateEventAttendee(e, att);
                    if (!foundChanges && changeFound) foundChanges = true;
                }
                //what if they delete unregistered attendees?
                foreach (var unregEmail in e.UnregisteredAttendees.Select(x => x.Email))
                {
                    if (!attendees.Any(x => !x.IsRegistered && x.Email == unregEmail))
                    {
                        //remove this unregistered
                        var toDelete = _dbContext.UnregisteredEventAttendees.Include(x => x.Event).FirstOrDefault(x => x.Event.Id == e.Id && x.Email == unregEmail);
                        if (toDelete != null)
                        {
                            foundChanges = true;
                            _dbContext.Remove(toDelete);
                        }
                    }
                }
                return foundChanges;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public async Task<SaveDataResponse> UpdateEventAttendees(UpdateEventAttendeesModel model)
        {
            try
            {
                var e = _dbContext.Events.Include(x => x.Attendees).ThenInclude(x => x.User).Include(x => x.UnregisteredAttendees).FirstOrDefault(x => x.Id == model.EventId);
                if (e == null)
                {
                    return new SaveDataResponse
                    {
                        Success = false,
                        ErrorMessage = "could not find event with id = " + model.EventId
                    };
                }
                bool foundChanges = await ProcessEventAttendeeChanges(e, model.Attendees);

                //look for deletes


                if (foundChanges)
                {
                    await _dbContext.SaveChangesAsync();
                }

                if (e.Status == Domain.Enums.EventStatus.Active)
                {
                    //send emails
                }
                return new SaveDataResponse { Success = true };
            }
            catch (Exception ex)
            {
                return new SaveDataResponse { Success = false, ErrorMessage = ex.ToString() };
            }
        }

        public async Task<SaveDataResponse> UpdateRecurringRSVP(User user, UpdateRSVPStatusModel model)
        {
            try
            {
                //dbContext.RecurringEvents.First().
                var eids = _dbContext.Events.Include(x => x.Recurrence).ThenInclude(x => x.Events).First(x => x.Id == model.EventId).Recurrence.Events.Select(x => x.Id).ToList();
                foreach (var ea in _dbContext.EventAttendees.Where(x => eids.Contains(x.EventId) && x.User.Id == user.Id))
                {
                    ea.ResponseStatus = model.Status;
                    ea.Viewed = true;
                    if (ea.ResponseStatus != RSVPTypes.None) ea.ShowRSVPReminder = false;
                    _dbContext.Update(ea);
                }

                //var ea =_dbContext.EventAttendees.Include(x => x.Event).Include(x => x.User).FirstOrDefault(x => x.User.Id == user.Id && x.Event.Id == model.EventId);
                //if (ea == null) return SaveDataResponse.FromErrorMessage("User not found");

                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> SendRSVPReminders(SendRSVPRequestModel model)
        {
            try
            {

                if (model.UpdateRecurring)
                {
                    var e = await _dbContext.Events.Include(x => x.Attendees).ThenInclude(x => x.User).Include(x => x.Recurrence).ThenInclude(x => x.Events).ThenInclude(x => x.Attendees).FirstOrDefaultAsync(x => x.Id == model.EventId);
                    foreach (var rev in e.Recurrence.Events.SelectMany(x => x.Attendees).Where(x => x.ResponseStatus == RSVPTypes.None))
                    {
                        rev.ShowRSVPReminder = true;
                        //send email
                        _dbContext.Update(rev);
                    }
                }
                else
                {
                    var e = await _dbContext.Events.Include(x => x.Attendees).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == model.EventId);
                    foreach (var ea in e.Attendees.Where(x => x.ResponseStatus == RSVPTypes.None))
                    {
                        ea.ShowRSVPReminder = true;
                        //send email
                        _dbContext.Update(ea);
                    }
                }

                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> UpdateRSVP(User user, UpdateRSVPStatusModel model)
        {
            try
            {
                var ea = _dbContext.EventAttendees.Include(x => x.Event).Include(x => x.User).FirstOrDefault(x => x.User.Id == user.Id && x.Event.Id == model.EventId);
                if (ea == null) return SaveDataResponse.FromErrorMessage("User not found");
                ea.ResponseStatus = model.Status;
                ea.Viewed = true;
                if (model.Status != RSVPTypes.None) ea.ShowRSVPReminder = false;
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> CreateEvent(CreateEventModel model, long userId)
        {
            try
            {
                var user = this.GetActiveUser(userId);
                var e = new Event();


                //we need this for the owners of the event to get messages

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
                e.Status = EventStatus.Draft;
                e.State = model.State;
                if (model.LocationTolerance.HasValue) e.CheckInLocationTolerance = model.LocationTolerance.Value;
                e.VerificationCode = model.VerificationCode;
                e.VerificationMethod = model.AttendanceVerificationMethod;
                //e.MustRSVP = model.RSVPRequired;
                e.Zip = model.ZipCode;


                e.StartTime = model.GetEventStartTimeUTC(user.TimeZoneId);
                e.EndTime = model.GetEventEndTimeUTC(user.TimeZoneId);
                _dbContext.Events.Add(e);

                var ea = new EventAttendee
                {
                    User = user,
                    Event = e,
                    IsOrganizer = true,
                    Viewed = true
                };
                _dbContext.EventAttendees.Add(ea);

                await _dbContext.SaveChangesAsync();

                return SaveDataResponse.IncludeData(e.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }


        }

        private EventDate AdjustDateByDays(DateTime startTime, DateTime endTime, DaysOfWeek allowedDays)
        {
            //this expects the event organizer time zone
            int dayOffset = 0;
            switch (startTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    if (allowedDays.HasFlag(DaysOfWeek.Monday))
                    {
                        dayOffset = 0;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Tuesday))
                    {
                        dayOffset = 1;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Wednesday))
                    {
                        dayOffset = 2;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Thursday))
                    {
                        dayOffset = 3;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Friday))
                    {
                        dayOffset = 4;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Saturday))
                    {
                        dayOffset = 5;
                        break;
                    }
                    else
                    {
                        dayOffset = 6;
                        break;
                    }
                case DayOfWeek.Tuesday:
                    if (allowedDays.HasFlag(DaysOfWeek.Tuesday))
                    {
                        dayOffset = 0;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Wednesday))
                    {
                        dayOffset = 1;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Thursday))
                    {
                        dayOffset = 2;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Friday))
                    {
                        dayOffset = 3;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Saturday))
                    {
                        dayOffset = 4;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Sunday))
                    {
                        dayOffset = 5;
                        break;
                    }
                    else
                    {
                        dayOffset = 6;
                        //monday
                        break;
                    }
                case DayOfWeek.Wednesday:
                    if (allowedDays.HasFlag(DaysOfWeek.Wednesday))
                    {
                        dayOffset = 0;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Thursday))
                    {
                        dayOffset = 1;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Friday))
                    {
                        dayOffset = 2;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Saturday))
                    {
                        dayOffset = 3;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Sunday))
                    {
                        dayOffset = 4;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Monday))
                    {
                        dayOffset = 5;
                        break;
                    }
                    else
                    {
                        dayOffset = 6;
                        //tuesday
                        break;
                    }
                case DayOfWeek.Thursday:
                    if (allowedDays.HasFlag(DaysOfWeek.Thursday))
                    {
                        dayOffset = 0;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Friday))
                    {
                        dayOffset = 1;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Saturday))
                    {
                        dayOffset = 2;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Sunday))
                    {
                        dayOffset = 3;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Monday))
                    {
                        dayOffset = 4;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Tuesday))
                    {
                        dayOffset = 5;
                        break;
                    }
                    else
                    {
                        dayOffset = 6;
                        //wed
                        break;
                    }
                case DayOfWeek.Friday:
                    if (allowedDays.HasFlag(DaysOfWeek.Friday))
                    {
                        dayOffset = 0;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Saturday))
                    {
                        dayOffset = 1;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Sunday))
                    {
                        dayOffset = 2;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Monday))
                    {
                        dayOffset = 3;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Tuesday))
                    {
                        dayOffset = 4;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Wednesday))
                    {
                        dayOffset = 5;
                        break;
                    }
                    else
                    {
                        dayOffset = 6;
                        //thursday
                        break;
                    }

                case DayOfWeek.Saturday:
                    if (allowedDays.HasFlag(DaysOfWeek.Saturday))
                    {
                        dayOffset = 0;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Sunday))
                    {
                        dayOffset = 1;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Monday))
                    {
                        dayOffset = 2;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Tuesday))
                    {
                        dayOffset = 3;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Wednesday))
                    {
                        dayOffset = 4;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Thursday))
                    {
                        dayOffset = 5;
                        break;
                    }
                    else
                    {
                        dayOffset = 6;
                        //friday
                        break;
                    }

                case DayOfWeek.Sunday:
                    if (allowedDays.HasFlag(DaysOfWeek.Sunday))
                    {
                        dayOffset = 0;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Monday))
                    {
                        dayOffset = 1;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Tuesday))
                    {
                        dayOffset = 2;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Wednesday))
                    {
                        dayOffset = 3;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Thursday))
                    {
                        dayOffset = 4;
                        break;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Friday))
                    {
                        dayOffset = 5;
                        break;
                    }
                    else
                    {
                        dayOffset = 6;
                        //saturday
                        break;
                    }
                default:
                    dayOffset = 0;
                    break;

            }
            if (dayOffset == 0) return new EventDate(startTime, endTime);
            return new EventDate(startTime.AddDays(dayOffset), endTime.AddDays(dayOffset));
        }
        private bool MatchDayEnums(DayOfWeek systemDay, DaysOfWeek appDay)
        {
            switch (systemDay)
            {
                case DayOfWeek.Friday:
                    return appDay.HasFlag(Domain.Enums.DaysOfWeek.Friday);
                case DayOfWeek.Monday:
                    return appDay.HasFlag(Domain.Enums.DaysOfWeek.Monday);
                case DayOfWeek.Tuesday:
                    return appDay.HasFlag(Domain.Enums.DaysOfWeek.Tuesday);
                case DayOfWeek.Wednesday:
                    return appDay.HasFlag(Domain.Enums.DaysOfWeek.Wednesday);
                case DayOfWeek.Thursday:
                    return appDay.HasFlag(Domain.Enums.DaysOfWeek.Thursday);
                case DayOfWeek.Saturday:
                    return appDay.HasFlag(Domain.Enums.DaysOfWeek.Saturday);
                case DayOfWeek.Sunday:
                    return appDay.HasFlag(Domain.Enums.DaysOfWeek.Sunday);
                default:
                    return false;
            }
        }

        /// <summary>
        /// event start and end times IN UTC
        /// </summary>
        private class EventDate
        {
            public EventDate ToUtc(TimeZoneInfo tzi)
            {
                return new EventDate(TimeZoneInfo.ConvertTimeToUtc(this.start, tzi), TimeZoneInfo.ConvertTimeToUtc(this.end, tzi));
            }


            public EventDate(DateTime startTime, DateTime endTime)
            {
                this.start = startTime;
                this.end = endTime;
            }
            public DateTime start { get; set; }
            public DateTime end { get; set; }
        }





        private List<EventDate> GetRecurrenceDates(CreateEventModel model, string tzId)
        {

            var list = new List<EventDate>();

            var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            //bool startedInDST = userTimeZone.SupportsDaylightSavingTime && userTimeZone.IsDaylightSavingTime(model.GetEventStartTimeLocal());

            //userTimeZone.GetAdjustmentRules()[0].DaylightTransitionEnd.

            //if start date is in standard time, dates in daylight time need to be pushed forward an hour
            //if start date is in daylight time, dates in standard time need to be pushed back an hour
            DateTime currentStart = DateTime.Now;
            DateTime currentEnd = DateTime.Now;
            if (model.Recurrence.Pattern == RecurrencePatterns.Daily)
            {
                currentStart = model.GetEventStartTimeLocal();
                currentEnd = model.GetEventEndTimeLocal();
                //this is all in UTC 
                for (var cnt = 0; cnt < model.Recurrence.RecurrenceLimit; cnt++)
                {
                    //list.Add(new EventDate(currentStart.AdjustForDST(startedInDST, userTimeZone, true), currentEnd.AdjustForDST(startedInDST, userTimeZone, true)));
                    list.Add(new EventDate(currentStart.ToUTC(userTimeZone), currentEnd.ToUTC(userTimeZone)));
                    currentStart = currentStart.AddDays(model.Recurrence.RecurrenceInterval);
                    currentEnd = currentEnd.AddDays(model.Recurrence.RecurrenceInterval);
                }
            }
            else if (model.Recurrence.Pattern == Domain.Enums.RecurrencePatterns.Monthly)
            {
                var daysOfMonth = new List<int>();
                foreach (var d in model.Recurrence.DaysOfMonth.Split(','))
                {
                    int testInt;
                    if (int.TryParse(d.Trim(), out testInt))
                    {
                        if (testInt > 0 && testInt <= 31 && !daysOfMonth.Contains(testInt)) daysOfMonth.Add(testInt);
                    }
                }
                daysOfMonth.Sort();

                //do we need the current date based on the time zone event was created in????
                //if the server is in russia it may be august 1st already, but if the evennt was created in abq, it may be july 31st still
                //if the user wants events on the 2nd, 16th and 31st, it would skip july 31st unless we account for the time zone the date was created in
                var localStartDate = model.GetEventStartTimeLocal();
                var localEndDate = model.GetEventEndTimeLocal();
                var baseDate = new DateTime(localStartDate.Year, localStartDate.Month, 1);
                //var eventCount = 1;
                var monthCount = 0;
                while (list.Count < model.Recurrence.RecurrenceLimit)
                {
                    var baseMonthDate = baseDate.AddMonths(monthCount);
                    for (var dayCount = 0; dayCount < daysOfMonth.Count; dayCount++)
                    {
                        //daysOfMonth i.e. 9th, 16th, 30th, 31st
                        //baseMonthDate is in local time of event organizer
                        var daysInMonth = DateTime.DaysInMonth(baseMonthDate.Year, baseMonthDate.Month);
                        if (daysInMonth < daysOfMonth[dayCount])
                        {
                            //if they choose 31st and current month if feb
                            currentStart = new DateTime(baseMonthDate.Year, baseMonthDate.Month, daysInMonth, localStartDate.Hour, localStartDate.Minute, localStartDate.Second);
                            currentEnd = new DateTime(baseMonthDate.Year, baseMonthDate.Month, daysInMonth, localEndDate.Hour, localEndDate.Minute, localEndDate.Second);
                        }
                        else
                        {
                            currentStart = new DateTime(baseMonthDate.Year, baseMonthDate.Month, daysOfMonth[dayCount], localStartDate.Hour, localStartDate.Minute, localStartDate.Second);
                            currentEnd = new DateTime(baseMonthDate.Year, baseMonthDate.Month, daysOfMonth[dayCount], localEndDate.Hour, localEndDate.Minute, localEndDate.Second);
                        }

                        //convert to utc
                        //currentStart = currentStart.AdjustForDST(startedInDST, userTimeZone, true);
                        //currentEnd = currentStart.AdjustForDST(startedInDST, userTimeZone, true);
                        currentStart = currentStart.ToUTC(userTimeZone);
                        currentEnd = currentStart.ToUTC(userTimeZone);


                        //in case some idiot choose 30 and 31 and we're in february
                        if (!list.Any(x => x.start == currentStart && x.end == currentEnd) && currentStart >= DateTime.UtcNow)
                        {
                            list.Add(new EventDate(currentStart, currentEnd));
                        }

                        //eventCount++;
                    }
                    monthCount += model.Recurrence.RecurrenceInterval;

                }
            }
            else if (model.Recurrence.Pattern == Domain.Enums.RecurrencePatterns.Weekly)
            {
                var localStartDate = model.GetEventStartTimeLocal();
                var localEndDate = model.GetEventEndTimeLocal();
                DaysOfWeek patternDays = GetDaysOfWeek(model.Recurrence.Days);
                var firstOccurance = AdjustDateByDays(localStartDate, localEndDate, patternDays);
                var seedDates = new List<EventDate> { firstOccurance };
                switch (firstOccurance.start.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        if (patternDays.HasFlag(DaysOfWeek.Tuesday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(1), firstOccurance.end.AddDays(1)));
                        if (patternDays.HasFlag(DaysOfWeek.Wednesday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(2), firstOccurance.end.AddDays(2)));
                        if (patternDays.HasFlag(DaysOfWeek.Thursday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(3), firstOccurance.end.AddDays(3)));
                        if (patternDays.HasFlag(DaysOfWeek.Friday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(4), firstOccurance.end.AddDays(4)));
                        if (patternDays.HasFlag(DaysOfWeek.Saturday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(5), firstOccurance.end.AddDays(5)));
                        if (patternDays.HasFlag(DaysOfWeek.Sunday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(6), firstOccurance.end.AddDays(6)));
                        break;
                    case DayOfWeek.Tuesday:
                        if (patternDays.HasFlag(DaysOfWeek.Wednesday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(1), firstOccurance.end.AddDays(1)));
                        if (patternDays.HasFlag(DaysOfWeek.Thursday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(2), firstOccurance.end.AddDays(2)));
                        if (patternDays.HasFlag(DaysOfWeek.Friday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(3), firstOccurance.end.AddDays(3)));
                        if (patternDays.HasFlag(DaysOfWeek.Saturday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(4), firstOccurance.end.AddDays(4)));
                        if (patternDays.HasFlag(DaysOfWeek.Sunday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(5), firstOccurance.end.AddDays(5)));
                        if (patternDays.HasFlag(DaysOfWeek.Monday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(6), firstOccurance.end.AddDays(6)));
                        break;
                    case DayOfWeek.Wednesday:
                        if (patternDays.HasFlag(DaysOfWeek.Thursday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(1), firstOccurance.end.AddDays(1)));
                        if (patternDays.HasFlag(DaysOfWeek.Friday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(2), firstOccurance.end.AddDays(2)));
                        if (patternDays.HasFlag(DaysOfWeek.Saturday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(3), firstOccurance.end.AddDays(3)));
                        if (patternDays.HasFlag(DaysOfWeek.Sunday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(4), firstOccurance.end.AddDays(4)));
                        if (patternDays.HasFlag(DaysOfWeek.Monday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(5), firstOccurance.end.AddDays(5)));
                        if (patternDays.HasFlag(DaysOfWeek.Tuesday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(6), firstOccurance.end.AddDays(6)));
                        break;
                    case DayOfWeek.Thursday:
                        if (patternDays.HasFlag(DaysOfWeek.Friday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(1), firstOccurance.end.AddDays(1)));
                        if (patternDays.HasFlag(DaysOfWeek.Saturday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(2), firstOccurance.end.AddDays(2)));
                        if (patternDays.HasFlag(DaysOfWeek.Sunday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(3), firstOccurance.end.AddDays(3)));
                        if (patternDays.HasFlag(DaysOfWeek.Monday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(4), firstOccurance.end.AddDays(4)));
                        if (patternDays.HasFlag(DaysOfWeek.Tuesday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(5), firstOccurance.end.AddDays(5)));
                        if (patternDays.HasFlag(DaysOfWeek.Wednesday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(6), firstOccurance.end.AddDays(6)));
                        break;
                    case DayOfWeek.Friday:
                        if (patternDays.HasFlag(DaysOfWeek.Saturday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(1), firstOccurance.end.AddDays(1)));
                        if (patternDays.HasFlag(DaysOfWeek.Sunday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(2), firstOccurance.end.AddDays(2)));
                        if (patternDays.HasFlag(DaysOfWeek.Monday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(3), firstOccurance.end.AddDays(3)));
                        if (patternDays.HasFlag(DaysOfWeek.Tuesday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(4), firstOccurance.end.AddDays(4)));
                        if (patternDays.HasFlag(DaysOfWeek.Wednesday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(5), firstOccurance.end.AddDays(5)));
                        if (patternDays.HasFlag(DaysOfWeek.Thursday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(6), firstOccurance.end.AddDays(6)));
                        break;
                    case DayOfWeek.Saturday:
                        if (patternDays.HasFlag(DaysOfWeek.Sunday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(1), firstOccurance.end.AddDays(1)));
                        if (patternDays.HasFlag(DaysOfWeek.Monday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(2), firstOccurance.end.AddDays(2)));
                        if (patternDays.HasFlag(DaysOfWeek.Tuesday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(3), firstOccurance.end.AddDays(3)));
                        if (patternDays.HasFlag(DaysOfWeek.Wednesday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(4), firstOccurance.end.AddDays(4)));
                        if (patternDays.HasFlag(DaysOfWeek.Thursday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(5), firstOccurance.end.AddDays(5)));
                        if (patternDays.HasFlag(DaysOfWeek.Friday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(6), firstOccurance.end.AddDays(6)));
                        break;
                    case DayOfWeek.Sunday:
                        if (patternDays.HasFlag(DaysOfWeek.Monday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(1), firstOccurance.end.AddDays(1)));
                        if (patternDays.HasFlag(DaysOfWeek.Tuesday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(2), firstOccurance.end.AddDays(2)));
                        if (patternDays.HasFlag(DaysOfWeek.Wednesday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(3), firstOccurance.end.AddDays(3)));
                        if (patternDays.HasFlag(DaysOfWeek.Thursday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(4), firstOccurance.end.AddDays(4)));
                        if (patternDays.HasFlag(DaysOfWeek.Friday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(5), firstOccurance.end.AddDays(5)));
                        if (patternDays.HasFlag(DaysOfWeek.Saturday)) seedDates.Add(new EventDate(firstOccurance.start.AddDays(6), firstOccurance.end.AddDays(6)));
                        break;

                }

                //seedDates are in local time zone
                int eventCount = 0;
                for (var seedCount = 0; seedCount < seedDates.Count; seedCount++)
                {
                    //convert to utc
                    var start = seedDates[seedCount].start.ToUTC(userTimeZone);
                    var end = seedDates[seedCount].end.ToUTC(userTimeZone);
                    list.Add(new EventDate(start, end));
                    eventCount++;
                }
                int multiplier = model.Recurrence.RecurrenceInterval;
                while (list.Count < model.Recurrence.RecurrenceLimit)
                {
                    for (var seedCount = 0; seedCount < seedDates.Count; seedCount++)
                    {
                        var start = seedDates[seedCount].start.AddDays(7 * multiplier);
                        var end = seedDates[seedCount].end.AddDays(7 * multiplier);
                        list.Add(new EventDate(start, end).ToUtc(userTimeZone));
                        eventCount++;
                    }
                    multiplier = multiplier + model.Recurrence.RecurrenceInterval;
                }


                //need to set the start date to the first day of week after the event date
            }
            return list;
        }

        public DaysOfWeek GetDaysOfWeek(bool[] model)
        {
            DaysOfWeek patternDays = DaysOfWeek.None;
            if (model[0] == true)
            {
                patternDays = patternDays == Domain.Enums.DaysOfWeek.None ? Domain.Enums.DaysOfWeek.Monday : patternDays | Domain.Enums.DaysOfWeek.Monday;

            }
            if (model[1] == true)
            {
                patternDays = patternDays == Domain.Enums.DaysOfWeek.None ? Domain.Enums.DaysOfWeek.Tuesday : patternDays | Domain.Enums.DaysOfWeek.Tuesday;

            }
            if (model[2] == true)
            {
                patternDays = patternDays == Domain.Enums.DaysOfWeek.None ? Domain.Enums.DaysOfWeek.Wednesday : patternDays | Domain.Enums.DaysOfWeek.Wednesday;

            }
            if (model[3] == true)
            {
                patternDays = patternDays == Domain.Enums.DaysOfWeek.None ? Domain.Enums.DaysOfWeek.Thursday : patternDays | Domain.Enums.DaysOfWeek.Thursday;

            }
            if (model[4] == true)
            {
                patternDays = patternDays == Domain.Enums.DaysOfWeek.None ? Domain.Enums.DaysOfWeek.Friday : patternDays | Domain.Enums.DaysOfWeek.Friday;

            }
            if (model[5] == true)
            {
                patternDays = patternDays == Domain.Enums.DaysOfWeek.None ? Domain.Enums.DaysOfWeek.Saturday : patternDays | Domain.Enums.DaysOfWeek.Saturday;

            }
            if (model[6] == true)
            {
                patternDays = patternDays == Domain.Enums.DaysOfWeek.None ? Domain.Enums.DaysOfWeek.Sunday : patternDays | Domain.Enums.DaysOfWeek.Sunday;

            }
            return patternDays;
        }



        public async Task<SaveDataResponse> CreateRecurringEvent(CreateEventModel model, long userId)
        {
            try
            {
                var user = this.GetActiveUser(userId);
                if (model.Recurrence.DaysOfMonth != null)
                {
                    var uniqueDays = model.Recurrence.DaysOfMonth.Split(',').Distinct().ToList();
                    uniqueDays.ForEach(x => x = x.Trim());
                    model.Recurrence.DaysOfMonth = string.Join(",", uniqueDays);
                }


                var dates = GetRecurrenceDates(model, user.TimeZoneId);
                long firstEventId = 0;

                var recur = new RecurringEvent
                {
                    Pattern = model.Recurrence.Pattern,
                    RecurrenceInterval = model.Recurrence.RecurrenceInterval,
                    RecurrenceLimit = model.Recurrence.RecurrenceLimit,
                    RecurrenceDays = model.Recurrence.Pattern == RecurrencePatterns.Weekly ? GetDaysOfWeek(model.Recurrence.Days) : DaysOfWeek.None,
                    DaysInMonth = model.Recurrence.DaysOfMonth
                };
                _dbContext.RecurringEvents.Add(recur);
                foreach (var eventDate in dates)
                {
                    var e = new Event();


                    //we need this for the owners of the event to get messages

                    e.Name = model.Name;
                    e.Description = model.Description;
                    e.LocationName = model.LocationName;
                    e.CheckInTimeTolerance = 30;
                    e.CoordinateX = model.XCoord;
                    e.CoordinateY = model.YCoord;
                    e.GooglePlaceId = model.GooglePlaceId;
                    e.Address1 = model.Address1;
                    e.Address2 = model.Address2;
                    e.City = model.City;
                    e.Status = Domain.Enums.EventStatus.Draft;
                    e.State = model.State;
                    if (model.LocationTolerance.HasValue) e.CheckInLocationTolerance = model.LocationTolerance.Value;
                    e.VerificationCode = model.VerificationCode;
                    e.VerificationMethod = model.AttendanceVerificationMethod;
                    //e.MustRSVP = model.RSVPRequired;
                    e.Zip = model.ZipCode;

                    e.StartTime = eventDate.start;
                    e.EndTime = eventDate.end;


                    e.Recurrence = recur;
                    _dbContext.Events.Add(e);

                    var ea = new EventAttendee
                    {
                        User = user,
                        Event = e,
                        IsOrganizer = true,
                        Viewed = true
                    };
                    _dbContext.EventAttendees.Add(ea);

                    if (firstEventId == 0) firstEventId = e.Id;
                }


                await _dbContext.SaveChangesAsync();

                return SaveDataResponse.IncludeData(firstEventId);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }


        }
        public async Task<EventEditModel> GetEventEditModel(long eid)
        {
            var e = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eid);
            var model = new EventEditModel();
            model.Id = e.Id;
            model.Address1 = e.Address1;
            model.Address2 = e.Address2;
            model.City = e.City;
            model.Description = e.Description;
            model.EndDate = e.EndTime.Date;
            model.EndAMPM = e.EndTime.Hour >= 12 ? Models.Enums.AMPM.PM : Models.Enums.AMPM.AM;
            model.EndHour = e.EndTime.Hour;
            model.EndMinute = e.EndTime.Minute;
            model.StartDate = e.StartTime.Date;
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
        public async Task<ViewEventDetailsModel> ViewEventDetails(long eventId, long userId)
        {
            var e = await _dbContext.Events.Include(x => x.Recurrence).FirstOrDefaultAsync(x => x.Id == eventId);
            var user = this.GetActiveUser(userId);
            var model = new ViewEventDetailsModel(e, user.TimeZoneId);
            return model;
        }
        public async Task<ViewEventLocationModel> ViewEventLocationDetails(long eventId)
        {
            var e = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            var model = new ViewEventLocationModel(e);
            return model;
        }

        public async Task<EditEventDetailsModel> EditEventDetails(long eventId, long userId)
        {
            var e = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            var u = this.GetActiveUser(userId);
            var model = new EditEventDetailsModel(e, u.TimeZoneId);
            return model;
        }


        public async Task<EditEventDateModel> EditEventDates(long eventId, long userId)
        {
            var u = await _dbContext.Users.FirstAsync(x => x.Id == userId);
            var e = await _dbContext.Events.Include(x => x.Recurrence).ThenInclude(x => x.Events).FirstOrDefaultAsync(x => x.Id == eventId);
            var model = new EditEventDateModel(e, u.TimeZoneId);
            return model;
        }
        public async Task<ViewEventDatesModel> ViewEventDates(long eventId, long userId)
        {
            var u = await _dbContext.Users.FirstAsync(x => x.Id == userId);
            var e = await _dbContext.Events.Include(x => x.Recurrence).ThenInclude(x => x.Events).FirstOrDefaultAsync(x => x.Id == eventId);
            var model = new ViewEventDatesModel(e, u.TimeZoneId);
            return model;
        }
        public async Task<EditEventLocationModel> EditEventLocation(long eventId)
        {
            var e = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            var model = new EditEventLocationModel(e);
            return model;
        }


        private List<string> FindChangedEventDetails(EditEventDetailsModel model, Event e)
        {
            var changes = new List<string>();
            if (e.Name != model.Name) changes.Add("Name");
            if (model.AttendanceVerificationMethod != e.VerificationMethod) changes.Add("Verification Method");
            if (model.Description != e.Description) changes.Add("Description");
            //if (model.RSVPRequired != e.MustRSVP) changes.Add("RSVP Required");

            int startHour, endHour;
            if (model.StartHour == 12) startHour = model.StartAMPM == Models.Enums.AMPM.AM ? 0 : 12;
            else startHour = model.StartAMPM == Models.Enums.AMPM.AM ? model.StartHour : model.StartHour + 12;

            if (model.EndHour == 12) endHour = model.EndAMPM == Models.Enums.AMPM.AM ? 0 : 12;
            else endHour = model.EndAMPM == Models.Enums.AMPM.AM ? model.EndHour : model.EndHour + 12;


            if (model.StartAMPM == Models.Enums.AMPM.AM && model.StartHour == 12) startHour = 0;
            else if (model.StartAMPM == Models.Enums.AMPM.AM) startHour = model.StartHour;
            else if (model.StartAMPM == Models.Enums.AMPM.PM && startHour == 12) startHour = 12;
            else startHour = model.StartHour + 12;

            var modelStartTime = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, startHour, model.StartMinute, 0);
            var modelEndTime = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, endHour, model.EndMinute, 0);

            if (modelStartTime != e.StartTime) changes.Add("Start Time");
            if (modelEndTime != e.EndTime) changes.Add("End Time");

            return changes;
        }
        public async Task<SaveDataResponse> UpdateRecurringEventLocation(EditEventLocationModel model)
        {
            try
            {
                var ev = await _dbContext.Events.Include(x => x.Recurrence).ThenInclude(x => x.Events).FirstOrDefaultAsync(x => x.Id == model.EventId);
                foreach (var e in ev.Recurrence.Events)
                {
                    e.Address1 = model.Address1;
                    e.Address2 = model.Address2;
                    e.City = model.City;
                    e.State = model.State;
                    e.Zip = model.ZipCode;
                    e.GooglePlaceId = model.GooglePlaceId;
                    e.CoordinateX = model.XCoord;
                    e.CoordinateY = model.YCoord;
                    _dbContext.Update(e);

                }
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }

        }
        public async Task<SaveDataResponse> UpdateRecurringEventDetails(EditEventDetailsModel model)
        {
            try
            {
                var ev = await _dbContext.Events.Include(x => x.Recurrence).ThenInclude(x => x.Events).FirstOrDefaultAsync(x => x.Id == model.EventId);

                foreach (var e in ev.Recurrence.Events)
                {
                    //users can update these individually
                    var changes = FindChangedEventDetails(model, ev);
                    var draftChange = model.Status == Domain.Enums.EventStatus.Active && ev.Status != Domain.Enums.EventStatus.Active;
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


                    if (e.Id == model.EventId)
                    {
                        //we don't want to change the dates for other events if recurring
                        e.StartTime = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, startHour, model.StartMinute, 0);
                        e.EndTime = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, endHour, model.EndMinute, 0);
                    }
                    else
                    {
                        e.StartTime = new DateTime(e.StartTime.Year, e.StartTime.Month, e.StartTime.Day, startHour, model.StartMinute, 0);
                        e.EndTime = new DateTime(e.EndTime.Year, e.EndTime.Month, e.EndTime.Day, endHour, model.EndMinute, 0);
                    }

                    e.Status = model.Status;
                    _dbContext.Update(e);

                    if (draftChange)
                    {
                        //send out all emails
                        foreach (var att in _dbContext.EventAttendees.Where(x => x.EventId == e.Id))
                        {

                        }

                    }
                    else if (e.Status == Domain.Enums.EventStatus.Active && changes.Count > 0)
                    {
                        //details have changed, must send out emails
                    }
                }
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }

        }
        public async Task<SaveDataResponse> UpdateEventStatus(UpdateEventStatusModel model)
        {
            try
            {
                if (model.UpdateRecurring)
                {
                    var ev = await _dbContext.Events.Include(x => x.Recurrence).ThenInclude(x => x.Events).FirstOrDefaultAsync(x => x.Id == model.EventId);
                    foreach (var e in ev.Recurrence.Events)
                    {
                        e.Status = model.Status;
                        if (e.Status == EventStatus.Active)
                        {

                        }
                        _dbContext.Update(e);
                    }
                }
                else
                {
                    var e = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == model.EventId);
                    e.Status = model.Status;
                    if (e.Status == EventStatus.Active)
                    {
                        //emails
                    }
                    _dbContext.Update(e);

                }
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> UpdateEventDetails(EditEventDetailsModel model, long userId)
        {
            try
            {
                var e = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == model.EventId);
                var user = this.GetActiveUser(userId);
                var changes = FindChangedEventDetails(model, e);
                var draftChange = model.Status == EventStatus.Active && e.Status != EventStatus.Active;

                e.Name = model.Name;
                e.Description = model.Description;
                e.StartTime = model.GetEventStartTimeUTC(user.TimeZoneId);
                e.EndTime = model.GetEventEndTimeUTC(user.TimeZoneId);
                //e.MustRSVP = model.RSVPRequired;
                e.CheckInLocationTolerance = model.LocationTolerance;
                e.VerificationMethod = model.AttendanceVerificationMethod;
                e.VerificationCode = model.VerificationCode;
                e.Status = model.Status;
                _dbContext.Update(e);
                await _dbContext.SaveChangesAsync();


                if (draftChange)
                {
                    //send out all emails
                    foreach (var att in _dbContext.EventAttendees.Where(x => x.EventId == e.Id))
                    {

                    }

                }
                else if (e.Status == Domain.Enums.EventStatus.Active && changes.Count > 0)
                {
                    //details have changed, must send out emails
                }

                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }

        }

        public async Task<SaveDataResponse> UpdateEventLocation(EditEventLocationModel model)
        {
            try
            {
                var e = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == model.EventId);
                e.Address1 = model.Address1;
                e.Address2 = model.Address2;
                e.City = model.City;
                e.State = model.State;
                e.Zip = model.ZipCode;
                e.GooglePlaceId = model.GooglePlaceId;
                e.CoordinateX = model.XCoord;
                e.CoordinateY = model.YCoord;
                _dbContext.Update(e);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }

        }
        //private CheckInStatuses GetCheckInStatus(Event e, EventAttendee ea)
        //{
        //    if (ea.CheckedIn) return CheckInStatuses.CheckInSuccessful;
        //    if (e.StartTime.ToLocalTime().AddMinutes(e.CheckInTimeTolerance * -1) <= DateTime.Now) return CheckInStatuses.AvailableForCheckIn;
        //    return CheckInStatuses.NotAvailableForCheckIn;
        //}
        public async Task<EventCheckInModel> GetEventCheckInModel(long eventId)
        {
            var e = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            if (e == null) return null;
            return new EventCheckInModel
            {
                EventId = eventId,
                CheckInMethod = e.VerificationMethod,
                DistanceTolerance = e.CheckInLocationTolerance,
                TimeTolerance = e.CheckInTimeTolerance,
                EventCoordX = e.CoordinateX,
                EventCoordY = e.CoordinateY,
                EventName = e.Name
            };
        }

        public async Task<EventNotificationModelContainer> GetEventNotifications(long eventId, long userId, EditAccessTypes accessLevel)
        {
            var ea = await _dbContext.EventAttendees.Include(x => x.User).FirstOrDefaultAsync(x => x.Event.Id == eventId && x.User.Id == userId);
            var model = new EventNotificationModelContainer();
            model.Notifications = new List<EventNotificationModel>();
            foreach (var ean in this._dbContext.EventAttendeeNotifications.Include(x => x.Notification).ThenInclude(x => x.PostedBy).ThenInclude(x => x.User).Where(x => x.AttendeeId == ea.Id))
            {
                var localPostedDate = ean.Notification.PostedDate.FromUTC(ea.User.TimeZoneId);
                model.Notifications.Add(new EventNotificationModel
                {
                    OrganizerName = ean.Notification.PostedBy.User.Name,
                    OrganizerId = ean.Notification.PostedBy.User.Id,
                    PostedDate = ean.Notification.PostedDate,
                    Subject = ean.Notification.Subject,
                    DateText = localPostedDate.DayOfWeek.ToString() + " " + localPostedDate.ToString("d") + " " + localPostedDate.ToString("t"),
                    NotificationText = ean.Notification.MessageText,
                    IsNew = !ean.Viewed,
                    Id = ean.Id
                });
            }
            model.IsOrganizer = accessLevel == EditAccessTypes.Edit;
            return model;
        }
        public async Task<EventMessageContainerModel> GetEventMessages(long eventId, long userId)
        {
            //        var e = await _dbContext.Events.Include(x => x.Recurrence).Include(x => x.Attendees).ThenInclude(x => x.User).
            //Include(x => x.Attendees).ThenInclude(x => x.MessageBoardItems).ThenInclude(x => x.MessageBoardItem).ThenInclude(x => x.PostedBy).
            //FirstOrDefaultAsync(x => x.Id == eventId);
            var ea = await _dbContext.EventAttendees.FirstOrDefaultAsync(x => x.Event.Id == eventId && x.User.Id == userId);

            if (ea == null) return null;
            var model = new EventMessageContainerModel();
            var topics = await _dbContext.MessageBoardTopics.Include(x => x.StartedBy).Include(x => x.Messages).ThenInclude(x => x.Attendees).Where(x => x.Event.Id == eventId).ToListAsync();
            foreach (var topic in topics)
            {

                var topicModel = new EventMessageBoardTopicModel { Subject = topic.Subject, StartedBy = topic.StartedBy.Name, Id = topic.Id };
                foreach (var msg in topic.Messages)
                {
                    var localPostedDate = msg.PostedDate.FromUTC(TimeZoneInfo.FindSystemTimeZoneById(ea.User.TimeZoneId));
                    topicModel.Messages.Add(new EventMessageModel
                    {
                        MessageText = msg.MessageText,
                        CanEdit = msg.PostedBy.Id == userId,
                        PostedBy = msg.PostedBy.Name,
                        PostedById = msg.PostedBy.Id,
                        DateText = localPostedDate.DayOfWeek.ToString() + " " + localPostedDate.ToString("d") + " " + localPostedDate.ToString("t"),
                        IsNew = msg.Attendees.Any(x => x.Attendee.Id == ea.Id && !x.Viewed)
                    });
                }


                model.Topics.Add(topicModel);
            }
            //foreach (var mba in ea.Messa.GroupBy(x=>x,)
            //{

            //    var localPostedDate = mba.MessageBoardItem.PostedDate.FromUTC(TimeZoneInfo.FindSystemTimeZoneById(ea.User.TimeZoneId));
            //    model.Add(new EventMessageContainerModel
            //    {
            //        PostedBy = mba.MessageBoardItem.PostedBy.Name,
            //        PostedById = mba.MessageBoardItem.PostedBy.Id,
            //        PostedDate = localPostedDate,
            //        DateText = localPostedDate.DayOfWeek.ToString() + " " + localPostedDate.ToString("d") + " " + localPostedDate.ToString("t"),
            //        Subject = mba.MessageBoardItem.Topic.Subject,
            //        MessageText = mba.MessageBoardItem.MessageText,
            //        CanEdit = mba.MessageBoardItem.PostedBy.Id == userId,
            //        IsNew = !mba.Viewed,
            //        Id = mba.Id
            //    });
            //}
            return model;
        }


        public async Task<EventMessageContainerModel> GetEventMessages(long eventId, User user, bool canEdit)
        {

            var attendee = await _dbContext.EventAttendees.FirstOrDefaultAsync(x => x.EventId == eventId && x.User.Id == user.Id);

            var model = new EventMessageContainerModel();

            foreach (var topic in _dbContext.MessageBoardTopics.Include(x => x.StartedBy).Include(x => x.Messages).ThenInclude(x => x.PostedBy).Include(x => x.Messages).ThenInclude(x => x.Attendees).ThenInclude(x => x.Attendee).Where(x => x.Event.Id == eventId).OrderByDescending(x => x.StartedDate))
            {
                var topicModel = new EventMessageBoardTopicModel
                {
                    Subject = topic.Subject,
                    StartedBy = topic.StartedBy.Name,
                    Id = topic.Id
                };
                foreach (var msg in topic.Messages)
                {
                    var localPostedDate = msg.PostedDate.FromUTC(TimeZoneInfo.FindSystemTimeZoneById(attendee.User.TimeZoneId));
                    topicModel.Messages.Add(new EventMessageModel
                    {
                        MessageText = msg.MessageText,
                        CanEdit = canEdit ? true : msg.PostedBy.Id == user.Id,
                        PostedBy = msg.PostedBy.Name,
                        PostedById = msg.PostedBy.Id,
                        Id = msg.Attendees.First(x => x.Attendee.Id == attendee.Id).Id,
                        DateText = localPostedDate.DayOfWeek.ToString() + " " + localPostedDate.ToString("d") + " " + localPostedDate.ToString("t"),
                        PostedDate = localPostedDate,
                        IsNew = msg.Attendees.Any(x => x.Attendee.Id == attendee.Id && !x.Viewed)
                    });
                }
                model.Topics.Add(topicModel);
            }

            return model;
        }
        public async Task<EventViewModel> GetEventViewModel(long eventId, long userId, EditAccessTypes accessLevel)
        {
            //var e = await dbContext.Events.Include(x=>x.Attendees).ThenInclude(x=>x.User).Include("Attendees.MessageBoardItems.MessageBoardItem.PostedBy").FirstOrDefaultAsync(x => x.Id == eventId);
            var user = this.GetActiveUser(userId);

            var e = await _dbContext.Events.Include(x => x.Recurrence).Include(x => x.Attendees).ThenInclude(x => x.User).
                Include(x => x.Attendees).Include(x => x.UnregisteredAttendees).FirstOrDefaultAsync(x => x.Id == eventId);
            var thisAttendee = e.Attendees.FirstOrDefault(x => x.User.Id == userId);
            if (thisAttendee.Viewed == false)
            {
                thisAttendee.Viewed = true;
                _dbContext.Update(thisAttendee);
                await _dbContext.SaveChangesAsync();
            }
            //var isOrganizer = e.Organizers.Any(x => x.User.Id == user.Id);
            var model = new EventViewModel(e, user.TimeZoneId);
            model.GoogleApiKey = this.googleOptions.GoogleMapsApiKey;

            if (thisAttendee.CheckedIn) model.CheckInStatus = CheckInStatuses.CheckInSuccessful;
            else if (e.StartTime.AddMinutes(e.CheckInTimeTolerance * -1) <= DateTime.UtcNow) model.CheckInStatus = CheckInStatuses.AvailableForCheckIn;
            else model.CheckInStatus = CheckInStatuses.NotAvailableForCheckIn;



            if (e.VerificationMethod == AttendanceVerificationMethods.None) model.CheckInType = "None";
            else if (e.VerificationMethod == AttendanceVerificationMethods.PasswordOnly) model.CheckInType = "Password";
            else model.CheckInType = "Password/Location";
            model.RSVPResponse = new EventAttendeeRSVPModel { Status = thisAttendee.ResponseStatus };
            model.IsOrganizer = accessLevel == EditAccessTypes.Edit;
            model.Attendees = new EventAttendeesModel(eventId);
            foreach (var att in e.Attendees.OrderByDescending(x => x.IsOrganizer).ThenBy(x => x.User.FirstName))
            {
                model.Attendees.Attendees.Add(new EventAttendeeModel
                {
                    EventAttendeeId = att.Id,
                    UserId = att.User.Id,
                    RSVPStatus = att.ResponseStatus,
                    Name = att.User.FirstName,
                    IsOrganizer = att.IsOrganizer,
                    IsRegistered = true,
                    IsEditable = att.User.Id != userId
                });
            }
            model.Attendees.Attendees.AddRange(e.UnregisteredAttendees.Select(x => new EventAttendeeModel
            {
                Name = x.Name,
                RSVPStatus = RSVPTypes.PendingRegistration,
                Email = x.Email,
                IsRegistered = false
            }));

            model.Attendees.IsOrganizer = model.IsOrganizer;

            model.Organizers = e.Attendees.Where(x => x.IsOrganizer).Select(y => new EventOrganizerModel { Name = y.User.Name, Id = y.User.Id }).ToList();
            //model.OrganizerName = e.Attendees.First(x => x.IsOrganizer).User.Name;

            model.Messages = new EventMessageContainerModel();

            foreach (var topic in _dbContext.MessageBoardTopics.Include(x => x.Messages).ThenInclude(x => x.Attendees).ThenInclude(x => x.Attendee).Where(x => x.Event.Id == eventId).OrderBy(x => x.StartedDate))
            {
                var topicModel = new EventMessageBoardTopicModel
                {
                    Subject = topic.Subject,
                    StartedBy = topic.StartedBy.Name,
                    Id = topic.Id
                };
                try
                {
                    foreach (var msg in topic.Messages)
                    {
                        var localPostedDate = msg.PostedDate.FromUTC(TimeZoneInfo.FindSystemTimeZoneById(thisAttendee.User.TimeZoneId));
                        topicModel.Messages.Add(new EventMessageModel
                        {
                            TopicName = topic.Subject,
                            MessageText = msg.MessageText,
                            CanEdit = msg.PostedBy.Id == userId,
                            PostedBy = msg.PostedBy.Name,
                            PostedById = msg.PostedBy.Id,
                            Id = msg.Attendees.First(x => x.Attendee.Id == thisAttendee.Id).Id,
                            DateText = localPostedDate.DayOfWeek.ToString() + " " + localPostedDate.ToString("d") + " " + localPostedDate.ToString("t"),
                            IsNew = msg.Attendees.Any(x => x.Attendee.Id == thisAttendee.Id && !x.Viewed),
                            PostedDate = localPostedDate
                        });
                    }
                }
                catch (Exception ex)
                {
                    var dddd = 4;
                }

                model.Messages.Topics.Add(topicModel);
            }





            model.Notifications = new EventNotificationModelContainer();
            model.Notifications.IsOrganizer = accessLevel == EditAccessTypes.Edit;
            model.Notifications.Notifications = new List<EventNotificationModel>();
            foreach (var ean in this._dbContext.EventAttendeeNotifications.Include("Notification.PostedBy.User").Where(x => x.AttendeeId == thisAttendee.Id))
            {
                var localPostedDate = ean.Notification.PostedDate.FromUTC(thisAttendee.User.TimeZoneId);
                model.Notifications.Notifications.Add(new EventNotificationModel
                {
                    OrganizerName = ean.Notification.PostedBy.User.Name,
                    OrganizerId = ean.Notification.PostedBy.User.Id,
                    PostedDate = ean.Notification.PostedDate,
                    DateText = localPostedDate.DayOfWeek.ToString() + " " + localPostedDate.ToString("d") + " " + localPostedDate.ToString("t"),
                    Subject = ean.Notification.Subject,
                    NotificationText = ean.Notification.MessageText,
                    IsNew = !ean.Viewed,
                    Id = ean.Id
                });
            }

            if (e.RecurrenceId.HasValue)
            {
                model.EventRecurrence = _dbContext.RecurringEvents.Include(x => x.Events).FirstOrDefault(x => x.Id == e.RecurrenceId.Value)?.Events.Select(y =>
                 new RecurrenceListModel
                 {
                     Name = y.Name,
                     StartDate = y.StartTime.FromUTC(thisAttendee.User.TimeZoneId),
                     EndDate = y.EndTime.FromUTC(thisAttendee.User.TimeZoneId),
                     Id = y.Id,
                     DateText = y.GetDateText(user.TimeZoneId)
                 }).ToArray();

                model.Details.RecurrenceText += " Until " + model.EventRecurrence.OrderBy(x => x.EndDate).Last().EndDate.ToShortDateString() + ".";
            }

            return model;
        }

        //public async Task AddEventAttendee(long eventId, int userId, bool asOwner)
        //{
        //    var e =_dbContext.Events.FirstOrDefault(x => x.Id == eventId);
        //    var u =_dbContext.Users.FirstOrDefault(x => x.Id == userId);
        //    var eo = new EventOrganizer { User = u, Event = e, Owner = asOwner };
        //   _dbContext.EventOrganizers.Add(eo);
        //    await dbContext.SaveChangesAsync();
        //}

        //public async Task<SaveDataResponse> AddEventAttendee(AddRegisteredEventAttendeeModel model)
        //{
        //    try
        //    {
        //        var e =_dbContext.Events.FirstOrDefault(x => x.Id == model.eventId);
        //        var u =_dbContext.Users.FirstOrDefault(x => x.Id == model.userId);
        //        var ea = new EventAttendee
        //        {
        //            Event = e,
        //            User = u,
        //            DateCreated = DateTime.Now,
        //            UserCreated = u.UserName,
        //            AllowEventUpdates = true,
        //            Viewed = false,
        //            IsOrganizer = model.isOwner
        //        };
        //       _dbContext.EventAttendees.Add(ea);
        //        await dbContext.SaveChangesAsync();
        //        return SaveDataResponse.Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return SaveDataResponse.FromException(ex);
        //    }

        //}


        public void PopulateEventAttendeeName(EventAttendeeModel model)
        {
            if (model.EventAttendeeId.HasValue)
            {
                var item = _dbContext.EventAttendees.Include(x => x.User).FirstOrDefault(x => x.Id == model.EventAttendeeId.Value);
                if (item != null) model.Name = item.User.FirstName;
            }
            else if (model.UserId.HasValue)
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.Id == model.UserId.Value);
                if (user != null) model.Name = user.FirstName;
            }
            else if (model.UserGroupId.HasValue)
            {
                var groupItem = _dbContext.AttendeeGroups.FirstOrDefault(x => x.Id == model.UserGroupId.Value);
                if (groupItem != null) model.Name = groupItem.Name;
            }


        }

        public async Task<EventAttendeesModel> GetEventAttendees(long eventId, long activeId)
        {
            var atts = await _dbContext.Events.Include(x => x.Attendees).ThenInclude(x => x.User).Include(x => x.UnregisteredAttendees).FirstOrDefaultAsync(x => x.Id == eventId);
            if (atts == null) throw new Exception("Event Not Found");
            var model = new EventAttendeesModel(eventId);
            //we should include the person viewing for now
            model.Attendees = atts.Attendees.OrderByDescending(x=>x.IsOrganizer).ThenBy(x=>x.User.FirstName).Select(x => new EventAttendeeModel
            {
                EventAttendeeId = x.Id,
                Name = x.User.FirstName,
                PhoneNumber = x.User.PhoneNumber,
                UserId = x.User.Id,
                RSVPStatus = x.ResponseStatus,
                IsRegistered = true,
                IsOrganizer = x.IsOrganizer,
                IsEditable = x.User.Id != activeId
            }).ToList();
            //model.Attendees = model.Attendees.OrderBy(x => x.IsOrganizer).ThenBy(x => x.Name).ToList();


            atts.UnregisteredAttendees.ToList().ForEach(x => model.Attendees.Add(new EventAttendeeModel
            {
                Name = x.Name,
                Email = x.Email,
                RSVPStatus = Domain.Enums.RSVPTypes.PendingRegistration,
                IsRegistered = false
            }));

            return model;




            //            model.IsOrganizer = accessLevel == EditAccessTypes.Edit;
            //model.Attendees = new ViewEventAttendeesModel();
            //foreach (var att in e.Attendees.Where(x => x.User.Id != userId))
            //{
            //    model.Attendees.Attendees.Add(new EventAttendeeModel
            //    {
            //        Id = att.Id,
            //        UserId = att.User.Id,
            //        RSVPStatus = att.ResponseStatus,
            //        Name = att.User.Name,
            //        IsOrganizer = att.IsOrganizer
            //    });
            //}
            //model.Attendees.IsOrganizer = model.IsOrganizer;
        }


        public async Task<SaveDataResponse> CheckInLocation(long userId, long eventId, double lon, double lat)
        {
            try
            {
                var e = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
                if (e == null) return SaveDataResponse.FromErrorMessage("Event Not Found");
                var ea = await _dbContext.EventAttendees.FirstOrDefaultAsync(x => x.Event.Id == eventId && x.User.Id == userId);
                if (ea == null) return SaveDataResponse.FromErrorMessage("Event Attendee Not Found");
                if (e.VerificationMethod != AttendanceVerificationMethods.PasswordOrLocation) return SaveDataResponse.FromErrorMessage("This event requires a password to check in");
                if (GetDistance(lat, lon, e.CoordinateY, e.CoordinateX) > e.CheckInLocationTolerance) return SaveDataResponse.FromErrorMessage("Distance is outside the tolerace");


                ea.CheckedIn = true;
                ea.CheckInCoordinateX = lon;
                ea.CheckInCoordinateY = lat;
                ea.CheckInTime = DateTime.Now.ToUniversalTime();
                _dbContext.Update(ea);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }

        }
        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var radlat1 = Math.PI * lat1 / 180;
            var radlat2 = Math.PI * lat2 / 180;
            var radlon1 = Math.PI * lon1 / 180;
            var radlon2 = Math.PI * lon2 / 180;
            var theta = lon1 - lon2;
            var radtheta = Math.PI * theta / 180;
            var dist = Math.Sin(radlat1) * Math.Sin(radlat2) + Math.Cos(radlat1) * Math.Cos(radlat2) * Math.Cos(radtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;
            dist = dist * 5280;
            return dist;
        }
        public async Task<SaveDataResponse> CheckInPassword(long userId, long eventId, string password)
        {
            try
            {
                var e = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
                if (e == null) return SaveDataResponse.FromErrorMessage("Event Not Found");
                var ea = await _dbContext.EventAttendees.FirstOrDefaultAsync(x => x.Event.Id == eventId && x.User.Id == userId);
                if (ea == null) return SaveDataResponse.FromErrorMessage("Event Attendee Not Found");

                if (password.ToUpper() != e.VerificationCode.ToUpper()) return SaveDataResponse.FromErrorMessage("Incorrect Password");

                ea.CheckInTime = DateTime.Now.ToUniversalTime();
                ea.CheckedIn = true;
                _dbContext.Update(ea);

                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }

        }

        public async Task<SaveDataResponse> DeleteEventAttendee(long attendee, long eventId)
        {
            try
            {
                var eo = _dbContext.EventAttendees.FirstOrDefault(x => x.Id == attendee);
                if (eo == null) return SaveDataResponse.FromErrorMessage("event attendee id " + attendee + " not found");
                if (eo.EventId != eventId) return SaveDataResponse.FromErrorMessage("attendee is not part of event");
                _dbContext.EventAttendees.Remove(eo);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }

        }
        private CheckInStatuses GetCheckInStatus(Event e, EventAttendee ea)
        {
            if (ea.CheckedIn) return CheckInStatuses.CheckInSuccessful;
            if (e.StartTime.AddMinutes(e.CheckInTimeTolerance * -1) <= DateTime.Now.ToUniversalTime()) return CheckInStatuses.AvailableForCheckIn;
            return CheckInStatuses.NotAvailableForCheckIn;
        }
        public async Task<EventInformationModel> GetDashboardEventItem(long eventId, long userId)
        {
            var e = _dbContext.Events.Include(x => x.Recurrence).Include(x => x.Attendees).ThenInclude(x => x.User).FirstOrDefault(x => x.Id == eventId);
            var ea = e.Attendees.FirstOrDefault(x => x.User.Id == userId);
            var eim = new EventInformationModel
            {
                EndDate = e.EndTime,
                StartDate = e.StartTime,
                DateText = e.GetSimpleDateText(ea.User.TimeZoneId),
                Id = e.Id,
                OrganizedByUser = ea.IsOrganizer,
                CheckInStatus = GetCheckInStatus(e, ea),
                IsNew = !ea.Viewed,
                Name = e.Name,
                OrganizerName = e.Attendees.First(x => x.IsOrganizer).User.Name,
                Status = e.Status,
                RSVPStatus = ea.ResponseStatus,
                IsRecurring = e.Recurrence != null,
                RSVPRequested = ea.ShowRSVPReminder == true
            };
            return eim;
        }
        public async Task<List<EventInformationModel>> SearchEvents(string search, long userId)
        {

            IQueryable<Event> events = null;
            if (search == "*")
            {
                events = _dbContext.Events.Include("Attendees.User");
            }
            else
            {
                events = _dbContext.Events.Include("Attendees.User").Where(x => x.Name.ToUpper().Contains(search.ToUpper()));
            }

            var list = new List<EventInformationModel>();
            await events.ForEachAsync(x => list.Add(new EventInformationModel
            {
                Status = x.Status,
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
