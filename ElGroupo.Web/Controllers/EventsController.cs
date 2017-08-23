using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Domain;
using Microsoft.AspNetCore.Identity;
using ElGroupo.Web.Models.Events;
using Microsoft.EntityFrameworkCore;
using ElGroupo.Domain.Data;
using ElGroupo.Web.Services;
using System.IO;

namespace ElGroupo.Web.Controllers
{
    [Route("Events")]
    public class EventsController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private ElGroupoDbContext dbContext;
        private IEmailSender emailSender;
        public EventsController(UserManager<User> userMgr,
                SignInManager<User> signinMgr, ElGroupoDbContext ctx, IEmailSender sndr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            dbContext = ctx;
            emailSender = sndr;

        }
        [Authorize]
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateEventModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this.userManager.GetUserAsync(HttpContext.User);
                var e = new Event();
                e.Organizers.Add(new EventOrganizer
                {
                    User = user,
                    Owner = true,
                    Event = e
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

                //move on to Contacts

                return RedirectToAction("Contacts", new { id = e.Id });
            }
            return View(model);

        }

        [Authorize(Roles = ("admin"))]
        [HttpGet("Admin")]
        public async Task<IActionResult> Admin()
        {
            //var modelList = new List<UserInformationModel>();
            //foreach(var u in dbContext.Users)
            //{
            //    modelList.Add(new UserInformationModel
            //    {
            //        Name = u.Name,
            //        Email = u.Email,
            //        UserName = u.UserName,
            //        Id = u.Id
            //    });
            //}
            return View(null);
        }

        [Authorize]
        [HttpGet]
        [Route("Search/{search}")]
        public async Task<IActionResult> Search([FromRoute]string search)
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
            await events.ForEachAsync(x => list.Add(new EventInformationModel {
                Draft = x.SavedAsDraft,
                StartDate = x.StartTime,
                EndDate = x.EndTime,
                Id = x.Id,
                Name = x.Name,
                OrganizerName = x.Organizers.First(y => y.Owner).User.Name }));
            return PartialView("_EventList", list.OrderBy(x => x.Name));
        }
        private async Task SendEventOrganizerEmail(User u, Event e)
        {
            var url = Url.Action("Edit", "Events", new { eid = e.Id }, HttpContext.Request.Scheme);

            var msg = "Hi " + u.Name + ",";
            msg += "<br/>";
            msg += "You've been added as an event organizer to " + e.Name + ", which is taking place on " + e.StartTime.Date.ToString("d") + " from " + e.StartTime.TimeOfDay.ToString() + " to " + e.EndTime.TimeOfDay.ToString() + ".";
            msg += "Click on this <a href='" + url + "'>link</a> to access the event details";
            //<a href='{callbackUrl}'>link</a>
            msg += "<br/>";
            msg += "Thanks,";
            msg += "<br/>";
            msg += "The ElGroupo Team";

            await emailSender.SendEmailAsync(u.Email, "You've been added as an event organizer!", msg);
        }

        private async Task SendEventAttendeeEmail(User u, Event e)
        {
            var url = Url.Action("View", "Events", new { eid = e.Id }, HttpContext.Request.Scheme);

            var msg = "Hi " + u.Name + ",";
            msg += "<br/>";
            msg += "You've been invited to a killer awesome event " + e.Name + ", which is taking place on " + e.StartTime.Date.ToString("d") + " from " + e.StartTime.TimeOfDay.ToString() + " to " + e.EndTime.TimeOfDay.ToString() + ".";
            msg += "Click on this <a href='" + url + "'>link</a> to access the event details";
            msg += "<br/>";
            msg += "Thanks,";
            msg += "<br/>";
            msg += "The ElGroupo Team";

            await emailSender.SendEmailAsync(u.Email, "You've been invited to a new event!", msg);
        }

        private async Task SendUnregisteredEventAttendeeEmail(string name, string email, string fromName, Guid token, Event e)
        {
            var url = Url.Action("Create", "Account", new { id = token }, HttpContext.Request.Scheme);

            var msg = "Hi " + name + ",";
            msg += "<br/>";
            msg += fromName + " has invited you a killer awesome event " + e.Name + ", which is taking place on " + e.StartTime.Date.ToString("d") + " from " + e.StartTime.TimeOfDay.ToString() + " to " + e.EndTime.TimeOfDay.ToString() + ".";
            msg += "<a href='" + url + "'>Click on this link to sign up for ElGroupo and access the event details</a>";
            msg += "<br/>";
            msg += "Thanks,";
            msg += "<br/>";
            msg += "The ElGroupo Team";

            await emailSender.SendEmailAsync(email, "You've been invited to a new event!", msg);
        }


        [Authorize]
        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> SaveEdits([FromForm]EditEventModel model)
        {
            return RedirectToAction("Contacts", new { id = model.Id });
            //return RedirectToAction("Edit", new { eid = model.Id });
        }

        [Authorize]
        [HttpGet]
        [Route("{eid}/Edit", Name = "editevent")]
        public async Task<IActionResult> Edit([FromRoute]long eid)
        {
            var user = this.userManager.GetUserAsync(HttpContext.User);
            var e = await dbContext.Events.Include("Organizers.User").FirstOrDefaultAsync(x => x.Id == eid);
            if (e == null) return View("../Shared/NotFound");
            if (!e.Organizers.Any(x => x.User.Id == user.Id) && !HttpContext.User.IsInRole("admin"))
            {
                //illegal access
                return View("../Shared/AccessDenied");
            }
            var model = new EditEventModel();
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


            return View(model);
        }
        [Authorize]
        [HttpGet]
        [Route("{eid}/View")]
        public async Task<IActionResult> View([FromRoute]long eid)
        {
            var user = await this.userManager.GetUserAsync(HttpContext.User);
            var e = await dbContext.Events.Include("Attendees.User").FirstOrDefaultAsync(x => x.Id == eid);
            if (e == null) return View("../Shared/NotFound");
            if (!e.Attendees.Any(x => x.User.Id == user.Id) && !HttpContext.User.IsInRole("admin"))
            {
                //illegal access
                return View("../Shared/AccessDenied");
            }
            var model = new ViewEventModel();

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


            return View(model);
        }


        [Authorize]
        [HttpPost]
        [Route("Organizers/{eid}/Add")]
        public async Task<IActionResult> AddEventOrganizer([FromRoute]long eid, [FromBody]AddEventOrganizerModel model)
        {
            var e = dbContext.Events.FirstOrDefault(x => x.Id == eid);
            var u = dbContext.Users.FirstOrDefault(x => x.Id == model.UserId);
            var eo = new EventOrganizer { User = u, Event = e, Owner = model.Owner };
            dbContext.EventOrganizers.Add(eo);
            await dbContext.SaveChangesAsync();
            await SendEventOrganizerEmail(u, e);
            return RedirectToAction("OrganizersList", new { id = eid });
        }

        [Authorize]
        [HttpPost]
        [Route("Attendees/{eid}/Add/{uid}")]
        public async Task<IActionResult> AddEventAttendee([FromRoute]long eid, [FromRoute]int uid)
        {
            var e = dbContext.Events.FirstOrDefault(x => x.Id == eid);
            var u = dbContext.Users.FirstOrDefault(x => x.Id == uid);
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
            await SendEventAttendeeEmail(u, e);
            return RedirectToAction("AttendeesList", new { id = eid });
        }

        [Authorize]
        [HttpPost]
        [Route("Attendees/{eid}/AddUnregistered")]
        public async Task<IActionResult> AddUnregisteredEventAttendee([FromRoute]long eid, [FromBody]UnregisteredEventAttendeeModel model)
        {
            var e = dbContext.Events.FirstOrDefault(x => x.Id == eid);
            var u = await userManager.GetUserAsync(HttpContext.User);

            //make sure this user doesn't exist in the system
            var userCheck = await userManager.FindByEmailAsync(model.Email);
            if (userCheck != null)
            {
                return RedirectToAction("AddEventAttendee", new { eid = e.Id, uid = userCheck.Id });
            }

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
            await SendUnregisteredEventAttendeeEmail(model.Name, model.Email, u.Name, uea.RegisterToken, e);
            return RedirectToAction("AttendeesList", new { id = eid });
        }



        [Authorize]
        [HttpPost]
        [Route("Organizers/{eid}/Delete/{oid}")]
        public async Task<IActionResult> DeleteEventOrganizer([FromRoute]long eid, [FromRoute]long oid)
        {
            var eo = dbContext.EventOrganizers.FirstOrDefault(x => x.Id == oid);
            if (eo.EventId != eid) return BadRequest();
            dbContext.EventOrganizers.Remove(eo);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("OrganizersList", new { id = eid });
        }



        [Authorize]
        [HttpPost]
        [Route("Organizers/{eid}/Delete/{oid}")]
        public async Task<IActionResult> DeleteEventAttendee([FromRoute]long eid, [FromRoute]long oid)
        {
            var eo = dbContext.EventAttendees.FirstOrDefault(x => x.Id == oid);
            if (eo.EventId != eid) return BadRequest();
            dbContext.EventAttendees.Remove(eo);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("AttendeesList", new { id = eid });
        }

        [Authorize]
        [HttpGet, HttpPost]
        [Route("Contacts/{id}/Organizers")]
        public async Task<IActionResult> OrganizersList([FromRoute]long id)
        {
            var orgs = await dbContext.Events.Include("Organizers.User").FirstOrDefaultAsync(x => x.Id == id);
            if (orgs == null) return BadRequest("bears");
            var model = new List<EventOrganizerModel>();
            orgs.Organizers.ToList().ForEach(x => model.Add(new EventOrganizerModel { Id = x.Id, Name = x.User.Name, Owner = x.Owner, PhoneNumber = x.User.PhoneNumber, UserId = x.User.Id }));
            return View("_Organizers", model);
        }

        [Authorize]
        [HttpGet, HttpPost]
        [Route("Contacts/{id}/Attendees")]
        public async Task<IActionResult> AttendeesList([FromRoute]long id)
        {
            var atts = await dbContext.Events.Include("Attendees.User").Include("UnregisteredAttendees").FirstOrDefaultAsync(x => x.Id == id);
            if (atts == null) return BadRequest("bears");
            var model = new List<EventAttendeeModel>();
            atts.Attendees.ToList().ForEach(x => model.Add(new EventAttendeeModel { Id = x.Id, Name = x.User.Name, PhoneNumber = x.User.PhoneNumber, UserId = x.User.Id, RSVPStatus = x.ResponseStatus }));
            atts.UnregisteredAttendees.ToList().ForEach(x => model.Add(new EventAttendeeModel { Name = x.Name, Email = x.Email, RSVPStatus = Domain.Enums.RSVPTypes.PendingRegistration }));
            return View("_Attendees", model.OrderBy(x => x.Name));
        }


        [Authorize]
        [HttpGet]
        [Route("Contacts/{id}")]
        public async Task<IActionResult> Contacts([FromRoute]long id)
        {
            var user = this.userManager.GetUserAsync(HttpContext.User);
            var e = await dbContext.Events.Include("Organizers.User").Include("Attendees.User").FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return View("../Shared/NotFound");
            if (!e.Organizers.Any(x => x.User.Id == user.Id) && !HttpContext.User.IsInRole("admin"))
            {
                //illegal access
                return View("../Shared/AccessDenied");
            }


            var model = new EventContactsModel();
            model.Event.Name = e.Name;
            model.Event.EndDate = e.EndTime;
            model.Event.StartDate = e.StartTime;
            model.Event.Id = e.Id;


            foreach (var att in e.Organizers.Where(x => x.User.Id != user.Id))
            {
                model.Organizers.Add(new EventOrganizerModel
                {
                    Name = att.User.Name,
                    Id = att.Id,
                    Owner = att.Owner,
                    UserId = att.User.Id,
                    PhoneNumber = att.User.PhoneNumber
                });
            }

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

            return View(model);

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
