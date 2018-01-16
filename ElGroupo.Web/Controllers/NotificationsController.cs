using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Domain;
using Microsoft.AspNetCore.Identity;
using ElGroupo.Web.Models.Notifications;
using Microsoft.EntityFrameworkCore;
using ElGroupo.Domain.Data;
using ElGroupo.Web.Services;
using Amazon.SimpleEmail;
using System.IO;

namespace ElGroupo.Web.Controllers
{
    [Authorize]
    [Route("Notifications")]
    public class NotificationsController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private ElGroupoDbContext dbContext;
        //private IEmailSender emailSender;
                private IAmazonSimpleEmailService emailSender;
        public NotificationsController(UserManager<User> userMgr,
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
        public async Task<IActionResult> Create([FromBody]CreateNotificationModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this.userManager.GetUserAsync(HttpContext.User);
                var e = await this.dbContext.Events.Include(x=>x.Attendees).ThenInclude(x=>x.User).FirstAsync(x => x.Id == model.EventId);
                var org = e.Attendees.FirstOrDefault(x => x.User.Id == user.Id && x.IsOrganizer);
                if (!HttpContext.User.IsInRole("admin") && org == null)
                {
                    return BadRequest();
                }



                var msg = new EventNotification
                {
                    PostedBy = org,
                    Event = e,
                    PostedDate = DateTime.Now.ToUniversalTime(),
                    MessageText = model.Text,
                    Subject = model.Subject,
                    Importance = Domain.Enums.NotificationImportanceTypes.Critical,
                };



                dbContext.EventNotifications.Add(msg);
                foreach (var att in e.Attendees)
                {
                    var ean = new EventAttendeeNotification
                    {
                        Notification = msg,
                        Attendee = att
                    };
                    dbContext.EventAttendeeNotifications.Add(ean);
                }

                //foreach (var attendee in e.Attendees)
                //{
                //    dbContext.MessageBoardItemAttendees.Add(new MessageBoardItemAttendee
                //    {
                //        Attendee = attendee,
                //        Viewed = attendee.User.Id == user.Id,
                //        MessageBoardItem = msg
                //    });
                //}


                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var fff = 4;
                }

                //move on to Contacts

                return RedirectToAction("NotificationList", new { eventId = e.Id });
            }
            return View(model);

        }




        [HttpGet, HttpPost]
        [Route("Events/{eventId}", Name = "GetNotificationsByEventId")]
        public async Task<IActionResult> NotificationList([FromRoute]long eventId)
        {
            try
            {
                var user = await this.userManager.GetUserAsync(HttpContext.User);
                var attendee = await this.dbContext.EventAttendees.FirstOrDefaultAsync(x => x.EventId == eventId && x.User.Id == user.Id);
                var canEdit = HttpContext.User.IsInRole("admin") || this.dbContext.EventAttendees.Any(x => x.EventId == eventId && x.User.Id == user.Id && x.IsOrganizer);
                if (attendee == null) return BadRequest();
                var model = new List<EventNotificationModel>();
                foreach (var ean in this.dbContext.EventAttendeeNotifications.Include("Notification.PostedBy.User").Where(x => x.AttendeeId == attendee.Id))
                {
                    model.Add(new EventNotificationModel
                    {
                        OrganizerName = ean.Notification.PostedBy.User.Name,
                        OrganizerId = ean.Notification.PostedBy.User.Id,
                        PostedDate = ean.Notification.PostedDate,
                        Subject = ean.Notification.Subject,
                        NotificationText = ean.Notification.MessageText,
                        IsNew = !ean.Viewed,
                        Id = ean.Id
                    });
                }
                return View("../Events/_ViewEventNotifications", model.OrderByDescending(x => x.PostedDate));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }






        [HttpGet]
        [Route("SetAsViewed/{notificationId}")]
        public async Task<IActionResult> SetNotificationAsViewed(long notificationId)
        {
            //var user = await this.userManager.GetUserAsync(HttpContext.User);
            var msg = await this.dbContext.EventAttendeeNotifications.FirstOrDefaultAsync(x => x.Id == notificationId);
            if (msg == null) return BadRequest();
            msg.Viewed = true;
            this.dbContext.Update(msg);
            await this.dbContext.SaveChangesAsync();
            return Json(new { message = "success" });
        }



        [Authorize]
        [HttpDelete]
        [Route("DeleteForAttendee/{notificationId}")]
        public async Task<IActionResult> DeleteNotificationForAttendee([FromRoute]long notificationId)
        {
            var user = await this.userManager.GetUserAsync(HttpContext.User);
            var msg = await this.dbContext.EventAttendeeNotifications.Include("Notification.Event").FirstOrDefaultAsync(x => x.Id == notificationId);

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
            return RedirectToRoute("GetMessagesByEventId", new { eid = msg.Notification.Event.Id });
        }



        [Authorize]
        [HttpDelete]
        [Route("Delete/{notificationId}")]
        public async Task<IActionResult> DeleteNotification([FromRoute]long notificationId)
        {
            //this must be from an event organizer
            var user = await this.userManager.GetUserAsync(HttpContext.User);
            var msg = await this.dbContext.EventAttendeeNotifications.Include("Notification.Event.Organizers").Include("Notification.Attendees").FirstOrDefaultAsync(x => x.Id == notificationId);

            if (!msg.Notification.Event.Attendees.Any(x => x.User.Id == user.Id && x.IsOrganizer) || HttpContext.User.IsInRole("admin"))
            {
                return BadRequest();
            }

            //foreach (var item in msg.Notification.Attendees)
            //{
            //    this.dbContext.Remove(item);
            //}
            this.dbContext.Remove(msg.Notification);
            await this.dbContext.SaveChangesAsync();

            return RedirectToRoute("GetNotificationsByEventId", new { eid = msg.Notification.Event.Id });
        }


    }
}
