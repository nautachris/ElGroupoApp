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
    public class UsersController:Controller
    {
        private ElGroupoDbContext dbContext;
        private IHostingEnvironment hostEnv;
        public UsersController(ElGroupoDbContext ctx, IHostingEnvironment env)
        {
            this.dbContext = ctx;
            this.hostEnv = env;
        }

        [Authorize]
        [HttpGet]
        [Route("SearchAutocomplete/{search}")]
        public async Task<IActionResult> SearchAutocomplete([FromRoute]string search)
        {
            var users = dbContext.Users.Where(x => x.Name.ToUpper().Contains(search.ToUpper()) || x.Email.ToUpper().Contains(search.ToUpper()));
            var list = new List<AutoCompleteModel>();
            await users.ForEachAsync(x => list.Add(new AutoCompleteModel { Email = x.Email, Id = x.Id, Name = x.Name }));
            return Json(list);
        }

        [Authorize]
        [HttpGet]
        [Route("Search/{search}")]
        public async Task<IActionResult> Search([FromRoute]string search)
        {
            var users = dbContext.Users.Where(x => x.Name.ToUpper().Contains(search.ToUpper()) || x.Email.ToUpper().Contains(search.ToUpper()));
            var list = new List<UserInformationModel>();
            await users.ForEachAsync(x => list.Add(new UserInformationModel { Email = x.Email, Id = x.Id, Name = x.Name, UserName = x.UserName }));
            return PartialView("../Account/_UserList", list.OrderBy(x=>x.UserName));
        }


        [Authorize]
        [HttpGet]
        [Route("UserPhoto/{id}")]
        public async Task<IActionResult> UserPhoto([FromRoute]int id)
        {
            var user = dbContext.Users.Include("Photo").FirstOrDefault(x => x.Id == id);
            if (user == null || user.Photo == null)
            {
                var ff = this.hostEnv.WebRootPath;
                string path = this.hostEnv.WebRootPath + "\\content\\resources\\images\\noimage.jpg";
                if (System.IO.File.Exists(path))
                {
                    var bytes = System.IO.File.ReadAllBytes(this.hostEnv.WebRootPath + "\\content\\resources\\images\\noimage.jpg");
                    return File(bytes, "image/jpg");
                }
                else
                {
                    return new EmptyResult();
                }
               
                //reader.wr

            }
            else
            {
                return File(user.Photo.ImageData, user.Photo.ContentType);
            }
            


        }
    }
}
