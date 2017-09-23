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
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using ElGroupo.Web.Services;
using Microsoft.AspNetCore.Http.Authentication;
using System.Security.Claims;
using ElGroupo.Web.Models.Configuration;
using Microsoft.Extensions.Options;


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
        private IEmailSender emailSender;
        private GoogleConfigOptions googleOptions;
        public AccountController(UserManager<User> userMgr,
                SignInManager<User> signinMgr, IPasswordHasher<User> hasher, ElGroupoDbContext ctx, IEmailSender sender, IOptions<GoogleConfigOptions> googConfig)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            passwordHasher = hasher;
            dbContext = ctx;
            emailSender = sender;
            googleOptions = googConfig.Value;
        }


        //[Route("ImportGoogleContacts")]
        //public async Task<IActionResult> ImportGoogleContacts()
        //{

        //    UserCredential cred = await GoogleWebAuthorizationBroker.AuthorizeAsync(
        //        new ClientSecrets
        //    {
        //        ClientId = googleOptions.GoogleClientId,
        //        ClientSecret = googleOptions.GoogleClientSecret
        //    }, 
        //    new string[1] { "https://www.googleapis.com/auth/contacts.readonly" },
        //    "user",
        //    System.Threading.CancellationToken.None);


        //    var oAuthParams = new OAuth2Parameters { AccessToken = cred.Token.AccessToken, RefreshToken = cred.Token.RefreshToken };
        //    var reqSettings = new RequestSettings("Tribez", oAuthParams);
        //    ContactsRequest cc = new ContactsRequest(reqSettings);
        //    cc.Get<>

        //    return BadRequest();
        //}

        [HttpGet("ImportGoogleContacts")]
        public IActionResult ImportGoogleContacts()
        {
            return View(new GoogleCredModel { ApiKey = googleOptions.GoogleApiKey, ClientId = googleOptions.GoogleClientId });
        }


        [AllowAnonymous]
        [Route("/GoogleLogin", Name = "GoogleLogin")]
        public IActionResult GoogleLogin(string returnUrl)
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account", new { ReturnUrl = returnUrl });
            AuthenticationProperties props = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", props);


        }

        [Authorize]
        [HttpGet("GoogleCreds")]
        public IActionResult GetGoogleCreds()
        {
            return Json(new { secret = googleOptions.GoogleClientSecret, id = googleOptions.GoogleClientId });


        }

        [HttpGet]
        [Route("GoogleResponse", Name = "GoogleResponse")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/", string remoteError = null)
        {
            try
            {
                ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return RedirectToAction(nameof(Login));
                }

                var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
                if (result.Succeeded)
                {
                    return Redirect(returnUrl == "/" ? "/Home/Dashboard" : returnUrl);
                }
                else
                {
                    User user = new User
                    {
                        Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                        UserName = info.Principal.FindFirst(ClaimTypes.Email).Value
                    };
                    if (info.Principal.HasClaim(x => x.Type == ClaimTypes.GivenName) && info.Principal.HasClaim(x => x.Type == ClaimTypes.Surname))
                    {
                        user.Name = info.Principal.FindFirst(ClaimTypes.GivenName).Value + " " + info.Principal.FindFirst(ClaimTypes.Surname).Value;
                    }

                    IdentityResult idResult = await userManager.CreateAsync(user);
                    if (idResult.Succeeded)
                    {
                        idResult = await userManager.AddLoginAsync(user, info);
                        if (idResult.Succeeded)
                        {
                            await signInManager.SignInAsync(user, false);
                            return Redirect("/Account/Edit");
                        }
                    }
                    return AccessDenied();
                }
            }
            catch (Exception ex)
            {
                return AccessDenied();
            }

        }

        [Authorize(Roles = ("admin"))]
        [HttpGet("Admin")]
        public async Task<IActionResult> Admin()
        {
            //var modelList = new List<UserInformationModel>();
            //foreach(var u in dbContext.Users)
            //{
            //    modelList.Add(new UserInformationModel
            //    {
            //        Name = u.Name,
            //        Email = u.Email,
            //        UserName = u.UserName,
            //        Id = u.Id
            //    });
            //}
            return View(null);
        }

        [Route("Login")]
        [Route("/")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var signedIn = signInManager.IsSignedIn(HttpContext.User);
            if (signedIn) await signInManager.SignOutAsync();

            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [Route("AccessDenied")]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("Create")]
        public IActionResult Create()
        {
            return View(new CreateAccountModel());
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Create/{id}")]
        public async Task<IActionResult> CreateFromInvite(Guid id)
        {
            var invite = dbContext.UnregisteredEventAttendees.Include("Event").FirstOrDefault(x => x.RegisterToken == id);
            if (invite == null) return BadRequest();
            return View("Create", new CreateAccountModel { InvitedFromEvent = true, InviteName = invite.Name, EventName = invite.Event.Name, InvitedEmail = invite.Email, InviteId = invite.RegisterToken });
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("Create")]
        public async Task<IActionResult> Create([FromForm]CreateAccountModel model)
        {
            if (model.InviteId.HasValue) model.EmailAddress = model.InvitedEmail;
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
                        PhoneNumber = model.PhoneNumber
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


                    if (model.InviteId.HasValue)
                    {
                        //add new user to this eevent
                        //remove EventUregisteredAttendee record
                        var uea = dbContext.UnregisteredEventAttendees.Include("Event").FirstOrDefault(x => x.RegisterToken == model.InviteId);
                        var ea = new EventAttendee
                        {
                            Event = uea.Event,
                            User = newUser,
                            Viewed = false,
                            ResponseStatus = Domain.Enums.RSVPTypes.None
                        };

                        dbContext.UnregisteredEventAttendees.Remove(uea);
                        dbContext.EventAttendees.Add(ea);
                        await dbContext.SaveChangesAsync();


                    }

                }
                catch (Exception ex)
                {

                }



            }
            return Redirect("/Account/Edit");

        }

        private List<UserConnectionModel> GetConnections(User user)
        {
            var model = new List<UserConnectionModel>();
            foreach(var c in user.ConnectedUsers)
            {
                model.Add(new UserConnectionModel
                {
                    Name = c.ConnectedUser.Name,
                    Email = c.ConnectedUser.Email,
                    Phone = c.ConnectedUser.ContactMethods.Any(x=>x.ContactMethod.Value == "Phone")? c.ConnectedUser.ContactMethods.First(x=>x.ContactMethod.Value == "Phone").Value : "",
                    Status = "Registered"
                });
            }
            foreach(var uc in user.UnregisteredConnections)
            {
                model.Add(new UserConnectionModel {
                    Name = uc.Name,
                    Email = uc.Email,
                    Phone = "",
                    Status = "Not Registered"
                });
            }

            return model;
        }


        private List<EditContactModel> GetContacts(User user)
        {
            var model = new List<EditContactModel>();
            foreach (var c in user.ContactMethods)
            {
                model.Add(new EditContactModel
                {
                    Id = c.Id,
                    Value = c.Value,
                    ContactTypeId = c.ContactMethod.Id,
                    ContactTypeDescription = c.ContactMethod.Value
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
            var userRecord = dbContext.Set<User>().Include("Photo").Include("ContactMethods.ContactMethod").Include("ConnectedUsers.ConnectedUser").Include("UnregisteredConnections").First(x => x.Id == user.Id);

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

        [HttpGet]
        [Route("View/{userId}")]
        public async Task<IActionResult> View(int userId)
        {

            var userRecord = await dbContext.Set<User>().Include("Photo").Include("Contacts.ContactType").FirstOrDefaultAsync(x => x.Id == userId);

            var model = new ViewAccountModel
            {
                Contacts = GetContacts(userRecord),
                ContactTypes = GetContactTypes(),
                EmailAddress = userRecord.Email,
                Id = userRecord.Id,
                Name = userRecord.Name,
                PhoneNumber = userRecord.PhoneNumber,
                ZipCode = userRecord.ZipCode
            };
            return View(model);
        }

        public bool ThumbnailCallback()
        {
            return false;
        }
        private byte[] CreateThumbnail(Stream s, string contentType)
        {
            //all images will now be 300 px wide
            int width = 300;
            Image i = new Bitmap(s);
            double dx = 300d / (double)i.Width;

            Image.GetThumbnailImageAbort callback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            Image thumbnail = i.GetThumbnailImage(width, Convert.ToInt32(i.Height * dx), callback, IntPtr.Zero);
            var ms = new MemoryStream();
            ImageFormat format = ImageFormat.Jpeg;
            switch (contentType)
            {
                case "image/jpeg":
                case "image/jpg":
                    format = ImageFormat.Jpeg;
                    break;
                case "image/png":
                    format = ImageFormat.Png;
                    break;
                case "image/gif":
                    format = ImageFormat.Gif;
                    break;
            }
            thumbnail.Save(ms, format);
            return ms.ToArray();
        }



        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("Edit/{id}", Name = "editadmin")]
        public async Task<IActionResult> EditAdmin([FromRoute]int id)
        {

            var userRecord = dbContext.Set<User>().Include("Photo").Include("Contacts.ContactType").First(x => x.Id == id);

            var model = new EditAccountModel
            {
                Contacts = GetContacts(userRecord),
                ContactTypes = GetContactTypes(),
                EmailAddress = userRecord.Email,
                HasPhoto = userRecord.Photo != null,
                Id = userRecord.Id,
                Name = userRecord.Name,
                PhoneNumber = userRecord.PhoneNumber,
                ZipCode = userRecord.ZipCode,
                IsAdminEditing = true
            };
            return View("Edit", model);
        }


        [HttpPost]
        [Route("Delete/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            await userManager.DeleteAsync(user);
            return View("Admin", null);
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> Edit(EditAccountModel model)
        {
            User user = await userManager.GetUserAsync(HttpContext.User);
            var isAdmin = await userManager.IsInRoleAsync(user, "admin");
            var editingOwnAccount = true;
            if (isAdmin && user.Id != model.Id)
            {
                user = await userManager.FindByIdAsync(model.Id.ToString());
                editingOwnAccount = false;
            }

            var userRecord = dbContext.Set<User>().Include("Photo").Include("Contacts.ContactType").First(x => x.Id == user.Id);
            userRecord.ZipCode = model.ZipCode;
            userRecord.Email = model.EmailAddress;
            userRecord.PhoneNumber = model.PhoneNumber;
            if (model.UpdatedPhoto != null)
            {
                byte[] fileBytes = CreateThumbnail(model.UpdatedPhoto.OpenReadStream(), model.UpdatedPhoto.ContentType);
                //byte[] fileBytes = null;
                //using (var imageStream = model.UpdatedPhoto.OpenReadStream())
                //{
                //    using (var ms = new MemoryStream())
                //    {
                //        await imageStream.CopyToAsync(ms);
                //        fileBytes = ms.ToArray();
                //    }
                //}
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

            if (isAdmin && !editingOwnAccount)
            {
                return RedirectToAction("EditAdmin", new { id = model.Id });
            }
            else
            {
                return RedirectToAction("Edit");
            }

        }

        [Authorize]
        [HttpGet, HttpDelete]
        [Route("Contacts")]
        public async Task<IActionResult> Contacts()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var userRecord = dbContext.Set<User>().Include("Contacts.ContactType").First(x => x.Id == user.Id);


            return View("_EditContacts", GetContacts(userRecord));
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

        [HttpGet("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [Route("ResetPasswordConfirmation"), HttpGet, AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [Route("ResetPassword"), HttpGet, AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View(new ResetPasswordModel { Code = code });
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                if (error.Code.Contains("Password"))
                {
                    ModelState.AddModelError("Password", error.Description);
                }
                else if (error.Code.Contains("UserName"))
                {
                    ModelState.AddModelError("UserName", error.Description);
                }
                else if (error.Code.Contains("Email"))
                {
                    ModelState.AddModelError("Email", error.Description);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

        }
        [Route("ResetPassword"), HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
            try
            {
                var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    //TempData["EmailConfirmMessage"] = "The password for " + user.Email + " has been successfully changed.  Please login below";
                    return RedirectToAction("Login");
                }
                AddErrors(result);
                return View();
            }
            catch (Exception ex)
            {

            }
            return View();


        }

        [Route("ForgotPassword"), HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await this.userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return View("ForgotPasswordConfirmation");

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var resetCode = await this.userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { code = resetCode },
                    HttpContext.Request.Scheme);
                var msg = "Dear " + user.Name + ",";
                msg += "<br/>";
                msg = "<a href='" + callbackUrl + "'>Click on this link to reset your ElGroupo password!</a>";
                msg += "<br/>";
                msg += "Thanks,";
                msg += "<br/>";
                msg += "The ElGroupo Team";

                await emailSender.SendEmailAsync(model.Email, "ElGroupo Password Reset", msg);

                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet("ForgotUsername")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotUsername()
        {
            return View();
        }

        [HttpPost("Login")]
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
                                user, details.Password, details.RememberMe, false);

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
        [Route("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


    }
}
