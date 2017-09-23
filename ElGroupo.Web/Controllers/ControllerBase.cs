using System;
using System.Collections.Generic;
using System.Linq;
using ElGroupo.Domain;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace ElGroupo.Web.Controllers
{
    public class ControllerBase:Controller
    {
        protected readonly UserManager<User> userManager = null;
        public ControllerBase(UserManager<User> manager)
        {
            this.userManager = manager;
        }
        private User _user = null;
        protected async Task<User> CurrentUser()
        {
            if (_user == null) _user = await this.userManager.GetUserAsync(HttpContext.User);
            return _user;
        }
    }
}
