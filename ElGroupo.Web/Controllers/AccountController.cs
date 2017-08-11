﻿using Microsoft.AspNetCore.Authorization;
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
    [Route("Account")]
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
        [HttpGet]
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
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("Create")]
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
                        foreach (var err in res.Errors)
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
                catch (Exception ex)
                {

                }



            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        [Route("UserPhoto/{id}")]
        public async Task<IActionResult> UserPhoto([FromRoute]int id)
        {
            var user = dbContext.Users.Include("Photo").FirstOrDefault(x => x.Id == id);
            if (user == null || user.Photo == null) return new EmptyResult();
            return File(user.Photo.ImageData, user.Photo.ContentType);
            

        }

        private List<EditContactModel> GetContacts(User user)
        {
            var model = new List<EditContactModel>();
            foreach (var c in user.Contacts)
            {
                model.Add(new EditContactModel
                {
                    Id = c.Id,
                    Value = c.Value,
                    ContactTypeId = c.ContactType.Id,
                    ContactTypeDescription = c.ContactType.Value
                });
            }
            return model;
        }
        private Dictionary<long, string> GetContactTypes()
        {
            var dict = new Dictionary<long, string>();
            foreach (var c in dbContext.ContactTypes)
            {
                dict.Add(c.Id, c.Value);
            }
            return dict;
        }


        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var userRecord = dbContext.Set<User>().Include("Photo").Include("Contacts.ContactType").First(x => x.Id == user.Id);

            var model = new EditAccountModel
            {
                Contacts = GetContacts(userRecord),
                ContactTypes = GetContactTypes(),
                EmailAddress = userRecord.Email,
                HasPhoto = userRecord.Photo != null,
                Id = userRecord.Id,
                Name = userRecord.Name,
                PhoneNumber = userRecord.PhoneNumber,
                ZipCode = userRecord.ZipCode
            };
            return View(model);
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> Edit(EditAccountModel model)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var userRecord = dbContext.Set<User>().Include("Photo").Include("Contacts.ContactType").First(x => x.Id == user.Id);
            userRecord.ZipCode = model.ZipCode;
            userRecord.Email = model.EmailAddress;
            userRecord.PhoneNumber = model.PhoneNumber;
            if (model.UpdatedPhoto != null)
            {
                //newUser = await userManager.FindByIdAsync(newUser.Id.ToString());
                //UserPhoto photo = new UserPhoto();
                byte[] fileBytes = null;
                using (var imageStream = model.UpdatedPhoto.OpenReadStream())
                {
                    using (var ms = new MemoryStream())
                    {
                        await imageStream.CopyToAsync(ms);
                        fileBytes = ms.ToArray();
                    }
                }
                if (userRecord.Photo == null)
                {
                    var newPhoto = new UserPhoto
                    {
                        ContentType = model.UpdatedPhoto.ContentType,
                        ImageData = fileBytes                        
                    };
                    dbContext.UserPhotos.Add(newPhoto);
                    userRecord.Photo = newPhoto;
                    
                }
                else
                {
                    userRecord.Photo.ContentType = model.UpdatedPhoto.ContentType;
                    userRecord.Photo.ImageData = fileBytes;
                }


            }

            dbContext.Users.Update(userRecord);
            await dbContext.SaveChangesAsync();
            //var model = new EditAccountModel
            //{
            //    Contacts = GetContacts(userRecord),
            //    ContactTypes = GetContactTypes(),
            //    EmailAddress = userRecord.Email,
            //    HasPhoto = userRecord.Photo != null,
            //    Id = userRecord.Id,
            //    Name = userRecord.Name,
            //    PhoneNumber = userRecord.PhoneNumber,
            //    ZipCode = userRecord.ZipCode
            //};
            return RedirectToAction("Edit");
        }

        [Authorize]
        [HttpGet]
        [Route("Contacts")]
        public async Task<IActionResult> Contacts()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var userRecord = dbContext.Set<User>().Include("Contacts.ContactType").First(x => x.Id == user.Id);


            return View("_Contacts", GetContacts(userRecord));
        }

        [HttpPost]
        [Route("Contacts/Create")]
        public async Task<IActionResult> CreateContact([FromBody]EditContactModel model)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var userRecord = dbContext.Set<User>().Include("Contacts.ContactType").First(x => x.Id == user.Id);
            var ct = dbContext.ContactTypes.First(x => x.Id == model.ContactTypeId);
            userRecord.AddContact(ct, model.Value);
            dbContext.Update(userRecord);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Contacts");
        }

        [HttpPost]
        [Route("Contacts/Update")]
        public async Task<IActionResult> UpdateContact([FromBody]EditContactModel model)
        {
            var c = dbContext.UserContacts.Include("User").First(x => x.Id == model.Id);
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (user.Id != c.User.Id)
            {
                return View("../Shared/AccessDenied");
            }
            c.Value = model.Value;
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Contact");
        }

        [Route("Contacts/Delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteContact([FromRoute]long id)
        {
            var c = dbContext.UserContacts.Include("User").First(x => x.Id == id);
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (user.Id != c.User.Id)
            {
                return View("../Shared/AccessDenied");
            }
            dbContext.UserContacts.Remove(c);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Contacts");
        }




        [Authorize(Roles = "admin")]
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
