using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ElGroupo.Domain;
using ElGroupo.Web.Models.Shared;
namespace ElGroupo.Web.Components
{
    public class HeaderComponent: ViewComponent
    {
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;

        public HeaderComponent(SignInManager<User> sim, UserManager<User> um)
        {
            this._signInManager = sim;
            this._userManager = um;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            var model = new HeaderViewModel();
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null) model.IsSignedIn = false;
                else
                {
                    model.IsSignedIn = true;
                    model.ActiveUser = user;
                }
            }                     
            return View(model);
        }

    }
}
