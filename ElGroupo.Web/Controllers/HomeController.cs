using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ElGroupo.Domain;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using ElGroupo.Web.Models.Home;
using ElGroupo.Domain.Data;
using Microsoft.EntityFrameworkCore;
using ElGroupo.Web.Models.Events;
using ElGroupo.Domain.Enums;

namespace ElGroupo.Web.Controllers
{
    [Route("Home")]
    [Authorize]
    public class HomeController : Controller
    {
        private ElGroupoDbContext dbContext;
        private UserManager<User> _userManager;

        public HomeController(UserManager<User> userManager, ElGroupoDbContext ctx)
        {
            this._userManager = userManager;
            dbContext = ctx;
        }


        private CheckInStatuses GetCheckInStatus(Event e, EventAttendee ea)
        {
            if (ea.CheckedIn) return CheckInStatuses.CheckInSuccessful;
            if (e.EndTime < DateTime.Now.ToUniversalTime()) return CheckInStatuses.CheckInExpired;
            if (e.StartTime.AddMinutes(e.CheckInTimeTolerance * -1) <= DateTime.Now.ToUniversalTime()) return CheckInStatuses.AvailableForCheckIn;
            return CheckInStatuses.NotAvailableForCheckIn;
        }


        private List<EventInformationModel> GroupEventsByRecurrence(List<EventInformationModel> events)
        {
            //var recurring = events.Where(x => x.RecurrenceId.HasValue);
            var outList = new List<EventInformationModel>();
            outList.AddRange(events.Where(x => !x.RecurrenceId.HasValue));
            foreach(var ev in events.Where(x => x.RecurrenceId.HasValue).GroupBy(x => x.RecurrenceId.Value))
            {
                var parentEvent = ev.OrderBy(x => x.StartDate).First();
                parentEvent.Recurrences = ev.OrderBy(x => x.StartDate).Skip(1).ToList();
                foreach (var r in parentEvent.Recurrences) r.IsRecurrenceItem = true;
                outList.Add(parentEvent);
            }
            
            return outList.OrderBy(x=>x.StartDate).ToList();
        }


        [HttpGet, HttpPost]
        [Route("Dashboard")]
        public async Task<IActionResult> Dashboard([FromQuery]bool confirmTimeZone = false)
        {
            var model = new DashboardModel();
            var allEvents = new List<EventInformationModel>();
            //do we want a third "tab" for events I'm organizing?
            var user = await this.CurrentUser();
            var organizedEvents = dbContext.EventAttendees.Include(x => x.User).Where(x => x.User.Id == user.Id && x.Active && x.IsOrganizer).Select(x =>
            new
            {
                ea = x,
                ev = x.Event,
                rec = x.Event.Recurrence
            }).ToList();
            //e.StartTime.ToLocalTime().DayOfWeek.ToString() + " " + e.StartTime.ToLocalTime().ToString("d") + " " + e.StartTime.ToLocalTime().ToString("t")
            var invitedEvents = dbContext.EventAttendees.Include(x => x.User).Include(x => x.Event).ThenInclude(x => x.Attendees).ThenInclude(x => x.User).Include(x => x.Event).ThenInclude(x => x.Recurrence).Where(x => x.User.Id == user.Id && x.Active && !x.IsOrganizer).ToList();
            foreach (var ev in organizedEvents)
            {
                allEvents.Add(new EventInformationModel
                {
                    EventAttendeeId = ev.ea.Id,
                    EndDate = ev.ev.EndTime,
                    StartDate = ev.ev.StartTime,
                    DateText = ev.ev.GetSimpleDateText(user.TimeZoneId),
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
                    DateText = ev.Event.GetSimpleDateText(user.TimeZoneId),
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
            model.TimeZoneChanged = confirmTimeZone;
            return View(model);
        }

        [Route("Index")]
        [AllowAnonymous]
        public async Task<IActionResult> Index() => RedirectToAction("Login", "Account");
        //public async Task<IActionResult> Index() => View(await this.CurrentUser);

        //[Authorize(Roles = "Users")]
        //this corresponds to the policy name defined in Startup.ConfigureServices.AddAuthorization
        //[Authorize(Policy = "DCUsers")]
        //public IActionResult OtherAction() => View("Index", GetData(nameof(OtherAction)));


        ////bob cannot access this b/c the requirement for the NotBob policy
        //[Authorize(Policy = "NotBob")]
        //public IActionResult NotBob() => View("Index", GetData(nameof(NotBob)));

        //private Dictionary<string, object> GetData(string actionName) => new Dictionary<string, object>
        //{
        //    ["Action"] = actionName,
        //    ["User"] = HttpContext.User.Identity.Name,
        //    ["Authenticated"] = HttpContext.User.Identity.IsAuthenticated,
        //    ["Auth Type"] = HttpContext.User.Identity.AuthenticationType,
        //    ["In Users Role"] = HttpContext.User.IsInRole("Users"),
        //    ["City"] = this.CurrentUser.Result.City,
        //    ["Qualification"] = this.CurrentUser.Result.Qualifications
        //};

        [Authorize]
        public async Task<IActionResult> UserProps()
        {
            return View(await this.CurrentUser());
        }

        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> UserProps([Required] Cities city, [Required] QualificationLevels qualifications)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        User user = await this.CurrentUser;
        //        user.City = city;
        //        user.Qualifications = qualifications;
        //        await _userManager.UpdateAsync(user);
        //        return RedirectToAction("Index");
        //    }
        //    return View(await this.CurrentUser);
        //}

        private async Task<User> CurrentUser()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        }

    }
}
