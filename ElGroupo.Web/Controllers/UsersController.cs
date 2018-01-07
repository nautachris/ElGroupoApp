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
using ElGroupo.Web.Services;

namespace ElGroupo.Web.Controllers
{



    [Route("Users")]
    public class UsersController : ControllerBase
    {

        private IHostingEnvironment hostEnv;
        private readonly UserService _userService = null;
        public UsersController(UserService userService, IHostingEnvironment env, UserManager<User> userMgr):base(userMgr)
        {
            _userService = userService;
            this.hostEnv = env;
            
        }

        [Authorize]
        [HttpGet]
        [Route("SearchAllUsers/{search}")]
        public async Task<IActionResult> SearchAllUsers([FromRoute]string search)
        {
            var list = await _userService.SearchAllUsers(search);
            return Json(list);
        }


        [Authorize]
        [HttpGet]
        [Route("SearchUserConnections/{search}")]
        public async Task<IActionResult> SearchUserConnections([FromRoute]string search)
        {
            var user = await CurrentUser();
            var list = await _userService.SearchUserConnections(user.Id, search);
            return Json(list.OrderBy(x => x.Name).ToList());
        }

        //[Authorize]
        //[HttpGet]
        //[Route("Search/{search}")]
        //public async Task<IActionResult> Search([FromRoute]string search)
        //{
        //    var users = dbContext.Users.Where(x => x.Name.ToUpper().Contains(search.ToUpper()) || x.Email.ToUpper().Contains(search.ToUpper()));
        //    var list = new List<UserInformationModel>();
        //    await users.ForEachAsync(x => list.Add(new UserInformationModel { Email = x.Email, Id = x.Id, Name = x.Name, UserName = x.UserName }));
        //    return PartialView("../Account/_UserList", list.OrderBy(x => x.UserName));
        //}

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
                var photo = await _userService.GetUserPhotoById(id);
                if (photo == null)
                {
                    var missing = MissingUserPhoto();
                    if (missing != null) return File(missing, "image/jpg");
                    return new EmptyResult();
                }
                else
                {
                    return File(photo.ImageData, photo.ContentType);
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
