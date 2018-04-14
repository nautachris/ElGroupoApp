using ElGroupo.Domain;
using ElGroupo.Domain.Activities;
using ElGroupo.Domain.Data;
using ElGroupo.Web.Models.Account;
using ElGroupo.Web.Models.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Services
{
    public class AccountService : BaseService
    {
        public AccountService(ElGroupoDbContext ctx) : base(ctx)
        {

        }
        public User GetUser(long id)
        {
            return _dbContext.Users.FirstOrDefault(x => x.Id == id);
        }

        public async Task<SaveDataResponse> AddUserConnection(User user, long userId)
        {
            try
            {
                var connectedUser = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
                if (connectedUser == null) return SaveDataResponse.FromErrorMessage("User Id " + userId + " not found");
                var uc = new UserConnection
                {
                    User = user,
                    ConnectedUser = connectedUser
                };
                _dbContext.Add(uc);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> RemoveRegisteredConnection(User user, long userId)
        {
            try
            {
                var toRemove = await _dbContext.UserConnections.FirstOrDefaultAsync(x => x.User.Id == user.Id && x.ConnectedUser.Id == userId);
                if (toRemove == null) return SaveDataResponse.FromErrorMessage("Connected User " + userId + " not found");
                _dbContext.Remove(toRemove);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<UnregisteredEventAttendee> GetUnregisteredAttendee(Guid token)
        {
            return await _dbContext.UnregisteredEventAttendees.FirstOrDefaultAsync(x => x.RegisterToken == token);
        }

        public async Task<SaveDataResponse> ImportContacts(User user, ImportSelectContactModel[] contacts)
        {
            try
            {

                var registeredUsers = _dbContext.Users.Where(x => contacts.Select(y => y.Email).Contains(x.Email)).ToDictionary(z => z.Id, z => z.Email);
                var connectedEmails = _dbContext.UserConnections.Include(x => x.ConnectedUser).Include(x => x.User).Where(x => x.User.Id == user.Id).Select(x => x.ConnectedUser.Email).ToList();
                foreach (var c in contacts.Where(x => registeredUsers.Values.Contains(x.Email) && !connectedEmails.Contains(x.Email)))
                {
                    //these are already in system
                    var uc = new UserConnection
                    {
                        User = user,
                        ConnectedUser = _dbContext.Users.First(x => x.Email == c.Email)
                    };
                    _dbContext.Add(uc);
                }

                foreach (var c in contacts.Where(x => !registeredUsers.Values.Contains(x.Email)))
                {
                    //these are not in system
                    var urc = new UnregisteredUserConnection
                    {
                        User = user,
                        Email = c.Email,
                        Name = c.FirstName + " " + c.LastName,
                        Phone1Type = "",
                        Phone1Value = c.Phone1,
                        Phone2Type = "",
                        Phone2Value = c.Phone2
                    };

                    _dbContext.Add(urc);
                }

                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> VerifyEmail(string code)
        {
            try
            {
                var codeGuid = new Guid(code);
                var token = await _dbContext.Set<UserValidationToken>().Include(x => x.User).FirstOrDefaultAsync(x => x.TokenType == Domain.Enums.TokenTypes.EmailVerification && x.Token == codeGuid);
                if (token == null) return SaveDataResponse.FromErrorMessage("Could not locate email token");
                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == token.User.Id);
                user.EmailConfirmed = true;
                _dbContext.Update(user);
                _dbContext.Remove(token);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> CreateUserPhoto(string contentType, byte[] fileBytes)
        {
            try
            {
                UserPhoto photo = new UserPhoto { ContentType = contentType, ImageData = fileBytes };
                _dbContext.UserPhotos.Add(photo);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.IncludeData(photo);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<SaveDataResponse> UpdateConnectionRecordsForNewUser(User newUser)
        {
            try
            {
                var connections = _dbContext.Set<UnregisteredUserConnection>().Include(x => x.User).Where(x => x.Email == newUser.Email).ToList();
                foreach (var conn in connections)
                {
                    var uc = new UserConnection { User = conn.User, ConnectedUser = newUser };
                    _dbContext.Add(uc);
                    _dbContext.UnregisteredUserConnections.Remove(conn);
                }
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<bool> VerifyInviteToken(Guid token)
        {

            return await _dbContext.UnregisteredEventAttendees.AnyAsync(x => x.RegisterToken == token);
        }
        public async Task<SaveDataResponse> UpdateAttendeeRecordsForNewUser(User newUser)
        {
            try
            {
                var ueas = await _dbContext.UnregisteredEventAttendees.Include(x => x.Event).Where(x => x.Email == newUser.Email).ToListAsync();
                foreach (var uea in ueas)
                {
                    var ea = new EventAttendee
                    {
                        Event = uea.Event,
                        User = newUser,
                        Viewed = false,
                        ResponseStatus = Domain.Enums.RSVPTypes.None
                    };
                    _dbContext.UnregisteredEventAttendees.Remove(uea);
                    _dbContext.EventAttendees.Add(ea);
                }



                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public List<OrganizationListModel> GetOrganizations()
        {
            var orgs = new List<OrganizationListModel>();
            foreach (var org in _dbContext.Organizations) orgs.Add(new OrganizationListModel { Id = org.Id, Name = org.Name });
            return orgs.OrderBy(x => x.Name).ToList();
        }
        public List<OrganizationListModel> GetDepartmentsByOrganizationId(long orgId)
        {
            var orgs = new List<OrganizationListModel>();
            foreach (var org in _dbContext.Departments.Where(x => x.OrganizationId == orgId)) orgs.Add(new OrganizationListModel { Id = org.Id, Name = org.Name });
            return orgs.OrderBy(x => x.Name).ToList();
        }
        public async Task<AttendeeGroupModel> GetAttendeeGroup(long id)
        {
            var ag = await _dbContext.AttendeeGroups.Include(x => x.User).Include(x => x.Attendees).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (ag == null) return new AttendeeGroupModel
            {
                Name = "",
                Id = 0
            };
            return new AttendeeGroupModel
            {
                Name = ag.Name,
                Id = ag.Id,
                UserId = ag.User.Id,
                Users = ag.Attendees.Select(x => new AttendeeGroupUserModel { Name = x.User.Name, Email = x.User.Email, Id = x.User.Id }).ToList()
            };

        }

        public async Task<SaveDataResponse> CreateVerifyEmailToken(User user)
        {
            try
            {
                var token = new UserValidationToken
                {
                    User = user,
                    TokenType = Domain.Enums.TokenTypes.EmailVerification,
                    Token = Guid.NewGuid()
                };

                _dbContext.Add(token);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.IncludeData(token.Token);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> DeleteAttendeeGroup(User user, long id)
        {
            try
            {
                var attendeeGroup = await _dbContext.AttendeeGroups.Include(x => x.Attendees).FirstOrDefaultAsync(x => x.Id == id);
                if (attendeeGroup.User.Id != user.Id) return SaveDataResponse.FromErrorMessage("this attendee group does not belong to you");
                if (attendeeGroup != null)
                {
                    _dbContext.Remove(attendeeGroup);
                    await _dbContext.SaveChangesAsync();
                }

                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }



        public async Task<SaveDataResponse> UpdateAttendeeGroup(User user, AttendeeGroupModel model)
        {
            try
            {
                AttendeeGroup attendeeGroup = null;
                if (model.Id == 0)
                {
                    attendeeGroup = new AttendeeGroup { Name = model.Name, User = user };
                    _dbContext.Add(attendeeGroup);
                    foreach (var groupUser in model.Users)
                    {
                        var agu = new AttendeeGroupUser
                        {
                            AttendeeGroup = attendeeGroup,
                            User = _dbContext.Users.First(x => x.Id == groupUser.Id)
                        };
                        _dbContext.Add(agu);
                    }
                    await _dbContext.SaveChangesAsync();
                    return SaveDataResponse.Ok();
                }
                else
                {
                    attendeeGroup = await _dbContext.AttendeeGroups.Include(x => x.User).Include(x => x.Attendees).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == model.Id);
                    if (attendeeGroup.User.Id != user.Id) return SaveDataResponse.FromErrorMessage("this group does not belong to you");
                    var existingUsers = attendeeGroup.Attendees.Select(x => x.User.Id).ToList();
                    var updatedUsers = model.Users.Select(x => x.Id).ToList();

                    var newUserIds = updatedUsers.Where(x => !existingUsers.Contains(x)).ToList();
                    var deletedUserIds = existingUsers.Where(x => !updatedUsers.Contains(x)).ToList();

                    if (model.Name != attendeeGroup.Name || newUserIds.Count > 0 || deletedUserIds.Count > 0)
                    {
                        if (model.Name != attendeeGroup.Name)
                        {
                            attendeeGroup.Name = model.Name;
                            _dbContext.Update(attendeeGroup);
                        }

                        foreach (var newUserId in newUserIds)
                        {
                            var agu = new AttendeeGroupUser
                            {
                                AttendeeGroup = attendeeGroup,
                                User = _dbContext.Users.First(x => x.Id == newUserId)
                            };
                            _dbContext.Add(agu);
                        }
                        var toDelete = attendeeGroup.Attendees.Where(x => deletedUserIds.Contains(x.User.Id)).ToList();
                        _dbContext.RemoveRange(toDelete);
                        await _dbContext.SaveChangesAsync();

                    }
                    return SaveDataResponse.Ok();
                }
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<Dictionary<long, string>> GetContactTypes()
        {
            return await _dbContext.ContactTypes.ToDictionaryAsync(x => x.Id, x => x.Value);
        }
        public async Task<List<EditContactModel>> GetUserContactMethods(long userId)
        {
            return await _dbContext.UserContacts.Include(x => x.User).Where(x => x.User.Id == userId).Select(x => new EditContactModel
            {
                Id = x.Id,
                Value = x.Value,
                ContactTypeId = x.ContactMethod.Id,
                ContactTypeDescription = x.ContactMethod.Value

            }).ToListAsync();
        }
        public SelectUserDepartmentsModel GetUserDepartments(long userId)
        {
            var model = new SelectUserDepartmentsModel();
            //var userDepartments = _dbContext.DepartmentUsers.Where(x => x.UserId == userId).Select(x => x.Department).ToList();
            var departmentUserRecords = _dbContext.DepartmentUsers.Include(x => x.Department).Include(x => x.UserGroupUsers).Where(x => x.UserId == userId);
            var userGroups = departmentUserRecords.SelectMany(x => x.UserGroupUsers).Select(x => x.UserGroup.Id).Distinct();
            //we cant necessarily get departments from groups b/c the user doenst have to be in a group within a department
            //var groupIds = _dbContext.DepartmentUserGroupUsers.Where(x => departmentUserRecords.Select(y => y.Id).Contains(x.User.Id)).Select(x => x.Id).ToList();
            var userOrg = departmentUserRecords.Count() > 0 ? departmentUserRecords.Select(x => x.Department.OrganizationId).Distinct().First() : 0;
            foreach (var org in _dbContext.Organizations.Include(x => x.Departments).ThenInclude(x => x.UserGroups).OrderBy(x => x.Name))
            {
                model.Organizations.Add(new OrganizationListModel
                {
                    Id = org.Id,
                    Name = org.Name,
                    IsSelected = userOrg == org.Id,
                    Departments = org.Departments.Select(x => new SelectDepartmentModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsSelected = departmentUserRecords.Select(y => y.Department.Id).Contains(x.Id),
                        Groups = x.UserGroups.Select(y => new SelectDepartmentUserGroupModel
                        {
                            Id = y.Id,
                            Name = y.Name,
                            IsSelected = userGroups.Contains(y.Id)
                        }).ToList()
                    }).ToList()
                });
            }

            return model;
        }


        private class SequenceCompareResult<T>
        {
            public List<T> Added { get; set; }
            public List<T> Removed { get; set; }
            public List<T> InBoth { get; set; }
        }

        private SequenceCompareResult<T> CompareSequences<T>(IEnumerable<T> original, IEnumerable<T> updated)
        {

            return new SequenceCompareResult<T>
            {
                Removed = original.Where(x => !updated.Contains(x)).ToList(),
                Added = updated.Where(x => !original.Contains(x)).ToList(),
                InBoth = updated.Intersect(original).ToList()
            };

        }

        public async Task<SaveDataResponse> AddOrganizationDepartment(long userId, AddDepartmentModel model)
        {
            try
            {
                var org = _dbContext.Organizations.FirstOrDefault(x => x.Id == model.OrganizationId);
                if (org == null) return SaveDataResponse.FromErrorMessage("Organization Id " + model.OrganizationId + " does not exist");

                var dept = new Department { Organization = org, Name = model.DepartmentName };
                _dbContext.Add(dept);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.IncludeData(dept.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<SaveDataResponse> DeleteDepartment(long departmentId)
        {
            try
            {
                var dept = _dbContext.Departments.Include(x => x.Users).Include(x => x.UserGroups).ThenInclude(x => x.Users).Include(x => x.UserGroups).ThenInclude(x => x.ActivityGroups).FirstOrDefault(x => x.Id == departmentId);
                if (dept != null) _dbContext.Remove(dept);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.IncludeData(dept.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<SaveDataResponse> DeleteDepartmentGroup(long groupId)
        {
            try
            {
                var group = _dbContext.DepartmentUserGroups.Include(x => x.Users).Include(x => x.ActivityGroups).FirstOrDefault(x => x.Id == groupId);
                if (group != null) _dbContext.Remove(group);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.IncludeData(group.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> UpdateUserDepartments(long userId, EditUserDepartmentsModel[] model)
        {
            try
            {
                //this updates groups and department
                //remove groups
                //remove departments
                //add departments
                //add groups
                //var departmentsToRemove = new List<long>();
                //var departmentGroupsToRemove = new List<long>();
                //var departmentsToAdd = new List<long>


                var existingDepartmentUsers = _dbContext.DepartmentUsers.Include(x => x.Department).Where(x => x.UserId == userId).ToList();
                var existingDepartmentGroupUsers = _dbContext.DepartmentUserGroupUsers.Include(x => x.UserGroup).ThenInclude(x => x.Department).Where(x => existingDepartmentUsers.Select(y => y.Id).Contains(x.UserId)).ToList();
                var updatedDepartments = CompareSequences(existingDepartmentUsers.Select(x => x.Department.Id), model.Select(y => y.DepartmentId));

                //deal with removed departments and associated groups
                foreach (var toRemove in updatedDepartments.Removed)
                {
                    var thisDepartmentUser = existingDepartmentUsers.First(x => x.DepartmentId == toRemove);
                    foreach (var item in _dbContext.DepartmentUserGroupUsers.Include(x => x.UserGroup).Where(x => x.UserGroup.DepartmentId == toRemove && x.UserId == thisDepartmentUser.Id))
                    {
                        //remove from any group associated with a department we're about to remove this user from
                        _dbContext.Remove(item);
                    }
                    _dbContext.Remove(thisDepartmentUser);
                    //var departmentUser = _dbContext.DepartmentUsers.FirstOrDefault(x => x.DepartmentId == toRemove && x.UserId == userId);
                    //if (departmentUser != null) 
                }

                //add new departmentusers
                foreach (var toAdd in updatedDepartments.Added)
                {
                    var du = new DepartmentUser { User = _dbContext.Users.First(x => x.Id == userId), Department = _dbContext.Departments.First(x => x.Id == toAdd) };
                    _dbContext.Add(du);
                    foreach (var groupToAdd in model.First(x => x.DepartmentId == toAdd).GroupIds)
                    {
                        var dugu = new DepartmentUserGroupUser { UserGroup = _dbContext.DepartmentUserGroups.First(x => x.Id == groupToAdd), User = du };
                        _dbContext.Add(dugu);
                    }
                }



                //check for modified groups in departments not added or deleted
                foreach (var inBoth in updatedDepartments.InBoth)
                {
                    var thisDepartmentUser = existingDepartmentUsers.First(x => x.DepartmentId == inBoth);

                    //groups that this user is current a part of within this department
                    var existing = existingDepartmentGroupUsers.Where(x => x.UserGroup.DepartmentId == inBoth).Select(x => x.UserGroupId).ToList();
                    var updated = model.First(x => x.DepartmentId == inBoth).GroupIds;
                    var groupCompare = CompareSequences(existing, updated);
                    foreach (var toDelete in groupCompare.Removed)
                    {
                        var item = _dbContext.DepartmentUserGroupUsers.FirstOrDefault(x => x.UserGroupId == toDelete && x.UserId == thisDepartmentUser.Id);
                        if (item != null) _dbContext.Remove(item);
                    }
                    foreach (var toAdd in groupCompare.Added)
                    {
                        var dugu = new DepartmentUserGroupUser { UserGroup = _dbContext.DepartmentUserGroups.First(x => x.Id == toAdd), User = thisDepartmentUser };
                        _dbContext.Add(dugu);
                    }
                }


                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();


            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }

        }

        public async Task<EditAccountModel> GetAccountEditModel(long userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            //TimeZoneInfo.GetSystemTimeZones().Select(x=> new { bear = x.})
            //if (!_dbContext.Entry(user).Reference(x => x.Photo).IsLoaded) _dbContext.Entry(user).Reference(x => x.Photo).Load();
            return new EditAccountModel
            {
                Contacts = await GetUserContactMethods(userId),
                ContactTypes = await GetContactTypes(),
                Connections = await GetUserConnections(userId),
                AttendeeGroups = await GetUserAttendeeGroupList(userId),
                EmailAddress = user.Email,
                HasPhoto = user.PhotoId.HasValue,
                Id = user.Id,
                Name = user.Name,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ZipCode = user.ZipCode,
                TimeZones = TimeZoneInfo.GetSystemTimeZones().ToDictionary(x => x.Id, x => x.DisplayName),
                TimeZoneId = user.TimeZoneId
            };
        }
        public async Task<ViewAccountModel> GetAccountViewModel(long userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            //if (!_dbContext.Entry(user).Reference(x => x.Photo).IsLoaded) _dbContext.Entry(user).Reference(x => x.Photo).Load();
            return new ViewAccountModel
            {
                Contacts = await GetUserContactMethods(userId),
                ContactTypes = await GetContactTypes(),
                EmailAddress = user.Email,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                ZipCode = user.ZipCode
            };
        }

        public async Task<List<UserConnectionModel>> GetUserConnections(long userId)
        {
            try
            {
                return await _dbContext.UserConnections.Include(x => x.User).Where(x => x.User.Id == userId).Select(x => new UserConnectionModel
                {
                    Name = x.ConnectedUser.Name,
                    Email = x.ConnectedUser.Email,
                    Phone = x.ConnectedUser.ContactMethods.Any(y => y.ContactMethod.Value == "Phone") ? x.ConnectedUser.ContactMethods.First(y => y.ContactMethod.Value == "Phone").Value : "",
                    Registered = true,
                    UserId = x.ConnectedUser.Id
                }).Union(_dbContext.UnregisteredUserConnections.Where(x => x.User.Id == userId).Select(x => new UserConnectionModel
                {
                    Name = x.Name,
                    Email = x.Email,
                    Phone = "",
                    Registered = false
                })).ToListAsync();
                //return await _dbContext.UserConnections.Include(x => x.User).Where(x => x.User.Id == userId).Select(x => new UserConnectionModel
                //{
                //    Name = x.ConnectedUser.Name,
                //    Email = x.ConnectedUser.Email,
                //    Phone = x.ConnectedUser.ContactMethods.Any(y => y.ContactMethod.Value == "Phone") ? x.ConnectedUser.ContactMethods.First(y => y.ContactMethod.Value == "Phone").Value : "",
                //    Registered = true,
                //    UserId = x.ConnectedUser.Id
                //}).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<UserConnectionModel>();
            }

        }
        public async Task<List<AttendeeGroupListModel>> GetUserAttendeeGroupList(long userId)
        {
            try
            {
                var list = await _dbContext.AttendeeGroups.Include(x => x.Attendees).Where(x => x.User.Id == userId).Select(x => new AttendeeGroupListModel
                {
                    Name = x.Name,
                    Id = x.Id,
                    UserCount = x.Attendees.Count
                }).ToListAsync();
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
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
        public async Task<SaveDataResponse> UpdateUser(EditAccountModel model)
        {
            try
            {
                var userRecord = _dbContext.Set<User>().Include(x => x.Photo).Include(x => x.ContactMethods).ThenInclude(x => x.ContactMethod).First(x => x.Id == model.Id);
                userRecord.ZipCode = model.ZipCode;
                userRecord.Email = model.EmailAddress;
                userRecord.PhoneNumber = model.PhoneNumber;
                userRecord.Name = model.Name;
                userRecord.FirstName = model.FirstName;
                userRecord.LastName = model.LastName;
                userRecord.TimeZoneId = model.TimeZoneId;
                if (model.UpdatedPhoto != null)
                {
                    byte[] fileBytes = CreateThumbnail(model.UpdatedPhoto.OpenReadStream(), model.UpdatedPhoto.ContentType);
                    if (userRecord.Photo == null)
                    {
                        var newPhoto = new UserPhoto
                        {
                            ContentType = model.UpdatedPhoto.ContentType,
                            ImageData = fileBytes
                        };
                        _dbContext.UserPhotos.Add(newPhoto);
                        userRecord.Photo = newPhoto;

                    }
                    else
                    {
                        userRecord.Photo.ContentType = model.UpdatedPhoto.ContentType;
                        userRecord.Photo.ImageData = fileBytes;
                    }


                }

                _dbContext.Users.Update(userRecord);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> CreateContactMethod(EditContactModel model, long userId)
        {
            try
            {
                var userRecord = await _dbContext.Set<User>().Include(x => x.ContactMethods).FirstOrDefaultAsync(x => x.Id == userId);
                if (userRecord == null) return SaveDataResponse.FromErrorMessage("user " + userId.ToString() + " does not exist");
                var ct = _dbContext.ContactTypes.First(x => x.Id == model.ContactTypeId);
                userRecord.AddContact(ct, model.Value);
                _dbContext.Update(userRecord);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> UpdateUserContactMethod(long id, string value)
        {
            try
            {
                var c = await _dbContext.UserContacts.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
                if (c == null) return SaveDataResponse.FromErrorMessage("contact method record not found");
                c.Value = value;
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> DeleteUserContactMethod(long id)
        {
            try
            {
                var c = await _dbContext.UserContacts.FirstOrDefaultAsync(x => x.Id == id);
                if (c == null) return SaveDataResponse.FromErrorMessage("contact method record not found");
                _dbContext.UserContacts.Remove(c);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> Test()
        {
            try
            {
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }


    }
}
