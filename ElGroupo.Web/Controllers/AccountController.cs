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
using ElGroupo.Web.Mail.Models;
using ElGroupo.Web.Mail;

namespace ElGroupo.Web.Controllers
{
    [Authorize]
    [Route("Account")]
    public class AccountController : ControllerBase
    {
        //private UserManager<User> userManager;
        private IPasswordHasher<User> passwordHasher;
        //private ElGroupoDbContext dbContext;
        private IEmailService emailService;
        private GoogleConfigOptions googleOptions;
        private readonly UserManager<User> _userManager = null;
        private readonly UserService _userService;
        private readonly AccountService _accountService;
        public AccountController(UserManager<User> userMgr, IPasswordHasher<User> hasher, ElGroupoDbContext ctx, IEmailService sender, IOptions<GoogleConfigOptions> googConfig, UserService usr, AccountService acct) : base(userMgr)
        {
            //userManager = userMgr;
            passwordHasher = hasher;

            emailService = sender;
            googleOptions = googConfig.Value;
            _userService = usr;
            _accountService = acct;
            _userManager = userMgr;
        }


        [AllowAnonymous]
        [HttpGet("GoogleConfig")]
        public IActionResult GoogleConfig()
        {
            return Json(new { config = googleOptions });
        }

        [AllowAnonymous]
        [HttpGet("About")]
        public IActionResult About()
        {
            return View();
        }

        [Authorize]
        [HttpPost("AddConnection/{uid}")]
        public async Task<IActionResult> AddConnection([FromRoute]long uid)
        {

            var user = await CurrentUser();
            var response = await _accountService.AddUserConnection(user, uid);
            if (response.Success) return RedirectToAction("GetConnectionList", new { uid = user.Id });
            return BadRequest(new { message = response.ErrorMessage });
        }


        [Authorize]
        [HttpPost("RemoveRegisteredConnection/{uid}")]
        public async Task<IActionResult> RemoveRegisteredConnection([FromRoute]long uid)
        {
            var user = await CurrentUser();
            var response = await _accountService.RemoveRegisteredConnection(user, uid);
            if (response.Success) return RedirectToAction("GetConnectionList", new { uid = user.Id });
            return BadRequest(new { message = response.ErrorMessage });
        }




        [Authorize]
        [HttpPost("ImportSelectedContacts")]
        public async Task<IActionResult> ImportContacts([FromBody]ImportSelectContactModel[] contacts)
        {
            var user = await CurrentUser();
            var response = await _accountService.ImportContacts(user, contacts);
            if (response.Success) return RedirectToAction("GetConnectionList", new { uid = user.Id });
            return BadRequest(new { message = response.ErrorMessage });
        }

        [Authorize]
        [HttpPost("LoadImportFile")]
        public async Task<IActionResult> LoadImportFile()
        {
            if (Request.Form.Files.Count == 0) return BadRequest();
            if (!Request.Form.ContainsKey("format")) return BadRequest();
            var format = Request.Form["format"].ToString();
            var ms = new MemoryStream();
            await Request.Form.Files[0].CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            //nake configurable!!
            int idxFirstName = -1;
            int idxLastName = -1;
            int idxEmail = -1;
            int idxPhone1 = -1;
            int idxPhone2 = -1;
            if (format == "google")
            {
                idxFirstName = 1;
                idxLastName = 3;
                idxEmail = 28;
                idxPhone1 = 32;
                idxPhone2 = 34;
            }
            else
            {
                idxFirstName = 0;
                idxLastName = 2;
                idxEmail = 14;
                //outlook has multiple phone columns - these are home and mobile
                idxPhone1 = 18;
                idxPhone2 = 20;
            }

            var outList = new List<ImportSelectContactModel>();
            var reader = new StreamReader(ms);
            var line = reader.ReadLine();
            //skip first line
            line = reader.ReadLine();
            while (line != null)
            {
                var lineAry = line.Split(',');
                outList.Add(new ImportSelectContactModel
                {
                    FirstName = lineAry[idxFirstName],
                    LastName = lineAry[idxLastName],
                    Email = lineAry[idxEmail],
                    Phone1 = lineAry[idxPhone1],
                    Phone2 = lineAry[idxPhone2],
                    Registered = _userService.UserEmailExists(lineAry[idxEmail])
                });
                line = reader.ReadLine();
            }

            return Json(outList);
            //return View("_ImportContactList", outList);
        }



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
            AuthenticationProperties props = _userService.GoogleAuthenticationProps(redirectUrl);
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
                ExternalLoginInfo info = await _userService.GetExternalLoginInfo();
                if (info == null)
                {
                    return RedirectToAction(nameof(Login));
                }

                var loginSuccess = await _userService.ExternalLogin(info);
                if (loginSuccess)
                {
                    return Redirect(returnUrl == "/" ? "/Home/Dashboard" : returnUrl);
                }
                else
                {
                    var createExternalUserResponse = await _userService.CreateExternalLoginUser(info);
                    if (createExternalUserResponse.Success) return Redirect("/Account/Edit");
                    return BadRequest(new { message = createExternalUserResponse.ErrorMessage });
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
        public IActionResult Login(string returnUrl)
        {
            if (_userService.IsSignedIn(HttpContext.User)) return Redirect("/Home/Dashboard");
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
            var invite = await _accountService.GetUnregisteredAttendee(id);
            if (invite == null) return BadRequest();
            return View("Create", new CreateAccountModel { InvitedFromEvent = true, InviteName = invite.Name, EventName = invite.Event.Name, InvitedEmail = invite.Email, InviteId = invite.RegisterToken });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("VerifyEmail/{code}", Name = "VerifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromRoute]string code)
        {
            var codeGuid = new Guid(code);
            var verifyResponse = await _accountService.VerifyEmail(code);
            if (!verifyResponse.Success) return BadRequest(new { message = verifyResponse.ErrorMessage });
            ViewBag.Message = "Thanks for confirming your email!  Please login below!";
            return RedirectToAction("Login");

        }

        //[HttpGet("EditAttendeeGroup/{id}")]
        //public async Task<IActionResult> EditAttendeeGroup([FromRoute]long id)
        //{
        //    var model = new EditAttendeeGroupModel();
        //    if (id != 0)
        //    {
        //        var attGroup = await dbContext.AttendeeGroups.
        //            Include(x => x.Attendees).ThenInclude(x => x.User).
        //            FirstOrDefaultAsync(x => x.Id == id);
        //        model.Id = id;
        //        model.Name = attGroup.Name;
        //        foreach(var user in attGroup.Attendees.Select(x => x.User))
        //        {
        //            var uim = new UserInformationModel();
        //            uim.Name = user.Name;
        //            uim.Email = user.Email;
        //            uim.Id = user.Id;
        //            uim.UserName = user.UserName;

        //        }
        //    }



        //}


        [Authorize]
        [HttpGet("AttendeeGroupUserList"), HttpPost("AttendeeGroupUserList")]
        public IActionResult AttendeeGroupUserList([FromBody]AttendeeGroupUserModel[] models)
        {
            //we will have added the new one on the client side
            return View("_AttendeeGroupUserList", models);
        }



        [AllowAnonymous]
        [HttpGet]
        [Route("PendingEmailConfirmation")]
        public IActionResult PendingEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Create")]
        public async Task<IActionResult> Create([FromForm]CreateAccountModel model)
        {
            if (model.InviteId.HasValue)
            {
                if (!await _accountService.VerifyInviteToken(model.InviteId.Value)) return AccessDenied();
                model.EmailAddress = model.InvitedEmail;
            }

            if (ModelState.IsValid)
            {
                try
                {

                    User newUser = new User
                    {
                        Email = model.EmailAddress,
                        UserName = model.UserName,
                        Name = model.Name,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        ZipCode = model.ZipCode,
                        PhoneNumber = model.PhoneNumber,
                        EmailConfirmed = model.InviteId.HasValue
                    };

                    //need to attempt to add first in case we get any password validation errors or something -

                    var createResponse = await _userService.CreateUser(newUser, model.Password);
                    //var res = await userManager.CreateAsync(newUser, model.Password);

                    if (!createResponse.Success)
                    {
                        ModelState.AddModelError("", createResponse.ErrorMessage);
                        return View(model);
                    }

                    if (model.Photo != null)
                    {
                        var newUserId = Convert.ToInt64(createResponse.ResponseData);
                        newUser = await _userService.GetUserById(newUserId);

                        byte[] fileBytes = null;
                        using (var imageStream = model.Photo.OpenReadStream())
                        {
                            using (var ms = new MemoryStream())
                            {
                                await imageStream.CopyToAsync(ms);
                                fileBytes = ms.ToArray();
                            }
                        }
                        var photoResult = await _accountService.CreateUserPhoto(model.Photo.ContentType, fileBytes);
                        if (photoResult.Success)
                        {
                            newUser.Photo = (UserPhoto)photoResult.ResponseData;
                            var updateResponse = await _userService.UpdateUser(newUser);
                            if (!updateResponse.Success) return BadRequest(new { message = updateResponse.ErrorMessage });
                        }

                    }

                    //what about converting unregistereduserconnection to userconnection??
                    await _accountService.UpdateConnectionRecordsForNewUser(newUser);
                    await _accountService.UpdateAttendeeRecordsForNewUser(newUser);
                    if (model.InviteId.HasValue)
                    {
                        //no need to verify email


                        return Redirect("/Account/Edit");
                    }
                    else
                    {
                        var tokenResponse = await _accountService.CreateVerifyEmailToken(newUser);
                        if (!tokenResponse.Success) return BadRequest(new { message = tokenResponse.ErrorMessage });
                        var mailModel = new VerifyEmailModel
                        {
                            Recipient = newUser.Name,
                            CallbackUrl = Url.Action("VerifyEmail", "Account", new { code = tokenResponse.ResponseData.ToString() }, HttpContext.Request.Scheme)
                        };
                        var mailMetadata = new MailMetadata
                        {
                            To = new List<string> { newUser.Email },
                            Subject = "Welcome To Footprint!"
                        };
                        await this.emailService.SendEmail(mailMetadata, mailModel);
                        return Redirect("/Account/PendingEmailConfirmation");
                    }







                }
                catch (Exception ex)
                {

                }



            }
            return View(model);

        }


        [HttpGet("ViewAttendeeGroup/{id}")]
        public async Task<IActionResult> ViewAttendeeGroup([FromRoute]long id)
        {
            var model = await _accountService.GetAttendeeGroup(id);
            return View("_ViewAttendeeGroup", model);
        }


        [HttpDelete("DeleteAttendeeGroup/{id}")]
        public async Task<IActionResult> DeleteAttendeeGroup([FromRoute] long id)
        {
            var activeUser = await CurrentUser();
            var deleteResponse = await _accountService.DeleteAttendeeGroup(activeUser, id);
            if (deleteResponse.Success) return RedirectToAction("ViewAttendeeGroups", new { uid = activeUser.Id });
            return BadRequest(new { message = deleteResponse.ErrorMessage });
        }

        [HttpPost]
        [Authorize]
        [Route("EditAttendeeGroup")]
        public async Task<IActionResult> EditAttendeeGroup([FromBody]AttendeeGroupModel model)
        {
            var user = await CurrentUser();
            var updateResponse = await _accountService.UpdateAttendeeGroup(user, model);
            if (updateResponse.Success) return RedirectToAction("ViewAttendeeGroups", new { uid = model.UserId });
            return BadRequest(new { message = updateResponse.ErrorMessage });
        }


        [HttpGet("ViewAttendeeGroups/{uid}"), HttpPost("ViewAttendeeGroups/{uid}")]
        public async Task<IActionResult> ViewAttendeeGroups([FromRoute] int uid)
        {
            var user = await CurrentUser();
            return View("_AttendeeGroupList", await _accountService.GetUserAttendeeGroupList(user.Id));
        }









        [HttpGet]
        [Route("ConnectionList/{uid}")]
        public async Task<IActionResult> GetConnectionList([FromRoute] int uid)
        {
            var user = await CurrentUser();
            return View("_ConnectionList", await _accountService.GetUserConnections(user.Id));
        }

        [HttpGet]
        [Route("Departments")]
        public async Task<IActionResult> Departments()
        {
            var user = await CurrentUser();
            var model = _accountService.GetUserDepartments(user.Id);
            //model.clientId = this.googleOptions.GoogleClientId;
            //model.apiKey = this.googleOptions.GoogleApiKey;
            //model.ShowSaveConfirmation = showConfirm;
            return View(model);
        }

        [HttpPost]
        [Route("SaveUserDepartments")]
        public async Task<IActionResult> SaveUserDepartments([FromBody]EditUserDepartmentsModel[] model)
        {
            var user = await CurrentUser();
            var response = await _accountService.UpdateUserDepartments(user.Id, model);
            return Ok();
        }

        [HttpPost]
        [Route("AddOrganizationDepartment")]
        public async Task<IActionResult> AddOrganizationDepartment([FromBody]AddDepartmentModel model)
        {
            var user = await CurrentUser();
            var response = await _accountService.AddOrganizationDepartment(user.Id, model);
            if (response.Success)
            {
                var deptId = Convert.ToInt64(response.ResponseData);
                var newModel = new SelectDepartmentModel { Id = deptId, Name = model.DepartmentName, IsSelected = false, Groups = new List<SelectDepartmentUserGroupModel>() };
                return View("_OrganizationDepartment", newModel);
            }
            else
            {
                return BadRequest(new { Message = response.ErrorMessage });
            }
        }

        [HttpPost]
        [Route("DeleteDepartment")]
        public async Task<IActionResult> DeleteDepartment([FromBody]DeleteDepartmentModel model)
        {
            var user = await CurrentUser();
            var response = await _accountService.DeleteDepartment(model.DepartmentId);
            if (response.Success)
            {
                var deptId = Convert.ToInt64(response.ResponseData);
                return Ok(new { id = deptId });
            }
            else
            {
                return BadRequest(new { Message = response.ErrorMessage });
            }
        }

        [HttpPost]
        [Route("DeleteDepartmentGroup")]
        public async Task<IActionResult> DeleteDepartmentGroup([FromBody]DeleteDepartmentGroupModel model)
        {
            var user = await CurrentUser();
            var response = await _accountService.DeleteDepartmentGroup(model.GroupId);
            if (response.Success)
            {
                var deptId = Convert.ToInt64(response.ResponseData);
                return Ok(new { id = deptId });
            }
            else
            {
                return BadRequest(new { Message = response.ErrorMessage });
            }
        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(bool showConfirm = false)
        {
            var user = await CurrentUser();
            var model = await _accountService.GetAccountEditModel(user.Id);
            model.clientId = this.googleOptions.GoogleClientId;
            model.apiKey = this.googleOptions.GoogleApiKey;
            model.ShowSaveConfirmation = showConfirm;
            return View(model);
        }

        [HttpGet]
        [Route("View/{userId}")]
        public async Task<IActionResult> View(int userId)
        {
            var user = await CurrentUser();
            return View(await _accountService.GetAccountViewModel(userId));
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
        public async Task<IActionResult> EditAdmin([FromRoute]long id)
        {
            var model = await _accountService.GetAccountEditModel(id);
            model.IsAdminEditing = true;
            return View("Edit", model);
        }


        [HttpPost]
        [Route("Delete/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute]long id)
        {
            var response = await _userService.Delete(id);
            if (response.Success) return View("Admin", null);
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> Edit(EditAccountModel model)
        {
            User user = await CurrentUser();
            var isAdmin = await _userService.IsAdmin(user);
            var editingOwnAccount = true;
            if (isAdmin && user.Id != model.Id)
            {
                user = await _userService.GetUserById(model.Id);
                editingOwnAccount = false;
            }

            var updateResponse = await _accountService.UpdateUser(model);
            if (!updateResponse.Success) return BadRequest(new { message = updateResponse.ErrorMessage });

            if (isAdmin && !editingOwnAccount)
            {
                return RedirectToAction("EditAdmin", new { id = model.Id });
            }
            else
            {
                return RedirectToAction("Edit", new { showConfirm = true });
            }

        }

        [Authorize]
        [HttpGet, HttpDelete]
        [Route("Contacts")]
        public async Task<IActionResult> Contacts()
        {
            var user = await CurrentUser();
            return View("_ContactMethods", await _accountService.GetUserContactMethods(user.Id));
        }

        [HttpPost]
        [Route("Contacts/Create")]
        public async Task<IActionResult> CreateContact([FromBody]EditContactModel model)
        {
            var user = await CurrentUser();
            var createResponse = await _accountService.CreateContactMethod(model, user.Id);
            if (!createResponse.Success) return BadRequest(new { message = createResponse.ErrorMessage });
            return RedirectToAction("Contacts");
        }

        [HttpPost]
        [Route("Contacts/Update")]
        public async Task<IActionResult> UpdateContact([FromBody]EditContactModel model)
        {
            var updateResponse = await _accountService.UpdateUserContactMethod(model.Id, model.Value);
            if (!updateResponse.Success) return BadRequest(new { message = updateResponse.ErrorMessage });
            return RedirectToAction("Contact");
        }

        [Route("Contacts/Delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteContact([FromRoute]long id)
        {
            var deleteResponse = await _accountService.DeleteUserContactMethod(id);
            if (!deleteResponse.Success) return BadRequest(new { message = deleteResponse.ErrorMessage });
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
            var result = await _userService.ResetPassword(model.Email, model.Code, model.Password);
            if (result.Success) return RedirectToAction("Login");
            return BadRequest(new { message = result.ErrorMessage });
        }

        [Route("ForgotPassword"), HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            var tokenResponse = await _userService.CreateForgotPasswordToken(model.Email);
            if (!tokenResponse.Success) return BadRequest(new { message = tokenResponse.ErrorMessage });

            var callbackUrl = Url.Action("ResetPassword", "Account", new { code = tokenResponse.ResponseData.ToString() }, HttpContext.Request.Scheme);
            var user = await _userService.GetUserByEmail(model.Email);
            var mailModel = new ElGroupo.Web.Mail.Models.ForgotPasswordMailModel { CallbackUrl = callbackUrl, Recipient = user.Name };
            var metadata = new Mail.MailMetadata { To = new List<string> { user.Email }, Subject = "ElGroupo Password Reset" };
            await emailService.SendEmail(metadata, mailModel);
            return View("ForgotPasswordConfirmation");
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
            var loginResponse = await _userService.Login(details.Email, details.Password, details.RememberMe);
            if (!loginResponse.Success) return BadRequest(new { message = loginResponse.ErrorMessage });
            //deal with time zone shit
            var tzChanged = await TimeZoneChanged(details.UtcOffset, Convert.ToInt64(loginResponse.ResponseData));
            if (returnUrl != null) return Redirect(returnUrl);
            return RedirectToAction("UserDashboard", "Home", new { confirmTimeZone = tzChanged });
            //return Redirect(returnUrl ?? "/Home/Dashboard");

        }

        private async Task<bool> TimeZoneChanged(int offset, long userid)
        {
            //HttpContext.User is null here...
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null || user.TimeZoneId == null)
            {
                var realUser = _accountService.GetUser(userid);
                var tz = TimeZoneInfo.FindSystemTimeZoneById(realUser.TimeZoneId);
                var currentOffset = tz.GetUtcOffset(DateTime.Now);
                return Math.Abs(Convert.ToDouble(offset)) != Math.Abs(currentOffset.TotalMinutes);
            }
            else
            {
                return false;
            }

        }



        [Route("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _userService.Logout();
            return RedirectToAction("Index", "Home");
        }


    }
}
