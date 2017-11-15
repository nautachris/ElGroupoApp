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


        [HttpGet, HttpPost]
        [Route("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var model = new DashboardModel();
            var allEvents = new List<EventInformationModel>();
            //do we want a third "tab" for events I'm organizing?
            var user = await this.CurrentUser;

            //var test = dbContext.Events.Include(x => x.Recurrence).ToList();
            //var test2 = dbContext.Events.Include(x => x.Recurrence).Select(x => x).ToList();
            var organizedEvents = dbContext.EventAttendees.Include(x=>x.User).Where(x => x.User.Id == user.Id && x.IsOrganizer).Select(x => new { ev = x.Event, rec = x.Event.Recurrence }).ToList();
            
            var invitedEvents = dbContext.EventAttendees.Include(x=>x.User).Include(x=>x.Event).ThenInclude(x=>x.Attendees).ThenInclude(x=>x.User).Include(x=>x.Event).ThenInclude(x=>x.Recurrence).Where(x => x.User.Id == user.Id && !x.IsOrganizer).ToList();
            foreach (var ev in organizedEvents)
            {
                allEvents.Add(new EventInformationModel
                {
                    EndDate = ev.ev.EndTime,
                    StartDate = ev.ev.StartTime,
                    Id = ev.ev.Id,
                    OrganizedByUser = true,
                    IsNew = false,
                    Name = ev.ev.Name,
                    Status = ev.ev.Status,
                    IsRecurring = ev.rec != null
                    
                });
            }
            foreach (var ev in invitedEvents.Where(x => !organizedEvents.Select(y => y.ev.Id).Contains(x.EventId)))
            {
                allEvents.Add(new EventInformationModel
                {
                    EndDate = ev.Event.EndTime,
                    StartDate = ev.Event.StartTime,
                    Id = ev.Event.Id,
                    OrganizedByUser = false,
                    IsNew = !ev.Viewed,
                    Name = ev.Event.Name,
                    OrganizerName = ev.Event.Attendees.First(x=>x.IsOrganizer).User.Name,
                    Status = ev.Event.Status,
                    RSVPStatus = ev.ResponseStatus,
                    IsRecurring = ev.Event.Recurrence != null,
                    RSVPRequested = ev.ShowRSVPReminder == true
                });
            }

            model.Drafts = allEvents.Where(x => x.OrganizedByUser && x.Status == Domain.Enums.EventStatus.Draft).OrderBy(x => x.StartDate).ToList();
            model.PastEvents = allEvents.Where(x => x.Status != Domain.Enums.EventStatus.Draft && x.StartDate < DateTime.Now).OrderBy(x => x.StartDate).ToList();
            model.FutureEvents = allEvents.Where(x => x.Status != Domain.Enums.EventStatus.Draft && x.StartDate >= DateTime.Now).OrderBy(x => x.StartDate).ToList();
            model.RSVPRequestedEvents = allEvents.Where(x => x.RSVPRequested).ToList();
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
            return View(await this.CurrentUser);
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

        private Task<User> CurrentUser => _userManager.GetUserAsync(HttpContext.User);

    }
}
