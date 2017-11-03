using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Domain;
using Microsoft.AspNetCore.Identity;
using ElGroupo.Web.Models.Account;
using Microsoft.EntityFrameworkCore;
using ElGroupo.Domain.Data;
using System.IO;
using ElGroupo.Web.Models.Users;
using Microsoft.AspNetCore.Hosting;

namespace ElGroupo.Web.Controllers
{



    [Route("Users")]
    public class UsersController : Controller
    {
        private ElGroupoDbContext dbContext;
        private IHostingEnvironment hostEnv;
        private UserManager<User> userManager = null;
        public UsersController(ElGroupoDbContext ctx, IHostingEnvironment env, UserManager<User> userMgr)
        {
            this.dbContext = ctx;
            this.hostEnv = env;
            this.userManager = userMgr;
        }

        [Authorize]
        [HttpGet]
        [Route("SearchAllUsers/{search}")]
        public async Task<IActionResult> SearchAllUsers([FromRoute]string search)
        {
            var users = dbContext.Users.Where(x => x.Name.ToUpper().Contains(search.ToUpper()) || x.Email.ToUpper().Contains(search.ToUpper()));
            var list = new List<AutoCompleteModel>();
            await users.ForEachAsync(x => list.Add(new AutoCompleteModel { Email = x.Email, Id = x.Id, Name = x.Name }));
            return Json(list);
        }


        [Authorize]
        [HttpGet]
        [Route("SearchUserConnections/{search}")]
        public async Task<IActionResult> SearchUserConnections([FromRoute]string search)
        {
            var user = await this.userManager.GetUserAsync(HttpContext.User);
            var users = dbContext.UserConnections.Include(x=>x.ConnectedUser).Where(x => x.User.Id == user.Id && x.ConnectedUser.Name.ToUpper().Contains(search.ToUpper()) || x.ConnectedUser.Email.ToUpper().Contains(search.ToUpper())).Select(x => x.ConnectedUser);
            var list = new List<ConnectionAutoCompleteModel>();
            await users.ForEachAsync(x => list.Add(new ConnectionAutoCompleteModel { Email = x.Email, Id = x.Id, Name = x.Name, Registered = true }));
            var unregisteredUsers = dbContext.UnregisteredUserConnections.Where(x => x.User.Id == user.Id && x.Name.ToUpper().Contains(search.ToUpper()) || x.Email.ToUpper().Contains(search.ToUpper()));
            await unregisteredUsers.ForEachAsync(x => list.Add(new ConnectionAutoCompleteModel { Email = x.Email, Name = x.Name, Id = x.Id, Registered = false }));

            var groups = dbContext.AttendeeGroups.Include(x => x.User).Include(x => x.Attendees).Where(x => x.User.Id == user.Id && x.Name.ToUpper().Contains(search.ToUpper()));
            await groups.ForEachAsync(x => list.Add(new ConnectionAutoCompleteModel { Group = true, GroupUserCount = x.Attendees.Count, Name = x.Name, Id = x.Id }));
            return Json(list.OrderBy(x => x.Name).ToList());
        }

        [Authorize]
        [HttpGet]
        [Route("Search/{search}")]
        public async Task<IActionResult> Search([FromRoute]string search)
        {
            var users = dbContext.Users.Where(x => x.Name.ToUpper().Contains(search.ToUpper()) || x.Email.ToUpper().Contains(search.ToUpper()));
            var list = new List<UserInformationModel>();
            await users.ForEachAsync(x => list.Add(new UserInformationModel { Email = x.Email, Id = x.Id, Name = x.Name, UserName = x.UserName }));
            return PartialView("../Account/_UserList", list.OrderBy(x => x.UserName));
        }

        private byte[] MissingUserPhoto()
        {

            var ff = this.hostEnv.WebRootPath;
            string path = this.hostEnv.WebRootPath + "\\content\\resources\\images\\noimage.jpg";
            if (System.IO.File.Exists(path))
            {
                var bytes = System.IO.File.ReadAllBytes(this.hostEnv.WebRootPath + "\\content\\resources\\images\\noimage.jpg");
                return bytes;
            }
            else
            {
                return null;
            }
        }


        [Authorize]
        [HttpGet]
        [Route("UserPhoto/{id}")]
        public async Task<IActionResult> UserPhoto([FromRoute]long id)
        {
            if (id != -1)
            {
                var user = dbContext.Users.Include("Photo").FirstOrDefault(x => x.Id == id);
                if (user == null || user.Photo == null)
                {
                    var missing = MissingUserPhoto();
                    if (missing != null) return File(missing, "image/jpg");
                    return new EmptyResult();
                }
                else
                {
                    return File(user.Photo.ImageData, user.Photo.ContentType);
                }
            }
            else
            {
                var missing = MissingUserPhoto();
                if (missing != null) return File(missing, "image/jpg");
                return new EmptyResult();
            }




        }
    }
}
