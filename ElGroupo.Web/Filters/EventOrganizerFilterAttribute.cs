using System;
using System.Collections.Generic;
using System.Linq;
using ElGroupo.Domain.Data;
using System.Threading.Tasks;
using ElGroupo.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace ElGroupo.Web.Filters
{
    public class EventOrganizerFilterAttribute: ActionFilterAttribute
    {
        private readonly ElGroupoDbContext context = null;
        private UserManager<User> userManager = null;
        public EventOrganizerFilterAttribute(ElGroupoDbContext ctx, UserManager<User> mgr)
        {
            this.context = ctx;
            this.userManager = mgr;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var f = context.HttpContext.RequestServices.GetService<UserManager<User>>();
            bool allowed = false;
            if (!context.HttpContext.User.IsInRole("admin"))
            {
                var uid = this.userManager.GetUserId(context.HttpContext.User);
                var user = await this.userManager.GetUserAsync(context.HttpContext.User);
                //var userId = Convert.ToInt32(this.userManager.GetUserId(context.HttpContext.User));
                var eventId = Convert.ToInt64(context.ActionArguments["eid"]);
                allowed = this.context.EventAttendees.Any(x => x.User.Id == user.Id && x.EventId == eventId && x.IsOrganizer);
                if (!allowed)
                {     
                    context.Result = new JsonResult("unauthorized");
                }
            }
            else
            {
                allowed = true;
            }
            
            if (allowed) await next();
        }
    }
}
