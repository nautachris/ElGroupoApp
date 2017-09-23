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
    public class MessagesController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private ElGroupoDbContext dbContext;
        private IEmailSender emailSender;
        public MessagesController(UserManager<User> userMgr,
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
                    PostedBy = user,
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
                var canEdit = HttpContext.User.IsInRole("admin") || this.dbContext.EventAttendees.Any(x => x.EventId == eventId && x.UserId == user.Id && x.IsOrganizer);
                if (attendee == null) return BadRequest();
                var model = new List<EventMessageModel>();
                foreach (var mba in this.dbContext.MessageBoardItemAttendees.Include("MessageBoardItem.User").Where(x => x.AttendeeId == attendee.Id))
                {
                    model.Add(new EventMessageModel
                    {
                        PostedBy = mba.MessageBoardItem.PostedBy.Name,
                        PostedById = mba.MessageBoardItem.PostedBy.Id,
                        CanEdit = mba.MessageBoardItem.PostedBy.Id == user.Id || canEdit,
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
        [Route("DeleteForAttendee/{messageId}")]
        public async Task<IActionResult> DeleteMessageForAttendee([FromRoute]long messageId)
        {
            var user = await this.userManager.GetUserAsync(HttpContext.User);
            var msg = await this.dbContext.MessageBoardItemAttendees.FirstOrDefaultAsync(x => x.Id == messageId);

            if (msg.Attendee.User.Id != user.Id && !HttpContext.User.IsInRole("admin"))
            {
                return BadRequest();
            }

            this.dbContext.Remove(msg);
            await this.dbContext.SaveChangesAsync();
            //var eo = dbContext.EventOrganizers.FirstOrDefault(x => x.Id == oid);
            //if (eo.EventId != eid) return BadRequest();
            //dbContext.EventOrganizers.Remove(eo);
            //await dbContext.SaveChangesAsync();
            //return RedirectToAction("OrganizersList", new { id = eid });
            return RedirectToRoute("GetMessagesByEventId", new { eid = msg.MessageBoardItem.EventId });
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
