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
using Amazon.SimpleEmail;
using ElGroupo.Web.Classes;

namespace ElGroupo.Web.Controllers
{
    [Authorize]
    [Route("Messages")]
    public class MessagesController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private ElGroupoDbContext dbContext;
        //private IEmailSender emailSender;
        private IAmazonSimpleEmailService emailSender;
        public MessagesController(UserManager<User> userMgr,
                SignInManager<User> signinMgr, ElGroupoDbContext ctx, IAmazonSimpleEmailService sndr)
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
                var e = await this.dbContext.Events.Include(x => x.Attendees).ThenInclude(x => x.User).FirstAsync(x => x.Id == model.EventId);

                MessageBoardTopic topic = null;
                if (model.ThreadId == -1)
                {
                    topic = new MessageBoardTopic();
                    topic.Event = e;
                    topic.Subject = model.Subject;
                    topic.StartedDate = DateTime.Now.ToUniversalTime();
                    topic.StartedBy = user;
                    this.dbContext.MessageBoardTopics.Add(topic);
                }
                else
                {
                    topic = this.dbContext.MessageBoardTopics.First(x => x.Id == model.ThreadId);
                }
                var msg = new MessageBoardItem
                {
                    PostedBy = user,
                    Topic = topic,
                    PostedDate = DateTime.Now.ToUniversalTime(),
                    MessageText = model.Text
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
                var attendee = await this.dbContext.EventAttendees.FirstOrDefaultAsync(x => x.EventId == eventId && x.User.Id == user.Id);
                var canEdit = HttpContext.User.IsInRole("admin") || this.dbContext.EventAttendees.Any(x => x.EventId == eventId && x.User.Id == user.Id && x.IsOrganizer);
                if (attendee == null) return BadRequest();
                var model = new EventMessageContainerModel();
                foreach (var topic in dbContext.MessageBoardTopics.Include(x=>x.StartedBy).Include(x => x.Messages).ThenInclude(x => x.Attendees).ThenInclude(x => x.Attendee).Where(x => x.Event.Id == eventId).OrderByDescending(x => x.StartedDate))
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
                            TopicName = topic.Subject,
                            MessageText = msg.MessageText,
                            CanEdit = msg.PostedBy.Id == user.Id,
                            PostedBy = msg.PostedBy.Name,
                            PostedById = msg.PostedBy.Id,
                            Id = msg.Attendees.First(x=>x.Attendee.Id == attendee.Id).Id,
                            DateText = localPostedDate.DayOfWeek.ToString() + " " + localPostedDate.ToString("d") + " " + localPostedDate.ToString("t"),
                            PostedDate = localPostedDate,
                            IsNew = msg.Attendees.Any(x => x.Attendee.Id == attendee.Id && !x.Viewed)
                        });
                    }
                    model.Topics.Add(topicModel);
                }
                return View("../Events/_ViewEventMessages", model);
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
            var msg = await this.dbContext.MessageBoardItemAttendees.Include(x => x.MessageBoardItem).ThenInclude(x => x.Topic).FirstOrDefaultAsync(x => x.Id == messageId);

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
            return RedirectToRoute("GetMessagesByEventId", new { eid = msg.MessageBoardItem.Topic.Event.Id });
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
            return RedirectToRoute("GetMessagesByEventId", new { eid = msg.MessageBoardItem.Topic.Event.Id });
        }


    }
}
