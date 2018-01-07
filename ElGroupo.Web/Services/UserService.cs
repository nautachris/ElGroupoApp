using ElGroupo.Domain;
using ElGroupo.Web.Models.Shared;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Authentication;
using System.Security.Claims;
using ElGroupo.Web.Models.Users;

namespace ElGroupo.Web.Services
{

    public class UserService : BaseService
    {
        private readonly UserManager<User> _userManager = null;
        private readonly IEmailService _emailService = null;
        private readonly SignInManager<User> _signInManager;
        public UserService(UserManager<User> manager, SignInManager<User> signIn, ElGroupoDbContext ctx, IEmailService eml) : base(ctx)
        {
            this._userManager = manager;
            this._emailService = eml;
            this._signInManager = signIn;
        }

        public async Task<List<AutoCompleteModel>> SearchAllUsers(string search)
        {
            var users = _dbContext.Users.Where(x => x.Name.ToUpper().Contains(search.ToUpper()) || x.Email.ToUpper().Contains(search.ToUpper()));
            var list = new List<AutoCompleteModel>();
            await users.ForEachAsync(x => list.Add(new AutoCompleteModel { Email = x.Email, Id = x.Id, Name = x.Name }));
            return list;
        }

        public async Task<List<ConnectionAutoCompleteModel>> SearchUserConnections(long userId, string search)
        {

            var users = _dbContext.UserConnections.Include(x => x.ConnectedUser).Where(x => x.User.Id == userId && x.ConnectedUser.Name.ToUpper().Contains(search.ToUpper()) || x.ConnectedUser.Email.ToUpper().Contains(search.ToUpper())).Select(x => x.ConnectedUser);
            var list = new List<ConnectionAutoCompleteModel>();
            await users.ForEachAsync(x => list.Add(new ConnectionAutoCompleteModel { Email = x.Email, Id = x.Id, Name = x.Name, Registered = true }));
            var unregisteredUsers = _dbContext.UnregisteredUserConnections.Where(x => x.User.Id == userId && x.Name.ToUpper().Contains(search.ToUpper()) || x.Email.ToUpper().Contains(search.ToUpper()));
            await unregisteredUsers.ForEachAsync(x => list.Add(new ConnectionAutoCompleteModel { Email = x.Email, Name = x.Name, Id = x.Id, Registered = false }));

            var groups = _dbContext.AttendeeGroups.Include(x => x.User).Include(x => x.Attendees).Where(x => x.User.Id == userId && x.Name.ToUpper().Contains(search.ToUpper()));
            await groups.ForEachAsync(x => list.Add(new ConnectionAutoCompleteModel { Group = true, GroupUserCount = x.Attendees.Count, Name = x.Name, Id = x.Id }));
            return list;

        }



        public bool IsSignedIn(ClaimsPrincipal principal)
        {
            return _signInManager.IsSignedIn(principal);
        }

        public bool UserEmailExists(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return _dbContext.Users.Any(x => x.Email.ToUpper() == email.ToUpper());
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfo()
        {
            return await _signInManager.GetExternalLoginInfoAsync();
        }
        public async Task<bool> ExternalLogin(ExternalLoginInfo info)
        {
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            return result.Succeeded;
        }

        public async Task<SaveDataResponse> CreateExternalLoginUser(ExternalLoginInfo info)
        {
            try
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

                IdentityResult idResult = await _userManager.CreateAsync(user);
                if (idResult.Succeeded)
                {
                    idResult = await _userManager.AddLoginAsync(user, info);
                    if (idResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return SaveDataResponse.Ok();
                    }
                    else
                    {
                        return SaveDataResponse.FromErrorMessage("Error Signing In User" + Environment.NewLine + string.Join(Environment.NewLine, idResult.Errors));
                    }
                }
                else
                {
                    return SaveDataResponse.FromErrorMessage("Error Creating User" + Environment.NewLine + string.Join(Environment.NewLine, idResult.Errors));
                }

            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }

        }
        public AuthenticationProperties GoogleAuthenticationProps(string returnUrl)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties("Google", returnUrl);

        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();

        }

        public async Task<SaveDataResponse> Login(string email, string password, bool persistent)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return SaveDataResponse.FromErrorMessage("Email address not found");

            await _signInManager.SignOutAsync();
            var result = await _signInManager.PasswordSignInAsync(user, password, persistent, false);

            if (result.Succeeded) return SaveDataResponse.IncludeData(user.Id);
            if (result.IsLockedOut) return SaveDataResponse.FromErrorMessage("User is locked out");
            if (result.IsNotAllowed) return SaveDataResponse.FromErrorMessage("User is not allowed");
            return SaveDataResponse.FromErrorMessage("Unspecified Login Error");
        }


        public async Task<SaveDataResponse> CreateForgotPasswordToken(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) return SaveDataResponse.FromErrorMessage("Email address not found");

                var resetCode = new UserValidationToken
                {
                    User = user,
                    Token = Guid.NewGuid(),
                    TokenType = Domain.Enums.TokenTypes.ForgotPassword
                };
                _dbContext.Add(resetCode);

                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.IncludeData(resetCode.Token);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }

        }

        public async Task<SaveDataResponse> ResetPassword(string email, string resetCode, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) return SaveDataResponse.FromErrorMessage("Email address not found");

                var token = await _dbContext.Set<UserValidationToken>().FirstOrDefaultAsync(x => x.User.Id == user.Id && x.TokenType == Domain.Enums.TokenTypes.ForgotPassword && x.Token == new Guid(resetCode));
                if (token == null) return SaveDataResponse.FromErrorMessage("Reset token not found");

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, password);
                if (result.Succeeded)
                {
                    _dbContext.Remove(token);
                    await _dbContext.SaveChangesAsync();
                    return SaveDataResponse.Ok();
                }
                else
                {
                    return SaveDataResponse.FromErrorMessage(string.Join(Environment.NewLine, result.Errors));
                }
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }

        }
        public async Task<SaveDataResponse> Delete(long userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                await _userManager.DeleteAsync(user);
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<bool> IsAdmin(User user)
        {
            var admin = await _userManager.IsInRoleAsync(user, "admin");
            return admin;
        }
        public async Task<User> GetUserById(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        }

        public async Task<UserPhoto> GetUserPhotoById(long id)
        {
            var usr = await _dbContext.Users.Include(x => x.Photo).FirstOrDefaultAsync(x => x.Id == id);
            if (usr == null || usr.Photo == null) return null;
            return usr.Photo;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<SaveDataResponse> CreateUser(User user, string password)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded) return SaveDataResponse.IncludeData(user.Id);
                return SaveDataResponse.FromErrorMessage(string.Join(Environment.NewLine, result.Errors));
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> UpdateUser(User user)
        {
            try
            {
                await _userManager.UpdateAsync(user);
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
    }
}
