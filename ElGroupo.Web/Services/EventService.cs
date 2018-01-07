﻿using ElGroupo.Domain;
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

namespace ElGroupo.Web.Services
{
    public class EventService:BaseService
    {

        private readonly IEmailService mailService;
        private UserManager<User> userManager;
        public EventService(ElGroupoDbContext ctx, IEmailService service, UserManager<User> manager): base(ctx)
        {
            this._dbContext = ctx;
            this.mailService = service;
            this.userManager = manager;
        }

        public async Task<EditAccessTypes> CheckEventAccess(ClaimsPrincipal principal, long eventId)
        {
            
            if (principal.IsInRole("admin")) return EditAccessTypes.Edit;
            var user = await this.userManager.GetUserAsync(principal);
            if (await this._dbContext.EventAttendees.AnyAsync(x => x.User.Id == user.Id && x.EventId == eventId && x.IsOrganizer)) return EditAccessTypes.Edit;
            if (await this._dbContext.EventAttendees.AnyAsync(x => x.User.Id == user.Id && x.EventId == eventId)) return EditAccessTypes.View;
            return EditAccessTypes.None;
        }
        public async Task<EditAccessTypes> CheckEventAccess(ClaimsPrincipal principal, long eventId, long userId)
        {
            if (principal.IsInRole("admin")) return EditAccessTypes.Edit;
            if (await this._dbContext.EventAttendees.AnyAsync(x => x.User.Id == userId && x.EventId == eventId && x.IsOrganizer)) return EditAccessTypes.Edit;
            if (await this._dbContext.EventAttendees.AnyAsync(x => x.User.Id == userId && x.EventId == eventId)) return EditAccessTypes.View;
            return EditAccessTypes.None;
        }

        public async Task<SaveDataResponse> AddUnregisteredAttendee(User u, Event e, UnregisteredEventAttendeeModel model)
        {
            try
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
                _dbContext.UnregisteredEventAttendees.Add(uea);
                await _dbContext.SaveChangesAsync();

                return new SaveDataResponse { Success = true, ResponseData = uea.RegisterToken };
            }
            catch (Exception ex)
            {
                return new SaveDataResponse { Success = false, ErrorMessage = ex.ToString() };
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
                    Name = att.User.Name,
                    Id = att.Id,
                    RSVPStatus = att.ResponseStatus,
                    UserId = att.User.Id,
                    PhoneNumber = att.User.PhoneNumber
                });
            }
            return model;
        }

        public async Task<SaveDataResponse> DeleteEvent(long eventId)
        {

            var e = _dbContext.Events.FirstOrDefault(x => x.Id == eventId);

            if (e == null) return SaveDataResponse.FromErrorMessage("Event Id " + eventId + " not found");

            try
            {
                var unregisteredEventAttendees = _dbContext.UnregisteredEventAttendees.Where(x => x.EventId == eventId).ToList();
                foreach (var u in unregisteredEventAttendees) _dbContext.Remove(unregisteredEventAttendees);

                var mbias = _dbContext.MessageBoardItemAttendees.Include(x => x.Attendee).ThenInclude(x => x.Event).Where(x => x.Attendee.Event.Id == eventId).ToList();
                foreach (var mbia in mbias) _dbContext.Remove(mbia);

                var mbis = _dbContext.MessageBoardItems.Include(x => x.Event).Where(x => x.Event.Id == eventId).ToList();
                foreach (var mbi in mbis) _dbContext.Remove(mbi);

                var eatns = _dbContext.EventAttendeeNotifications.Include(x => x.Attendee).ThenInclude(x => x.Event).Where(x => x.Attendee.Event.Id == eventId).ToList();
                foreach (var eatn in eatns) _dbContext.Remove(eatn);

                var eans = _dbContext.EventAttendees.Include(x => x.Event).Where(x => x.Event.Id == eventId).ToList();
                foreach (var ean in eans) _dbContext.Remove(ean);

                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }



        }
        public async Task<SaveDataResponse> UpdateRecurringEventAttendees(SavePendingAttendeesModel model)
        {
            try
            {
                var ev = await _dbContext.Events.Include(x => x.Recurrence).ThenInclude(x => x.Events).FirstOrDefaultAsync(x => x.Id == model.EventId);
                if (ev == null) return new SaveDataResponse { Success = false, ErrorMessage = "Could not find event with id = " + model.EventId };
                foreach (var recurringEvent in ev.Recurrence.Events)
                {
                    foreach (var attendee in model.Attendees)
                    {
                        await AddEventAttendee(recurringEvent, attendee);
                    }
                    if (recurringEvent.Status == EventStatus.Active)
                    {
                        //send emails
                    }
                }
                await _dbContext.SaveChangesAsync();
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
            return await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == id);
        }

        private async Task AddEventAttendee(Event e, PendingEventAttendeeModel model)
        {
            if (model.Id == -1)
            {
                //new user
                //check for email
                var emailCheck = await _dbContext.Users.AnyAsync(x => x.Email == model.Email);
                if (emailCheck)
                {
                    var newAttendee = new EventAttendee
                    {
                        User = _dbContext.Find<User>(model.Id),
                        Event = e,
                        IsOrganizer = model.Owner,
                        ResponseStatus = Domain.Enums.RSVPTypes.None

                    };
                    _dbContext.Add(newAttendee);
                }
                else
                {


                    var unregisteredAttendee = new UnregisteredEventAttendee
                    {
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
            }
            else
            {
                //check to see if this attendee has already been added
                if (model.Group)
                {
                    var attGroup = await _dbContext.AttendeeGroups.Include(x => x.Attendees).ThenInclude(x => x.User).FirstAsync(x => x.Id == model.Id);
                    foreach (var user in attGroup.Attendees)
                    {
                        var newAttendee = new EventAttendee
                        {
                            User = user.User,
                            Event = e,
                            IsOrganizer = false,
                            ResponseStatus = Domain.Enums.RSVPTypes.None
                        };
                        _dbContext.Add(newAttendee);
                        if (e.Status == Domain.Enums.EventStatus.Active)
                        {
                            //send email
                        }
                    }
                }
                else
                {
                    var emailCheck = await _dbContext.EventAttendees.AnyAsync(x => x.EventId == e.Id && x.User.Id == model.Id);
                    if (!emailCheck)
                    {
                        var newAttendee = new EventAttendee
                        {
                            User = _dbContext.Find<User>(model.Id),
                            Event = e,
                            IsOrganizer = model.Owner,
                            ResponseStatus = Domain.Enums.RSVPTypes.None
                        };
                        _dbContext.Add(newAttendee);
                    }
                }

            }
        }
        public async Task<SaveDataResponse> UpdateEventAttendees(SavePendingAttendeesModel model)
        {
            try
            {
                var e = _dbContext.Find<Event>(model.EventId);
                if (e == null)
                {
                    return new SaveDataResponse
                    {
                        Success = false,
                        ErrorMessage = "could not find event with id = " + model.EventId
                    };
                }

                foreach (var att in model.Attendees)
                {
                    await AddEventAttendee(e, att);
                }

                await _dbContext.SaveChangesAsync();
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
                var eids =_dbContext.Events.Include(x => x.Recurrence).ThenInclude(x => x.Events).First(x => x.Id == model.EventId).Recurrence.Events.Select(x => x.Id).ToList();
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
        public async Task<SaveDataResponse> SendRSVPReminders(User user, SendRSVPRequestModel model)
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
                var ea =_dbContext.EventAttendees.Include(x => x.Event).Include(x => x.User).FirstOrDefault(x => x.User.Id == user.Id && x.Event.Id == model.EventId);
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
                e.MustRSVP = model.RSVPRequired;
                e.Zip = model.ZipCode;


                e.StartTime = model.GetFullStartDate(user.TimeZoneId);
                e.EndTime = model.GetFullEndDate(user.TimeZoneId);
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

        private DateTime AdjustDateByDays(DateTime startDate, DaysOfWeek allowedDays)
        {
            switch (startDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    if (allowedDays.HasFlag(DaysOfWeek.Monday))
                    {
                        return startDate;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Tuesday))
                    {
                        return startDate.AddDays(1);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Wednesday))
                    {
                        return startDate.AddDays(2);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Thursday))
                    {
                        return startDate.AddDays(3);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Friday))
                    {
                        return startDate.AddDays(4);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Saturday))
                    {
                        return startDate.AddDays(5);
                    }
                    else
                    {
                        return startDate.AddDays(6);
                    }
                case DayOfWeek.Tuesday:
                    if (allowedDays.HasFlag(DaysOfWeek.Tuesday))
                    {
                        return startDate;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Wednesday))
                    {
                        return startDate.AddDays(1);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Thursday))
                    {
                        return startDate.AddDays(2);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Friday))
                    {
                        return startDate.AddDays(3);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Saturday))
                    {
                        return startDate.AddDays(4);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Sunday))
                    {
                        return startDate.AddDays(5);
                    }
                    else
                    {
                        //monday
                        return startDate.AddDays(6);
                    }
                case DayOfWeek.Wednesday:
                    if (allowedDays.HasFlag(DaysOfWeek.Wednesday))
                    {
                        return startDate;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Thursday))
                    {
                        return startDate.AddDays(1);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Friday))
                    {
                        return startDate.AddDays(2);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Saturday))
                    {
                        return startDate.AddDays(3);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Sunday))
                    {
                        return startDate.AddDays(4);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Monday))
                    {
                        return startDate.AddDays(5);
                    }
                    else
                    {
                        //tuesday
                        return startDate.AddDays(6);
                    }
                case DayOfWeek.Thursday:
                    if (allowedDays.HasFlag(DaysOfWeek.Thursday))
                    {
                        return startDate;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Friday))
                    {
                        return startDate.AddDays(1);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Saturday))
                    {
                        return startDate.AddDays(2);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Sunday))
                    {
                        return startDate.AddDays(3);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Monday))
                    {
                        return startDate.AddDays(4);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Tuesday))
                    {
                        return startDate.AddDays(5);
                    }
                    else
                    {
                        //wed
                        return startDate.AddDays(6);
                    }
                case DayOfWeek.Friday:
                    if (allowedDays.HasFlag(DaysOfWeek.Friday))
                    {
                        return startDate;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Saturday))
                    {
                        return startDate.AddDays(1);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Sunday))
                    {
                        return startDate.AddDays(2);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Monday))
                    {
                        return startDate.AddDays(3);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Tuesday))
                    {
                        return startDate.AddDays(4);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Wednesday))
                    {
                        return startDate.AddDays(5);
                    }
                    else
                    {
                        //thursday
                        return startDate.AddDays(6);
                    }

                case DayOfWeek.Saturday:
                    if (allowedDays.HasFlag(DaysOfWeek.Saturday))
                    {
                        return startDate;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Sunday))
                    {
                        return startDate.AddDays(1);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Monday))
                    {
                        return startDate.AddDays(2);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Tuesday))
                    {
                        return startDate.AddDays(3);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Wednesday))
                    {
                        return startDate.AddDays(4);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Thursday))
                    {
                        return startDate.AddDays(5);
                    }
                    else
                    {
                        //friday
                        return startDate.AddDays(6);
                    }

                case DayOfWeek.Sunday:
                    if (allowedDays.HasFlag(DaysOfWeek.Sunday))
                    {
                        return startDate;
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Monday))
                    {
                        return startDate.AddDays(1);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Tuesday))
                    {
                        return startDate.AddDays(2);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Wednesday))
                    {
                        return startDate.AddDays(3);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Thursday))
                    {
                        return startDate.AddDays(4);
                    }
                    else if (allowedDays.HasFlag(DaysOfWeek.Friday))
                    {
                        return startDate.AddDays(5);
                    }
                    else
                    {
                        //saturday
                        return startDate.AddDays(6);
                    }
                default:
                    return startDate;

            }
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
        private List<DateTime> GetRecurrenceDates(EventRecurrenceModel model, DateTime startDate)
        {
            var list = new List<DateTime>();
            DateTime currentDate = startDate;
            if (model.Pattern == Domain.Enums.RecurrencePatterns.Daily)
            {
                for (var cnt = 0; cnt < model.RecurrenceLimit; cnt++)
                {
                    list.Add(currentDate);
                    currentDate = currentDate.AddDays(model.RecurrenceInterval);
                    cnt++;
                }
            }
            else if (model.Pattern == Domain.Enums.RecurrencePatterns.Monthly)
            {
                var daysOfMonth = new List<int>();
                foreach (var d in model.DaysOfMonth.Split(','))
                {
                    int testInt;
                    if (int.TryParse(d.Trim(), out testInt))
                    {
                        if (testInt > 0 && testInt <= 31 && !daysOfMonth.Contains(testInt)) daysOfMonth.Add(testInt);
                    }
                }
                daysOfMonth.Sort();

                var baseDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                //var eventCount = 1;
                var monthCount = 0;
                while (list.Count < model.RecurrenceLimit)
                {
                    var baseMonthDate = baseDate.AddMonths(monthCount);
                    for (var dayCount = 0; dayCount < daysOfMonth.Count; dayCount++)
                    {
                        var daysInMonth = DateTime.DaysInMonth(baseMonthDate.Year, baseMonthDate.Month);
                        if (daysInMonth < daysOfMonth[dayCount]) currentDate = new DateTime(baseMonthDate.Year, baseMonthDate.Month, daysInMonth);
                        else currentDate = new DateTime(baseMonthDate.Year, baseMonthDate.Month, daysOfMonth[dayCount]);

                        //in case some idiot choose 30 and 31 and we're in february
                        if (!list.Contains(currentDate) && currentDate.Date >= DateTime.Today.Date) list.Add(currentDate);

                        //eventCount++;
                    }
                    monthCount += model.RecurrenceInterval;

                }
                //for (var cnt = 0; cnt < model.RecurrenceLimit; cnt++)
                //{
                //    list.Add(currentDate);
                //    currentDate = currentDate.AddMonths(model.RecurrenceInterval);
                //    cnt++;
                //}
            }
            else if (model.Pattern == Domain.Enums.RecurrencePatterns.Weekly)
            {

                Domain.Enums.DaysOfWeek patternDays = GetDaysOfWeek(model.Days);



                startDate = AdjustDateByDays(startDate, patternDays);
                var seedDates = new List<DateTime> { startDate };
                switch (startDate.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        if (patternDays.HasFlag(DaysOfWeek.Tuesday)) seedDates.Add(startDate.AddDays(1));
                        if (patternDays.HasFlag(DaysOfWeek.Wednesday)) seedDates.Add(startDate.AddDays(2));
                        if (patternDays.HasFlag(DaysOfWeek.Thursday)) seedDates.Add(startDate.AddDays(3));
                        if (patternDays.HasFlag(DaysOfWeek.Friday)) seedDates.Add(startDate.AddDays(4));
                        if (patternDays.HasFlag(DaysOfWeek.Saturday)) seedDates.Add(startDate.AddDays(5));
                        if (patternDays.HasFlag(DaysOfWeek.Sunday)) seedDates.Add(startDate.AddDays(6));
                        break;
                    case DayOfWeek.Tuesday:
                        if (patternDays.HasFlag(DaysOfWeek.Wednesday)) seedDates.Add(startDate.AddDays(1));
                        if (patternDays.HasFlag(DaysOfWeek.Thursday)) seedDates.Add(startDate.AddDays(2));
                        if (patternDays.HasFlag(DaysOfWeek.Friday)) seedDates.Add(startDate.AddDays(3));
                        if (patternDays.HasFlag(DaysOfWeek.Saturday)) seedDates.Add(startDate.AddDays(4));
                        if (patternDays.HasFlag(DaysOfWeek.Sunday)) seedDates.Add(startDate.AddDays(5));
                        if (patternDays.HasFlag(DaysOfWeek.Monday)) seedDates.Add(startDate.AddDays(6));
                        break;
                    case DayOfWeek.Wednesday:
                        if (patternDays.HasFlag(DaysOfWeek.Thursday)) seedDates.Add(startDate.AddDays(1));
                        if (patternDays.HasFlag(DaysOfWeek.Friday)) seedDates.Add(startDate.AddDays(2));
                        if (patternDays.HasFlag(DaysOfWeek.Saturday)) seedDates.Add(startDate.AddDays(3));
                        if (patternDays.HasFlag(DaysOfWeek.Sunday)) seedDates.Add(startDate.AddDays(4));
                        if (patternDays.HasFlag(DaysOfWeek.Monday)) seedDates.Add(startDate.AddDays(5));
                        if (patternDays.HasFlag(DaysOfWeek.Tuesday)) seedDates.Add(startDate.AddDays(6));
                        break;
                    case DayOfWeek.Thursday:
                        if (patternDays.HasFlag(DaysOfWeek.Friday)) seedDates.Add(startDate.AddDays(1));
                        if (patternDays.HasFlag(DaysOfWeek.Saturday)) seedDates.Add(startDate.AddDays(2));
                        if (patternDays.HasFlag(DaysOfWeek.Sunday)) seedDates.Add(startDate.AddDays(3));
                        if (patternDays.HasFlag(DaysOfWeek.Monday)) seedDates.Add(startDate.AddDays(4));
                        if (patternDays.HasFlag(DaysOfWeek.Tuesday)) seedDates.Add(startDate.AddDays(5));
                        if (patternDays.HasFlag(DaysOfWeek.Wednesday)) seedDates.Add(startDate.AddDays(6));
                        break;
                    case DayOfWeek.Friday:
                        if (patternDays.HasFlag(DaysOfWeek.Saturday)) seedDates.Add(startDate.AddDays(1));
                        if (patternDays.HasFlag(DaysOfWeek.Sunday)) seedDates.Add(startDate.AddDays(2));
                        if (patternDays.HasFlag(DaysOfWeek.Monday)) seedDates.Add(startDate.AddDays(3));
                        if (patternDays.HasFlag(DaysOfWeek.Tuesday)) seedDates.Add(startDate.AddDays(4));
                        if (patternDays.HasFlag(DaysOfWeek.Wednesday)) seedDates.Add(startDate.AddDays(5));
                        if (patternDays.HasFlag(DaysOfWeek.Thursday)) seedDates.Add(startDate.AddDays(6));
                        break;
                    case DayOfWeek.Saturday:
                        if (patternDays.HasFlag(DaysOfWeek.Sunday)) seedDates.Add(startDate.AddDays(1));
                        if (patternDays.HasFlag(DaysOfWeek.Monday)) seedDates.Add(startDate.AddDays(2));
                        if (patternDays.HasFlag(DaysOfWeek.Tuesday)) seedDates.Add(startDate.AddDays(3));
                        if (patternDays.HasFlag(DaysOfWeek.Wednesday)) seedDates.Add(startDate.AddDays(4));
                        if (patternDays.HasFlag(DaysOfWeek.Thursday)) seedDates.Add(startDate.AddDays(5));
                        if (patternDays.HasFlag(DaysOfWeek.Friday)) seedDates.Add(startDate.AddDays(6));
                        break;
                    case DayOfWeek.Sunday:
                        if (patternDays.HasFlag(DaysOfWeek.Monday)) seedDates.Add(startDate.AddDays(1));
                        if (patternDays.HasFlag(DaysOfWeek.Tuesday)) seedDates.Add(startDate.AddDays(2));
                        if (patternDays.HasFlag(DaysOfWeek.Wednesday)) seedDates.Add(startDate.AddDays(3));
                        if (patternDays.HasFlag(DaysOfWeek.Thursday)) seedDates.Add(startDate.AddDays(4));
                        if (patternDays.HasFlag(DaysOfWeek.Friday)) seedDates.Add(startDate.AddDays(5));
                        if (patternDays.HasFlag(DaysOfWeek.Saturday)) seedDates.Add(startDate.AddDays(6));
                        break;

                }
                int eventCount = 0;
                for (var seedCount = 0; seedCount < seedDates.Count; seedCount++)
                {
                    list.Add(seedDates[seedCount]);
                    eventCount++;
                }
                int multiplier = model.RecurrenceInterval;
                while (list.Count < model.RecurrenceLimit)
                {
                    for (var seedCount = 0; seedCount < seedDates.Count; seedCount++)
                    {
                        list.Add(seedDates[seedCount].AddDays(7 * multiplier));
                        eventCount++;
                    }
                    multiplier = multiplier + model.RecurrenceInterval;
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

        private DateTime GetUTCDateFromEventModel(CreateEventModel model)
        {
            //how do we adjust this for utc?
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


            return new DateTime(model.EventDate.Year, model.EventDate.Month, model.EventDate.Day, startHour, model.StartMinute, 0).ToUniversalTime();

        }

        public async Task<SaveDataResponse> CreateRecurringEvent(CreateEventModel model, User user)
        {
            try
            {
                if (model.Recurrence.DaysOfMonth != null)
                {
                    var uniqueDays = model.Recurrence.DaysOfMonth.Split(',').Distinct().ToList();
                    uniqueDays.ForEach(x => x = x.Trim());
                    model.Recurrence.DaysOfMonth = string.Join(",", uniqueDays);
                }


                var dates = GetRecurrenceDates(model.Recurrence,GetUTCDateFromEventModel(model));
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


                    e.StartTime = new DateTime(eventDate.Year, eventDate.Month, eventDate.Day, startHour, model.StartMinute, 0);
                    e.EndTime = new DateTime(eventDate.Year, eventDate.Month, eventDate.Day, endHour, model.EndMinute, 0);
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

        public async Task<EditEventDetailsModel> EditEventDetails(long eventId)
        {
            var e = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            var model = new EditEventDetailsModel(e);
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
                        e.StartTime = new DateTime(model.EventDate.Year, model.EventDate.Month, model.EventDate.Day, startHour, model.StartMinute, 0);
                        e.EndTime = new DateTime(model.EventDate.Year, model.EventDate.Month, model.EventDate.Day, endHour, model.EndMinute, 0);
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
                e.StartTime = model.GetFullStartDate(user.TimeZoneId);
                e.EndTime = model.GetFullEndDate(user.TimeZoneId);
             
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
        private CheckInStatuses GetCheckInStatus(Event e, EventAttendee ea)
        {
            if (ea.CheckedIn) return CheckInStatuses.CheckInSuccessful;
            if (e.StartTime.ToLocalTime().AddMinutes(e.CheckInTimeTolerance * -1) <= DateTime.Now) return CheckInStatuses.AvailableForCheckIn;
            return CheckInStatuses.NotAvailableForCheckIn;
        }
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
        public async Task<EventViewModel> GetEventViewModel(long eventId, long userId, EditAccessTypes accessLevel)
        {
            //var e = await dbContext.Events.Include(x=>x.Attendees).ThenInclude(x=>x.User).Include("Attendees.MessageBoardItems.MessageBoardItem.PostedBy").FirstOrDefaultAsync(x => x.Id == eventId);
            var user = this.GetActiveUser(userId);
            
            var e = await _dbContext.Events.Include(x => x.Recurrence).Include(x => x.Attendees).ThenInclude(x => x.User).
                Include(x => x.Attendees).ThenInclude(x => x.MessageBoardItems).ThenInclude(x => x.MessageBoardItem).ThenInclude(x => x.PostedBy).
                FirstOrDefaultAsync(x => x.Id == eventId);
            var thisAttendee = e.Attendees.FirstOrDefault(x => x.User.Id == userId);
            if (thisAttendee.Viewed == false)
            {
                thisAttendee.Viewed = true;
               _dbContext.Update(thisAttendee);
                await _dbContext.SaveChangesAsync();
            }
            //var isOrganizer = e.Organizers.Any(x => x.User.Id == user.Id);
            var model = new EventViewModel(e, user.TimeZoneId);


            if (thisAttendee.CheckedIn) model.CheckInStatus = CheckInStatuses.CheckInSuccessful;
            else if (e.MustRSVP && e.StartTime.ToLocalTime().AddMinutes(e.CheckInTimeTolerance * -1) <= DateTime.Now) model.CheckInStatus = CheckInStatuses.AvailableForCheckIn;
            else model.CheckInStatus = CheckInStatuses.NotAvailableForCheckIn;

            model.RSVPResponse = new EventAttendeeRSVPModel { Status = thisAttendee.ResponseStatus };
            model.IsOrganizer = accessLevel == EditAccessTypes.Edit;
            model.Attendees = new ViewEventAttendeesModel();
            foreach (var att in e.Attendees.Where(x => x.User.Id != userId))
            {
                model.Attendees.Attendees.Add(new EventAttendeeModel
                {
                    Id = att.Id,
                    UserId = att.User.Id,
                    RSVPStatus = att.ResponseStatus,
                    Name = att.User.Name,
                    IsOrganizer = att.IsOrganizer
                });
            }
            model.Attendees.IsOrganizer = model.IsOrganizer;

            model.Organizers = e.Attendees.Where(x => x.IsOrganizer).Select(y => new EventOrganizerModel { Name = y.User.Name, Id = y.User.Id }).ToList();
            //model.OrganizerName = e.Attendees.First(x => x.IsOrganizer).User.Name;

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
                    CanEdit = mba.MessageBoardItem.PostedBy.Id == userId,
                    IsNew = !mba.Viewed,
                    Id = mba.Id
                });
            }
            model.Messages = model.Messages.OrderByDescending(x => x.PostedBy).ToList();


            model.Notifications = new List<EventNotificationModel>();
            foreach (var ean in this._dbContext.EventAttendeeNotifications.Include("Notification.PostedBy.User").Where(x => x.AttendeeId == thisAttendee.Id))
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

            if (e.RecurrenceId.HasValue)
            {
                model.EventRecurrence =_dbContext.RecurringEvents.Include(x => x.Events).FirstOrDefault(x => x.Id == e.RecurrenceId.Value).Events.Select(y =>
                new RecurrenceListModel
                {
                    Name = y.Name,
                    StartDate = y.StartTime.ToLocalTime(),
                    EndDate = y.EndTime.ToLocalTime(),
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

        public async Task<ViewEventAttendeesModel> GetEventAttendees(long eventId, long activeId)
        {
            var atts = await _dbContext.Events.Include(x => x.Attendees).ThenInclude(x => x.User).Include(x => x.UnregisteredAttendees).FirstOrDefaultAsync(x => x.Id == eventId);
            if (atts == null) throw new Exception("Event Not Found");
            var model = new ViewEventAttendeesModel();
            atts.Attendees.ToList().Where(x => x.User.Id != activeId).ToList().ForEach(x => model.Attendees.Add(new EventAttendeeModel { Id = x.Id, Name = x.User.Name, PhoneNumber = x.User.PhoneNumber, UserId = x.User.Id, RSVPStatus = x.ResponseStatus }));
            atts.UnregisteredAttendees.ToList().ForEach(x => model.Attendees.Add(new EventAttendeeModel { Name = x.Name, Email = x.Email, RSVPStatus = Domain.Enums.RSVPTypes.PendingRegistration }));

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
                if (e.VerificationMethod != AttendanceVerificationMethods.PasswordAndLocation) return SaveDataResponse.FromErrorMessage("This event requires a password to check in");
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
                var eo =_dbContext.EventAttendees.FirstOrDefault(x => x.Id == attendee);
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

        public async Task<List<EventInformationModel>> SearchEvents(string search, long userId)
        {

            IQueryable<Event> events = null;
            if (search == "*")
            {
                events =_dbContext.Events.Include("Attendees.User");
            }
            else
            {
                events =_dbContext.Events.Include("Attendees.User").Where(x => x.Name.ToUpper().Contains(search.ToUpper()));
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
