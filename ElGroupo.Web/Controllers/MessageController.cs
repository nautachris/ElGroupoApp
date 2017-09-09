using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Domain;
using Microsoft.AspNetCore.Identity;
using ElGroupo.Web.Models.Messages;
using Microsoft.EntityFrameworkCore;
using ElGroupo.Domain.Data;
using ElGroupo.Web.Services;
using System.IO;

namespace ElGroupo.Web.Controllers
{
    [Authorize]
    [Route("Messages")]
    public class MessageController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private ElGroupoDbContext dbContext;
        private IEmailSender emailSender;
        public MessageController(UserManager<User> userMgr,
                SignInManager<User> signinMgr, ElGroupoDbContext ctx, IEmailSender sndr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            dbContext = ctx;
            emailSender = sndr;

        }


        [HttpPost]
        [Authorize]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody]CreateMessageModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this.userManager.GetUserAsync(HttpContext.User);
                var e = await this.dbContext.Events.Include("Attendees.User").FirstAsync(x => x.Id == model.EventId);
                var msg = new MessageBoardItem
                {
                    User = user,
                    Event = e,
                    PostedDate = DateTime.Now,
                    MessageText = model.Text,
                    Subject = model.Subject
                };

                dbContext.MessageBoardItems.Add(msg);

                foreach (var attendee in e.Attendees)
                {
                    dbContext.MessageBoardItemAttendees.Add(new MessageBoardItemAttendee
                    {
                        Attendee = attendee,
                        Viewed = attendee.User.Id == user.Id,
                        MessageBoardItem = msg
                    });
                }


                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var fff = 4;
                }

                //move on to Contacts

                return RedirectToAction("MessageList", new { eventId = e.Id });
            }
            return View(model);

        }




        [HttpGet, HttpPost]
        [Route("Events/{eventId}", Name = "GetMessagesByEventId")]
        public async Task<IActionResult> MessageList([FromRoute]long eventId)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(HttpContext.User);
                var attendee = await this.dbContext.EventAttendees.FirstOrDefaultAsync(x => x.EventId == eventId && x.UserId == user.Id);
                var canEdit = HttpContext.User.IsInRole("admin") || this.dbContext.EventOrganizers.Any(x => x.EventId == eventId && x.UserId == user.Id);
                if (attendee == null) return BadRequest();
                var model = new List<EventMessageModel>();
                foreach (var mba in this.dbContext.MessageBoardItemAttendees.Include("MessageBoardItem.User").Where(x => x.AttendeeId == attendee.Id))
                {
                    model.Add(new EventMessageModel
                    {
                        PostedBy = mba.MessageBoardItem.User.Name,
                        PostedById = mba.MessageBoardItem.User.Id,
                        CanEdit = mba.MessageBoardItem.User.Id == user.Id || canEdit,
                        PostedDate = mba.MessageBoardItem.PostedDate,
                        Subject = mba.MessageBoardItem.Subject,
                        MessageText = mba.MessageBoardItem.MessageText,
                        IsNew = !mba.Viewed,
                        Id = mba.Id
                    });
                }
                return View("../Events/_MessageList", model.OrderByDescending(x => x.PostedDate));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }


        //private async Task SendEventOrganizerEmail(User u, Event e)
        //{
        //    var url = Url.Action("Edit", "Events", new { eid = e.Id }, HttpContext.Request.Scheme);

        //    var msg = "Hi " + u.Name + ",";
        //    msg += "<br/>";
        //    msg += "You've been added as an event organizer to " + e.Name + ", which is taking place on " + e.StartTime.Date.ToString("d") + " from " + e.StartTime.TimeOfDay.ToString() + " to " + e.EndTime.TimeOfDay.ToString() + ".";
        //    msg += "Click on this <a href='" + url + "'>link</a> to access the event details";
        //    //<a href='{callbackUrl}'>link</a>
        //    msg += "<br/>";
        //    msg += "Thanks,";
        //    msg += "<br/>";
        //    msg += "The ElGroupo Team";

        //    await emailSender.SendEmailAsync(u.Email, "You've been added as an event organizer!", msg);
        //}

        //private async Task SendEventAttendeeEmail(User u, Event e)
        //{
        //    var url = Url.Action("View", "Events", new { eid = e.Id }, HttpContext.Request.Scheme);

        //    var msg = "Hi " + u.Name + ",";
        //    msg += "<br/>";
        //    msg += "You've been invited to a killer awesome event " + e.Name + ", which is taking place on " + e.StartTime.Date.ToString("d") + " from " + e.StartTime.TimeOfDay.ToString() + " to " + e.EndTime.TimeOfDay.ToString() + ".";
        //    msg += "Click on this <a href='" + url + "'>link</a> to access the event details";
        //    msg += "<br/>";
        //    msg += "Thanks,";
        //    msg += "<br/>";
        //    msg += "The ElGroupo Team";

        //    await emailSender.SendEmailAsync(u.Email, "You've been invited to a new event!", msg);
        //}



        [HttpGet]
        [Route("SetAsViewed/{messageId}")]
        public async Task<IActionResult> SetMessageAsViewed(long messageId)
        {
            //var user = await this.userManager.GetUserAsync(HttpContext.User);
            var msg = await this.dbContext.MessageBoardItemAttendees.Include("Attendee.User").FirstOrDefaultAsync(x => x.Id == messageId);
            if (msg == null) return BadRequest();
            msg.Viewed = true;
            this.dbContext.Update(msg);
            await this.dbContext.SaveChangesAsync();
            return Json(new { message = "success" });
        }







        [Authorize]
        [HttpDelete]
        [Route("Delete/{messageId}")]
        public async Task<IActionResult> DeleteMessage([FromRoute]long messageId)
        {
            var user = await this.userManager.GetUserAsync(HttpContext.User);
            var msg = await this.dbContext.MessageBoardItemAttendees.Include("Attendee.User").Include("MessageBoardItem.Attendees").FirstOrDefaultAsync(x => x.Id == messageId);

            if (msg.Attendee.User.Id != user.Id || HttpContext.User.IsInRole("admin"))
            {
                return BadRequest();
            }

            foreach (var item in msg.MessageBoardItem.Attendees)
            {
                this.dbContext.Remove(item);
            }
            this.dbContext.Remove(msg.MessageBoardItem);
            await this.dbContext.SaveChangesAsync();
            //var eo = dbContext.EventOrganizers.FirstOrDefault(x => x.Id == oid);
            //if (eo.EventId != eid) return BadRequest();
            //dbContext.EventOrganizers.Remove(eo);
            //await dbContext.SaveChangesAsync();
            //return RedirectToAction("OrganizersList", new { id = eid });
            return RedirectToRoute("GetMessagesByEventId", new { eid = msg.MessageBoardItem.EventId });
        }


    }
}
