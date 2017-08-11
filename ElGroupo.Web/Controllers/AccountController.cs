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

namespace ElGroupo.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private IPasswordHasher<User> passwordHasher;
        private ElGroupoDbContext dbContext;

        public AccountController(UserManager<User> userMgr,
                SignInManager<User> signinMgr, IPasswordHasher<User> hasher, ElGroupoDbContext ctx)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            passwordHasher = hasher;
            dbContext = ctx;
        }

        //[AllowAnonymous]
        //public IActionResult GoogleLogin(string returnUrl)
        //{
        //    string redirectUrl = Url.Action("GoogleResponse", "Account", new { ReturnUrl = returnUrl });
        //    AuthenticationProperties props = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        //    return new ChallengeResult("Google", props);


        //}

        //[AllowAnonymous]
        //public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        //{
        //    ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {
        //        return RedirectToAction(nameof(Login));
        //    }

        //    var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
        //    if (result.Succeeded)
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        AppUser user = new AppUser { Email = info.Principal.FindFirst(ClaimTypes.Email).Value, UserName = info.Principal.FindFirst(ClaimTypes.Email).Value };
        //        IdentityResult idResult = await userManager.CreateAsync(user);
        //        if (idResult.Succeeded)
        //        {
        //            idResult = await userManager.AddLoginAsync(user, info);
        //            if (idResult.Succeeded)
        //            {
        //                await signInManager.SignInAsync(user, false);
        //                return Redirect(returnUrl);
        //            }
        //        }
        //        return AccessDenied();
        //    }
        //}


        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var signedIn = signInManager.IsSignedIn(HttpContext.User);
            if (signedIn) await signInManager.SignOutAsync();

            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromForm]CreateAccountModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User newUser = new User
                    {
                        Email = model.EmailAddress,
                        UserName = model.UserName,
                        Name = model.Name,
                        ZipCode = model.ZipCode,
                    };

                    //need to attempt to add first in case we get any password validation errors or something -
                    var res = await userManager.CreateAsync(newUser, model.Password);

                    if (!res.Succeeded)
                    {
                        foreach(var err in res.Errors)
                        {
                            ModelState.AddModelError("", err.Description);                            
                        }
                        return View(model);
                    }

                    if (model.Photo != null)
                    {
                        newUser = await userManager.FindByIdAsync(newUser.Id.ToString());
                        UserPhoto photo = new UserPhoto();
                        byte[] fileBytes = null;
                        using (var imageStream = model.Photo.OpenReadStream())
                        {
                            using (var ms = new MemoryStream())
                            {
                                await imageStream.CopyToAsync(ms);
                                fileBytes = ms.ToArray();
                            }
                        }
                        photo.ContentType = model.Photo.ContentType;
                        photo.ImageData = fileBytes;
                        dbContext.UserPhotos.Add(photo);
                        await dbContext.SaveChangesAsync();

                        newUser.Photo = photo;
                        await userManager.UpdateAsync(newUser);
                    }


                }
                catch(Exception ex)
                {

                }

                
                
            }
            return View(model);
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var userRecord = dbContext.Set<User>().Include("Photo").Include("Contacts").First(x => x.Id == user.Id);

            var model = new EditAccountModel();
            return new EmptyResult();
        }

        public async Task<IActionResult> EditContacts

        [Authorize(Roles ="admin")]
        public async Task<IActionResult> Users()
        {
            return new EmptyResult();
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel details,
                string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByEmailAsync(details.Email);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result =
                            await signInManager.PasswordSignInAsync(
                                user, details.Password, false, false);

                    //to permanently add claims to a user (via database)
                    //await userManager.AddClaimsAsync(user, new Claim[] { new Claim(ClaimTypes.PostalCode, "DC 20050", ClaimValueTypes.String, "RemoteClaims"), new Claim(ClaimTypes.StateOrProvince, "DC", ClaimValueTypes.String, "RemoteClaims") });




                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/Home/Dashboard");
                    }
                }
                ModelState.AddModelError(nameof(LoginModel.Email),
                    "Invalid user or password");
            }
            return View(details);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


    }
}
