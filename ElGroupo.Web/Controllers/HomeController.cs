using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ElGroupo.Domain;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ElGroupo.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private UserManager<User> _userManager;

        public HomeController(UserManager<User> userManager)
        {
            this._userManager = userManager;
        }


         
        
        public async Task<IActionResult> Dashboard() => View();

        [AllowAnonymous]
        public async Task<IActionResult> Index() => RedirectToAction("Login", "Account");
        //public async Task<IActionResult> Index() => View(await this.CurrentUser);

        //[Authorize(Roles = "Users")]
        //this corresponds to the policy name defined in Startup.ConfigureServices.AddAuthorization
        //[Authorize(Policy = "DCUsers")]
        //public IActionResult OtherAction() => View("Index", GetData(nameof(OtherAction)));


        ////bob cannot access this b/c the requirement for the NotBob policy
        //[Authorize(Policy = "NotBob")]
        //public IActionResult NotBob() => View("Index", GetData(nameof(NotBob)));

        //private Dictionary<string, object> GetData(string actionName) => new Dictionary<string, object>
        //{
        //    ["Action"] = actionName,
        //    ["User"] = HttpContext.User.Identity.Name,
        //    ["Authenticated"] = HttpContext.User.Identity.IsAuthenticated,
        //    ["Auth Type"] = HttpContext.User.Identity.AuthenticationType,
        //    ["In Users Role"] = HttpContext.User.IsInRole("Users"),
        //    ["City"] = this.CurrentUser.Result.City,
        //    ["Qualification"] = this.CurrentUser.Result.Qualifications
        //};

        [Authorize]
        public async Task<IActionResult> UserProps()
        {
            return View(await this.CurrentUser);
        }

        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> UserProps([Required] Cities city, [Required] QualificationLevels qualifications)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        User user = await this.CurrentUser;
        //        user.City = city;
        //        user.Qualifications = qualifications;
        //        await _userManager.UpdateAsync(user);
        //        return RedirectToAction("Index");
        //    }
        //    return View(await this.CurrentUser);
        //}

        private Task<User> CurrentUser => _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

    }
}
