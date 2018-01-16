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
using ElGroupo.Web.Mail.Models;
using ElGroupo.Web.Filters;
using ElGroupo.Web.Mail;
using System.IO;
using ElGroupo.Web.Classes;
using ElGroupo.Web.Models.Messages;
using ElGroupo.Web.Models.Notifications;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using ElGroupo.Domain.Enums;
using ElGroupo.Web.Models.Shared;

namespace ElGroupo.Web.Controllers
{
    [Route("Events")]
    public class EventsController : ControllerBase
    {




        private SignInManager<User> signInManager;
        //private ElGroupoDbContext dbContext;
        private IEmailService mailService;
        private EventService eventService;
        private readonly UserService _userService = null;
        public EventsController(UserManager<User> userMgr,
                SignInManager<User> signinMgr, ElGroupoDbContext ctx, IEmailService sndr, EventService service, UserService usr) : base(userMgr)
        {

            signInManager = signinMgr;
            //dbContext = ctx;
            mailService = sndr;
            eventService = service;
            _userService = usr;
        }
        [Authorize]
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {

            return View(new CreateEventModel());
        }


        [Authorize]
        [HttpGet, HttpPost]
        [Route("PendingAttendeeList")]
        public async Task<IActionResult> PendingAttendeeList([FromBody]PendingEventAttendeeModel[] models)
        {
            //we will have added the new one on the client side
            return View("_PendingAttendeeList", models);
        }

        [HttpPost, HttpGet]
        [Authorize]
        [Route("ViewEventAttendees/{eventId}", Name = "ViewEventAttendees")]
        public async Task<IActionResult> ViewEventAttendees([FromRoute]long eventId)
        {
            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, eventId);
            var model = await eventService.GetEventAttendees(eventId, accessCheck.userId);
            model.IsOrganizer = accessCheck.accessType == EditAccessTypes.Edit;
            return View("_ViewEventAttendees", model);
        }


        [HttpPost]
        [Authorize]
        [Route("UpdateEventStatus")]
        public async Task<IActionResult> UpdateEventStatus([FromBody]UpdateEventStatusModel model)
        {
            //var eid = Convert.ToInt64(model["eventId"]);
            //var status = (EventStatus)Enum.Parse(typeof(EventStatus), model["status"]);
            //var updateRecurring = bool.Parse(model["updateRecurring"]);
            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, model.EventId);
            if (accessCheck.accessType != EditAccessTypes.Edit) return BadRequest(new { message = "unauthorized" });
            var response = await eventService.UpdateEventStatus(model);
            if (response.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { message = response.ErrorMessage });
            }

        }

        [HttpPost]
        [Authorize]
        [Route("SendRSVPReminders")]
        public async Task<IActionResult> SendRSVPReminders([FromBody]SendRSVPRequestModel model)
        {
            //var eid = Convert.ToInt64(model["eventId"]);
            //var status = (EventStatus)Enum.Parse(typeof(EventStatus), model["status"]);
            //var updateRecurring = bool.Parse(model["updateRecurring"]);
            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, model.EventId);
            if (accessCheck.accessType != EditAccessTypes.Edit) return BadRequest(new { message = "unauthorized" });
            var response = await eventService.SendRSVPReminders(model);
            if (response.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { message = response.ErrorMessage });
            }

        }




        [HttpPost]
        [Authorize]
        [Route("SavePendingAttendees")]
        public async Task<IActionResult> SavePendingAttendees([FromBody]SavePendingAttendeesModel model)
        {

            var user = await CurrentUser();
            var response = model.UpdateRecurring ? await eventService.UpdateRecurringEventAttendees(model) : await eventService.UpdateEventAttendees(model);
            if (response.Success)
            {
                return RedirectToRoute("ViewEventAttendees", new { eventId = model.EventId });
            }
            else
            {
                return BadRequest(new { message = response.ErrorMessage });
            }
        }


        [HttpPost]
        [Authorize]
        [Route("RemoveEventAttendee")]
        public async Task<IActionResult> RemoveEventAttendee([FromBody]RemoveAttendeeModel model)
        {

            var user = await CurrentUser();
            var response = model.UpdateRecurring ? await eventService.RemoveRecurringEventAttendee(model) : await eventService.RemoveEventAttendee(model);
            if (response.Success)
            {
                return RedirectToRoute("ViewEventAttendees", new { eventId = model.EventId });
            }
            else
            {
                return BadRequest(new { message = response.ErrorMessage });
            }
        }

        //[Authorize]
        //[Route("AddPendingAttendee")]
        //public async Task<IActionResult> AddPendingAttendee([FromBody]PendingEventAttendeeModel[] models)
        //{
        //    return View("_PendingAttendeeList", models);
        //}


        [HttpPost]
        [Authorize]
        [Route("UpdateRSVPStatus")]
        public async Task<IActionResult> UpdateRSVPStatus([FromBody]UpdateRSVPStatusModel model)
        {

            var user = await CurrentUser();
            var response = model.UpdateRecurring ? await this.eventService.UpdateRecurringRSVP(user, model) : await this.eventService.UpdateRSVP(user, model);
            if (response.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { error = response.ErrorMessage });
            }


        }

        [HttpPost]
        [Authorize]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateEventModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await CurrentUser();
                SaveDataResponse response = null;
                if (!model.IsRecurring) response = await this.eventService.CreateEvent(model, user.Id);
                else response = await this.eventService.CreateRecurringEvent(model, user.Id);
                if (response.Success)
                {
                    return RedirectToRoute("ViewEvent", new { eid = Convert.ToInt64(response.ResponseData) });
                }
                else
                {
                    return BadRequest(new { error = response.ErrorMessage });
                }

                //move on to Contacts
                //now lets just redirect to the normal view/edit view

            }
            return View(model);

        }

        [Authorize(Roles = ("admin"))]
        [HttpGet("Admin")]
        public async Task<IActionResult> Admin()
        {

            return View(null);
        }

        [Authorize]
        [HttpGet]
        [Route("Search/{search}")]
        public async Task<IActionResult> Search([FromRoute]string search)
        {
            var user = await CurrentUser();
            var list = await eventService.SearchEvents(search, user.Id);
            return PartialView("_EventList", list.OrderBy(x => x.Name));
        }




        private async Task SendUnregisteredEventAttendeeEmail(string name, string email, string fromName, Guid token, Event e)
        {
            var url = Url.Action("Create", "Account", new { id = token }, HttpContext.Request.Scheme);

            var model = new EventUnregisteredAttendeeMailModel
            {
                Recipient = name,
                EventName = e.Name,
                Location = e.LocationName,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                CallbackUrl = url
            };
            var metadata = new MailMetadata
            {
                To = new List<string> { email },
                Subject = "Welcome to ElGroupo!  You'be been invited to a new event!"
            };

            await this.mailService.SendEmail(metadata, model);
        }


        [Authorize]
        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> SaveEdits([FromForm]EventEditModel model)
        {
            return RedirectToAction("Contacts", new { id = model.Id });
        }





        //[Authorize]
        //[HttpGet]
        //[ServiceFilter(typeof(EventOrganizerFilterAttribute))]
        //[Route("{eid}/Edit", Name = "editevent")]
        //public async Task<IActionResult> Edit([FromRoute]long eid)
        //{
        //    if (!await CheckEventExists(eid)) return View("../Shared/NotFound");
        //    if (await CheckEventAccess(eid) != EditAccessTypes.Edit) return View("../Shared/AccessDenied");
        //    var model = await this.eventService.GetEventEditModel(eid);


        //    return View(model);
        //}


        [Authorize]
        [HttpGet, HttpPost]
        [Route("{eid}/View", Name = "ViewEvent")]
        public async Task<IActionResult> View([FromRoute]long eid)
        {
            var accessLevel = await eventService.CheckEventAccess(HttpContext.User, eid);
            var model = await this.eventService.GetEventViewModel(eid, accessLevel.userId, accessLevel.accessType);
            return View(model);
        }

        [Authorize]
        [HttpGet, HttpPost]
        [Route("{eid}/ViewNew", Name = "ViewEventNew")]
        public async Task<IActionResult> ViewNew([FromRoute]long eid)
        {
            var accessLevel = await eventService.CheckEventAccess(HttpContext.User, eid);
            var model = await this.eventService.GetEventViewModel(eid, accessLevel.userId, accessLevel.accessType);
            return View("View_New", model);
        }

        [Authorize]
        [HttpGet]
        [Route("{eid}/CheckIn", Name = "CheckIn")]
        public async Task<IActionResult> CheckIn([FromRoute]long eid)
        {
            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, eid);
            if (accessCheck.accessType == EditAccessTypes.None) return BadRequest();
            var model = await this.eventService.GetEventCheckInModel(eid);
            return View(model);
        }


        [Authorize]
        [HttpPost]
        [Route("CheckInLocation", Name = "CheckInLocation")]
        public async Task<IActionResult> CheckInLocation([FromBody]LocationCheckInModel model)
        {
            var user = await CurrentUser();

            if (await eventService.CheckEventAccess(HttpContext.User, model.EventId, user.Id) == EditAccessTypes.None) return BadRequest(new { message = "unauthorized event" });
            var response = await eventService.CheckInLocation(user.Id, model.EventId, model.CoordinateX, model.CoordinateY);
            if (response.Success)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = response.ErrorMessage });
            }
            //var accessLevel = await CheckEventAccess(eid);
            //var user = await CurrentUser();
            ////coords
            //var model = await this.eventService.GetEventCheckInModel(eid);
            //return View(model);
        }
        [Authorize]
        [HttpPost]
        [Route("CheckInPassword", Name = "CheckInPassword")]
        public async Task<IActionResult> CheckInPassword([FromBody]PasswordCheckInModel model)
        {
            var user = await CurrentUser();


            if (await eventService.CheckEventAccess(HttpContext.User, model.EventId, user.Id) == EditAccessTypes.None) return BadRequest(new { message = "unauthorized event" });
            var response = await eventService.CheckInPassword(user.Id, model.EventId, model.Password);
            if (response.Success)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = response.ErrorMessage });
            }
        }




        /// <summary>
        /// when user saves detail edits - we refresh with an updated view
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [HttpPost]
        [Route("{eid}/ViewLocation")]
        public async Task<IActionResult> ViewLocationDetails([FromRoute]long eid)
        {
            var accessLevel = await eventService.CheckEventAccess(HttpContext.User, eid);
            var model = await this.eventService.ViewEventLocationDetails(eid);
            return View("_ViewLocationDetails", model);
        }

        /// <summary>
        /// when user clicks "edit details" - load the edit form
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("{eid}/EditLocation")]
        public async Task<IActionResult> EditLocationDetails([FromRoute]long eid)
        {
            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, eid);
            if (accessCheck.accessType != EditAccessTypes.Edit) return BadRequest();
            var model = await this.eventService.EditEventLocation(eid);
            return View("_EditEventLocation", model);
        }

        [Authorize]
        [HttpGet]
        [Route("{eid}/LoadMessages")]
        public async Task<IActionResult> LoadMessages([FromRoute]long eid)
        {
            var user = await CurrentUser();
            var accessLevel = await eventService.CheckEventAccess(HttpContext.User, eid);
            //if (accessLevel != EditAccessTypes.Edit) return BadRequest();
            var model = await this.eventService.GetEventMessages(eid, user.Id);
            return View("_ViewEventMessages", model);
        }

        [Authorize]
        [HttpGet]
        [Route("{eid}/LoadNotifications")]
        public async Task<IActionResult> LoadNotifications([FromRoute]long eid)
        {

            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, eid);
            //if (accessLevel != EditAccessTypes.Edit) return BadRequest();
            var model = await this.eventService.GetEventNotifications(eid, accessCheck.userId, accessCheck.accessType);
            return View("_ViewEventNotifications", model);
        }


        /// <summary>
        /// when user clicks "edit details" - load the edit form
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("{eid}/EditLocationNew")]
        public async Task<IActionResult> EditLocationDetailsNew([FromRoute]long eid)
        {
            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, eid);
            if (accessCheck.accessType != EditAccessTypes.Edit) return BadRequest();
            var model = await this.eventService.EditEventLocation(eid);
            return View("_EditEventLocation_New", model);
        }

        [Authorize]
        [HttpGet]
        [Route("{eid}/EditDates")]
        public async Task<IActionResult> EditEventDates([FromRoute]long eid)
        {
            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, eid);
            if (accessCheck.accessType != EditAccessTypes.Edit) return BadRequest();
            var model = await this.eventService.EditEventDates(eid, accessCheck.userId);
            return View("_EditEventDates", model);
        }

        [Authorize]
        [HttpGet]
        [Route("{eid}/ViewDates")]
        public async Task<IActionResult> ViewEventDates([FromRoute]long eid)
        {
            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, eid);
            if (accessCheck.accessType != EditAccessTypes.Edit) return BadRequest();
            var model = await this.eventService.ViewEventDates(eid, accessCheck.userId);
            return View("_ViewEventDates", model);
        }
        /// <summary>
        /// when user clicks "edit location details" - load the edit form
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("EditLocationNew")]
        public async Task<IActionResult> EditLocationDetailsNew([FromBody]EditEventLocationModel model)
        {
            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, model.EventId);
            if (accessCheck.accessType != EditAccessTypes.Edit) return BadRequest();

            var response = model.UpdateRecurring ? await this.eventService.UpdateRecurringEventLocation(model) : await this.eventService.UpdateEventLocation(model);
            if (response.Success)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = response.ErrorMessage });
            }
        }

        /// <summary>
        /// when user clicks "edit location details" - load the edit form
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("EditLocation")]
        public async Task<IActionResult> EditLocationDetails([FromBody]EditEventLocationModel model)
        {
            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, model.EventId);
            if (accessCheck.accessType != EditAccessTypes.Edit) return BadRequest();

            var response = model.UpdateRecurring ? await this.eventService.UpdateRecurringEventLocation(model) : await this.eventService.UpdateEventLocation(model);
            return RedirectToAction("ViewLocationDetails", new { eid = model.EventId });
        }















        /// <summary>
        /// when user saves detail edits - we refresh with an updated view
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [HttpPost]
        [Route("{eid}/ViewDetails")]
        public async Task<IActionResult> ViewDetails([FromRoute]long eid)
        {
            var accessLevel = await eventService.CheckEventAccess(HttpContext.User, eid);
            var user = await CurrentUser();
            var model = await this.eventService.ViewEventDetails(eid, user.Id);
            return View("_ViewEventDetails", model);
        }

        /// <summary>
        /// when user clicks "edit details" - load the edit form
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("{eid}/EditDetails")]
        public async Task<IActionResult> EditDetails([FromRoute]long eid)
        {
            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, eid);
            if (accessCheck.accessType != EditAccessTypes.Edit) return BadRequest();

            var model = await this.eventService.EditEventDetails(eid, accessCheck.userId);

            return View("_EditEventDetails", model);
        }

        /// <summary>
        /// when user clicks "edit details" - load the edit form
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("EditDetails")]
        public async Task<IActionResult> EditDetails([FromBody]EditEventDetailsModel model)
        {

            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, model.EventId);
            if (accessCheck.accessType != EditAccessTypes.Edit) return BadRequest();
            if (model.UpdateRecurring) await this.eventService.UpdateRecurringEventDetails(model);
            else await this.eventService.UpdateEventDetails(model, accessCheck.userId);
            return RedirectToAction("ViewDetails", new { eid = model.EventId });
        }


        //[Authorize]
        //[HttpPost]
        //[Route("Organizers/{eid}/Add")]
        //public async Task<IActionResult> AddEventOrganizer([FromRoute]long eid, [FromBody]AddEventOrganizerModel model)
        //{
        //    await this.eventService.AddEventAttendee(eid, model.UserId, model.Owner);
        //    await SendEventOrganizerEmail(model.UserId, eid);
        //    return RedirectToAction("OrganizersList", new { id = eid });
        //}

        //[Authorize]
        //[HttpPost]
        //[Route("AddRegisteredAttendee", Name = "AddRegisteredAttendee")]
        //public async Task<IActionResult> AddRegisteredEventAttendee([FromBody]AddRegisteredEventAttendeeModel model)
        //{
        //    await this.eventService.AddEventAttendee(model);
        //    var e = dbContext.Find<Event>(model.eventId);
        //    if (e.Status == Domain.Enums.EventStatus.Active)
        //    {
        //        await SendEventAttendeeEmail(model.userId, model.eventId);
        //    }

        //    return RedirectToAction("AttendeesList", new { id = model.eventId });
        //}

        private MailMetadata CreateMailMetadata(string toEmailAddress, string subject)
        {
            var metadata = new MailMetadata
            {
                To = new List<string> { toEmailAddress },
                Subject = subject
            };
            return metadata;
        }

        [Authorize]
        [HttpPost]
        [Route("AddUnregisteredAttendee")]
        public async Task<IActionResult> AddUnregisteredEventAttendee([FromBody]UnregisteredEventAttendeeModel model)
        {
            var e = await eventService.GetEvent(model.EventId);
            var u = await CurrentUser();

            //make sure this user doesn't exist in the system
            var userCheck = await _userService.GetUserByEmail(model.Email);
            if (userCheck != null)
            {
                return RedirectToAction("AddRegisteredAttendee", new { eid = e.Id, uid = userCheck.Id });
            }

            var token = await this.eventService.AddUnregisteredAttendee(u, e, model);
            if (e.Status == Domain.Enums.EventStatus.Active) await this.mailService.SendEmail(CreateMailMetadata(model.Email, "Welcome to ElGroupo!  You'be been invited to a new event!"), new EventUnregisteredAttendeeMailModel
            {
                Recipient = model.Name,
                EventName = e.Name,
                Location = e.LocationName,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                CallbackUrl = Url.Action("Create", "Account", new { id = (Guid)token.ResponseData }, HttpContext.Request.Scheme)
            });

            return RedirectToAction("AttendeesList", new { id = model.EventId });
        }


        [Authorize]
        [ServiceFilter(typeof(EventOrganizerFilterAttribute))]
        [HttpDelete("{eid}")]
        public async Task<IActionResult> DeleteEvent([FromRoute]long eid)
        {
            var response = await this.eventService.DeleteEvent(eid);
            if (response.Success)
            {
                var redirectUrl = Url.Action("Dashboard", "Home", new { }, HttpContext.Request.Scheme);
                return Json(new { url = redirectUrl });
            }
            else
            {

                return BadRequest(new { message = response.ErrorMessage });
            }


        }
        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteEvent([FromBody]EventModelBase model)
        {
            SaveDataResponse response = model.UpdateRecurring ? await this.eventService.DeleteRecurringEvent(model.EventId) : await this.eventService.DeleteEvent(model.EventId);
            if (response.Success)
            {
                var redirectUrl = Url.Action("Dashboard", "Home", new { }, HttpContext.Request.Scheme);
                return Json(new { url = redirectUrl });
            }
            else
            {
                return BadRequest(new { message = response.ErrorMessage });
            }


        }


        //[Authorize]
        //[HttpPost]
        //[Route("Organizers/{eid}/Delete/{oid}")]
        //public async Task<IActionResult> DeleteEventOrganizer([FromRoute]long eid, [FromRoute]long oid)
        //{
        //    if (await CheckEventAccess(eid) != EditAccessTypes.Edit) return View("../Shared/AccessDenied");
        //    if (await this.eventService.DeleteEventOrganizer(oid, eid))
        //    {
        //        return RedirectToAction("OrganizersList", new { id = eid });
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }

        //}



        [Authorize]
        [HttpPost]
        [Route("Organizers/{eid}/Delete/{oid}")]
        public async Task<IActionResult> DeleteEventAttendee([FromRoute]long eid, [FromRoute]long oid)
        {
            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, eid);
            if (accessCheck.accessType != EditAccessTypes.Edit) return View("../Shared/AccessDenied");
            var response = await this.eventService.DeleteEventAttendee(oid, eid);
            if (response.Success)
            {
                return RedirectToAction("AttendeesList", new { id = eid });
            }
            else
            {
                return BadRequest(new { message = response.ErrorMessage });
            }


        }







        [Authorize]
        [HttpGet]
        [Route("Contacts/{id}")]
        public async Task<IActionResult> Contacts([FromRoute]long id)
        {
            var accessCheck = await eventService.CheckEventAccess(HttpContext.User, id);
            if (accessCheck.accessType != EditAccessTypes.Edit) return View("../Shared/AccessDenied");
            var user = await CurrentUser();
            return View(await this.eventService.GetEventContacts(id, user.Id));

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
