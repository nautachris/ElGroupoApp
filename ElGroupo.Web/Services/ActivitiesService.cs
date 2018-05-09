using ElGroupo.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElGroupo.Web.Models.Activities;
using Microsoft.EntityFrameworkCore;
using ElGroupo.Web.Models.Shared;
using ElGroupo.Domain.Activities;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ElGroupo.Web.Services
{
    public class ActivitiesService : BaseService
    {
        public ActivitiesService(ElGroupoDbContext ctx) : base(ctx)
        {

        }
        public async Task<SaveDataResponse> EditAttendenceLog(EditLogAttendenceModel model, long userId)
        {


            var existingUserActivity = _dbContext.UserActivities.Include(x => x.Credits).First(x => x.Id == model.UserActivityId);
            existingUserActivity.AttendanceTypeId = model.AttendenceType;

            //check for modified
            foreach (var credit in model.Credits)
            {
                if (existingUserActivity.Credits.Any(x => x.CreditTypeCategoryId == credit.CreditTypeCategoryId)) existingUserActivity.Credits.First(x => x.CreditTypeCategoryId == credit.CreditTypeCategoryId).CreditHours = credit.NumberOfCredits;
            }

            var toRemove = existingUserActivity.Credits.Where(x => !model.Credits.Select(y => y.CreditTypeCategoryId).Contains(x.CreditTypeCategoryId)).ToList();
            foreach (var remove in toRemove) existingUserActivity.Credits.Remove(remove);

            var toAdd = model.Credits.Where(x => !existingUserActivity.Credits.Select(y => y.CreditTypeCategoryId).Contains(x.CreditTypeCategoryId));
            foreach (var add in toAdd) existingUserActivity.Credits.Add(new UserActivityCredit { CreditTypeCategoryId = add.CreditTypeCategoryId, CreditHours = add.NumberOfCredits });

            _dbContext.Update(existingUserActivity);


            await _dbContext.SaveChangesAsync();
            return SaveDataResponse.Ok();
        }
        public async Task<SaveDataResponse> AddAttendenceLog(LogAttendenceModel model, long userId)
        {
            var activity = _dbContext.Activities.First(x => x.Id == model.ActivityId);
            var user = _dbContext.Users.First(x => x.Id == userId);
            var userActivity = new UserActivity { Activity = activity, User = user, AttendanceTypeId = model.AttendenceType, Credits = new List<UserActivityCredit>() };
            userActivity.Credits = model.Credits.Select(x => new UserActivityCredit { CreditHours = x.NumberOfCredits, CreditTypeCategoryId = x.CreditTypeCategoryId }).ToList();
            _dbContext.Add(userActivity);


            await _dbContext.SaveChangesAsync();
            return SaveDataResponse.IncludeData(userActivity.Id);
        }
        public async Task<SaveDataResponse> DeleteActivity(long activityId, long userId, bool isAdmin)
        {
            try
            {
                var activity = _dbContext.Activities.Include(x => x.Organizers).Include(x => x.Credits).Include(x => x.Users).ThenInclude(x => x.Credits).Include(x => x.Users).ThenInclude(x => x.Documents).ThenInclude(x => x.Document).ThenInclude(x => x.Activities).First(x => x.Id == activityId);
                if (!isAdmin && !activity.Organizers.Select(x => x.UserId).Contains(userId)) throw new Exception("unauthorized user");

                var docsToRemove = new List<long>();
                foreach (var userActivity in activity.Users)
                {
                    docsToRemove.AddRange(userActivity.Documents.Where(x => x.Document.Activities.Count == 1).Select(x => x.Document.Id));

                }

                _dbContext.Remove(activity);
                foreach (var doc in _dbContext.AccreditationDocuments.Where(x => docsToRemove.Contains(x.Id))) _dbContext.Remove(doc);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }


            return SaveDataResponse.Ok();
        }

        public async Task<SaveDataResponse> DeleteActivityGroup(long activityGroupId, long userId, bool isAdmin)
        {
            try
            {
                var org = _dbContext.ActivityGroupOrganizers.FirstOrDefault(x => x.ActivityGroupId == activityGroupId && x.UserId == userId);
                if (!isAdmin && org == null) throw new Exception("unauthorized");

                var group = _dbContext.ActivityGroups.Include(x => x.Organizers).Include(x => x.Activities).ThenInclude(x => x.Organizers).Include(x => x.Activities).ThenInclude(x => x.Credits).Include(x => x.Activities).ThenInclude(x => x.Users).Include(x => x.Activities).ThenInclude(x => x.Users).ThenInclude(x => x.Documents).ThenInclude(x => x.Document).ThenInclude(x => x.Activities).FirstOrDefault(x => x.Id == activityGroupId);


                if (group == null) throw new Exception("invalid group id");
                foreach (var item in _dbContext.DepartmentUserGroupActivityGroups.Include(x => x.ActivityGroup).Where(x => x.ActivityGroup.Id == activityGroupId))
                {
                    _dbContext.Remove(item);
                }
                var docsToRemove = new List<long>();
                foreach (var userActivity in group.Activities.SelectMany(x => x.Users))
                {
                    docsToRemove.AddRange(userActivity.Documents.Where(x => x.Document.Activities.Count == 1).Select(x => x.Document.Id));

                }
                _dbContext.Remove(group);

                foreach (var doc in _dbContext.AccreditationDocuments.Where(x => docsToRemove.Contains(x.Id))) _dbContext.Remove(doc);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }


            return SaveDataResponse.Ok();
        }


        public async Task<RecordsModel> GetUserRecords(long userId, string timeZoneId)
        {
            var myRecords = await _dbContext.UserActivities.Include(x => x.Credits).Include(x => x.AttendanceType).Include(x => x.Activity).ThenInclude(x => x.ActivityGroup).ThenInclude(x => x.Department).Where(x => x.UserId == userId).ToListAsync();
            var model = new RecordsModel();
            
            foreach (var record in myRecords.OrderByDescending(x => x.Activity.StartDate))
            {
                model.Records.Add(new RecordsListModel
                {
                    ActivityDate = TimeZoneInfo.ConvertTimeToUtc(record.Activity.StartDate.Value, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)),
                    ActivityDescription = record.Activity.Description,
                    AttendenceType = record.AttendanceType.Description,
                    Department = record.Activity.ActivityGroup.Department.Name,
                    GroupName = record.Activity.ActivityGroup.Name,
                    Id = record.Id,
                    TotalCredits = record.Credits.Sum(x => x.CreditHours)
                });
            }
            return model;
        }

        public async Task<SaveDataResponse> SaveNewActivity(CreateActivityModel model, long userId)
        {
            try
            {
                var user = _dbContext.Users.First(x => x.Id == userId);
                var group = _dbContext.ActivityGroups.First(x => x.Id == model.ActivityGroupId);



                var activity = new Activity
                {
                    Description = model.ActivityDescription,
                    Location = model.ActivityLocation,
                    ActivityGroup = group,
                    Credits = new List<ActivityCredit>()
                };
                if (model.StartTime.HasValue)
                {
                    if (model.StartTime.Value.Kind != DateTimeKind.Utc)
                    {
                        activity.StartDate = TimeZoneInfo.ConvertTimeToUtc(model.StartTime.Value, TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneId));
                    }
                    else
                    {
                        activity.StartDate = model.StartTime.Value;
                    }

                }
                if (model.EndTime.HasValue)
                {
                    if (model.EndTime.Value.Kind != DateTimeKind.Utc)
                    {
                        activity.EndDate = TimeZoneInfo.ConvertTimeToUtc(model.EndTime.Value, TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneId));
                    }
                    else
                    {
                        activity.EndDate = model.EndTime.Value;
                    }
                }

                foreach (var creditTypeCategory in _dbContext.CreditTypeCategories.Where(x => model.Credits.Select(y => y.CreditTypeCategoryId).Contains(x.Id)))
                {
                    activity.Credits.Add(new ActivityCredit { CreditTypeCategory = creditTypeCategory });
                }
                _dbContext.Add(activity);

                var ao = new ActivityOrganizer { Activity = activity, User = user };
                _dbContext.Add(ao);
                var userActivity = new UserActivity { Activity = activity, User = user, AttendanceTypeId = model.AttendenceType, Credits = new List<UserActivityCredit>() };
                userActivity.Credits = model.Credits.Select(x => new UserActivityCredit { CreditHours = x.NumberOfCredits, CreditTypeCategoryId = x.CreditTypeCategoryId }).ToList();
                _dbContext.Add(userActivity);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.IncludeData(userActivity.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> SaveNewActivityGroup(CreateActivityGroupModel model, long userId)
        {
            try
            {
                var user = _dbContext.Users.First(x => x.Id == userId);
                var activityGroup = new ActivityGroup
                {
                    Name = model.GroupName
                };
                if (model.MakePublic)
                {
                    var groups = _dbContext.DepartmentUserGroups.Include(x => x.Department).Where(x => model.SharedGroupIds.Contains(x.Id));
                    if (groups.Any()) activityGroup.Department = groups.First().Department;
                    _dbContext.Add(activityGroup);
                    foreach (var group in groups)
                    {
                        var activityGroupUserGroup = new DepartmentUserGroupActivityGroup { ActivityGroup = activityGroup, UserGroup = group };
                        _dbContext.Add(activityGroupUserGroup);
                    }
                }
                else
                {
                    activityGroup.User = user;
                    _dbContext.Add(activityGroup);
                }


                var ago = new ActivityGroupOrganizer { ActivityGroup = activityGroup, User = user };
                _dbContext.Add(ago);



                var activity = new Activity
                {
                    Description = model.ActivityDescription,
                    Location = model.ActivityLocation,
                    ActivityGroup = activityGroup,
                    Credits = new List<ActivityCredit>()
                };
                if (model.StartTime.Value.Kind != DateTimeKind.Utc)
                {
                    activity.StartDate = TimeZoneInfo.ConvertTimeToUtc(model.StartTime.Value, TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneId));
                }
                else
                {
                    activity.StartDate = model.StartTime.Value;
                }
                if (model.EndTime.Value.Kind != DateTimeKind.Utc)
                {
                    activity.EndDate = TimeZoneInfo.ConvertTimeToUtc(model.EndTime.Value, TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneId));
                }
                else
                {
                    activity.EndDate = model.EndTime.Value;
                }

                foreach (var creditTypeCategory in _dbContext.CreditTypeCategories.Where(x => model.Credits.Select(y => y.CreditTypeCategoryId).Contains(x.Id)))
                {
                    activity.Credits.Add(new ActivityCredit { CreditTypeCategory = creditTypeCategory });
                }
                _dbContext.Add(activity);

                var userActivity = new UserActivity { Activity = activity, User = user, AttendanceTypeId = model.AttendenceType, Credits = new List<UserActivityCredit>() };
                userActivity.Credits = model.Credits.Select(x => new UserActivityCredit { CreditHours = x.NumberOfCredits, CreditTypeCategoryId = x.CreditTypeCategoryId }).ToList();
                _dbContext.Add(userActivity);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.IncludeData(userActivity.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public ActivityGroupActivitiesModel GetActivityGroupActivitiesModel(long groupId, long userId, bool isAdmin)
        {

            var group = _dbContext.ActivityGroups.Include(x => x.Activities).ThenInclude(x => x.Organizers).Include(x => x.Organizers).FirstOrDefault(x => x.Id == groupId);
            var attendedActivities = _dbContext.UserActivities.Where(x => x.User.Id == userId).ToDictionary(x => x.ActivityId, x => x.Id);
            if (group == null) return null;
            var model = new ActivityGroupActivitiesModel { ActivityGroupId = group.Id, ActivityGroupName = group.Name, CanEditGroup = isAdmin || group.Organizers.Select(x => x.UserId).Contains(userId) };
            foreach (var activity in group.Activities.OrderBy(x => x.StartDate))
            {
                var activityModel = new ActivityListModel { Id = activity.Id, Description = activity.Description };
                if (isAdmin) activityModel.CanEditActivity = true;
                else activityModel.CanEditActivity = activity.Organizers.Select(x => x.UserId).Contains(userId);
                if (attendedActivities.ContainsKey(activity.Id))
                {
                    activityModel.AttendenceLogged = true;
                    activityModel.UserActivityId = attendedActivities[activity.Id];
                }
                model.Activities.Add(activityModel);
            }


            return model;
        }

        public string GetSimpleDateText(string timeZoneId, DateTime? date)
        {
            if (date == null) return null;
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            var localTime = TimeZoneInfo.ConvertTimeFromUtc(date.Value, tz);
            return localTime.ToString("d") + " " + localTime.ToString("t");

        }
        public CreateActivityModel GetCreateActivityModel(long userId, long groupId)
        {
            //if groupId has value, that means the user clicked "Other" from within a group, therefore that group will by default be selected and "Is Public" will be true
            var model = new CreateActivityModel();
            model.AttendenceTypes = _dbContext.ActivityAttendanceTypes.Select(x => new LookupModel { Id = x.Id, Description = x.Description }).OrderBy(x => x.Description).ToList();
            model.CreditTypes = _dbContext.CreditTypes.Select(x => new LookupModel { Id = x.Id, Description = x.Description }).OrderBy(x => x.Description).ToList();
            model.CreditCategories = _dbContext.CreditTypeCategories.Select(x => new CreditCategoryModel { Id = x.Id, CreditTypeId = x.CreditTypeId, Description = x.Description }).OrderBy(x => x.Description).ToList();
            model.ActivityGroupId = groupId;
            model.ActivityGroupName = _dbContext.ActivityGroups.First(x => x.Id == groupId).Name;

            return model;

        }

        public AccreditationDocument GetDocument(long id)
        {
            var doc = _dbContext.AccreditationDocuments.First(x => x.Id == id);
            return doc;
        }

        public async Task<SaveDataResponse> DeleteDocument(long userActivityDocumentId)
        {
            try
            {
                var doc = _dbContext.UserActivityDocuments.Include(x => x.Document).ThenInclude(x => x.Activities).FirstOrDefault(x => x.Id == userActivityDocumentId);
                bool deleteDocument = doc.Document.Activities.Count == 1;
                _dbContext.Remove(doc);
                if (deleteDocument) _dbContext.Remove(doc.Document);
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }

        }

        public async Task<SaveDataResponse> SaveDocument(SaveDocumentsModel model)
        {
            try
            {
                var userActivity = _dbContext.UserActivities.First(x => x.Id == model.UserActivityId);
                foreach (var doc in model.Documents)
                {
                    AccreditationDocument newDocument = new AccreditationDocument
                    {
                        ImageData = doc.Data,
                        ContentType = doc.ContentType,
                        Description = doc.Description,
                        FileName = doc.FileName,
                        Activities = new List<UserActivityDocument>()
                    };
                    newDocument.Activities.Add(new UserActivityDocument { UserActivity = userActivity });
                    _dbContext.Add(newDocument);
                }
                await _dbContext.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public LogAttendenceModel GetCreateAttendenceLogModel(long activityId, long userId, string timeZoneId)
        {
            //if groupId has value, that means the user clicked "Other" from within a group, therefore that group will by default be selected and "Is Public" will be true
            var model = new LogAttendenceModel();
            var activity = _dbContext.Activities.Include(x => x.ActivityGroup).FirstOrDefault(x => x.Id == activityId);

            //check if user previously logged
            //var existingRecord = _dbContext.UserActivities.FirstOrDefault(x => x.UserId == userId && x.ActivityId == activityId);
            //if (existingRecord != null)
            //{
            //    model.UserActivityId = existingRecord.Id;
            //    model.AttendenceType = existingRecord.AttendanceTypeId;
            //    model.Credits = _dbContext.UserActivityCredits.Where(x => x.UserActivityId == existingRecord.Id).Select(x => new ActivityCreditModel { CreditTypeCategoryId = x.CreditTypeCategoryId, NumberOfCredits = x.CreditHours }).ToList();

            //    //_dbContext.UserActivityDocuments.Include(x=>x.Document).Where(x=>x.UserActivityId == existingRecord.Id)
            //}
            model.ActivityDescription = activity.Description;
            model.ActivityLocation = activity.Location;
            model.ActivityGroupName = activity.ActivityGroup.Name;
            model.ActivityId = activity.Id;
            model.ActivityGroupId = activity.ActivityGroup.Id;
            model.ActivityStartTimeText = GetSimpleDateText(timeZoneId, activity.StartDate);
            model.AttendenceTypes = _dbContext.ActivityAttendanceTypes.Select(x => new LookupModel { Id = x.Id, Description = x.Description }).OrderBy(x => x.Description).ToList();
            model.CreditTypes = _dbContext.CreditTypes.Select(x => new LookupModel { Id = x.Id, Description = x.Description }).OrderBy(x => x.Description).ToList();
            model.CreditCategories = _dbContext.CreditTypeCategories.Select(x => new CreditCategoryModel { Id = x.Id, CreditTypeId = x.CreditTypeId, Description = x.Description }).OrderBy(x => x.Description).ToList();
            ;

            return model;

        }

        public EditLogAttendenceModel GetEditAttendenceLogModel(long userActivityId, long userId, string timeZoneId)
        {
            //if groupId has value, that means the user clicked "Other" from within a group, therefore that group will by default be selected and "Is Public" will be true
            var model = new EditLogAttendenceModel();


            //check if user previously logged
            var existingRecord = _dbContext.UserActivities.Include(x => x.Activity).ThenInclude(x => x.ActivityGroup).FirstOrDefault(x => x.Id == userActivityId);
            if (existingRecord == null) throw new Exception("record is null");
            if (existingRecord.UserId != userId) throw new Exception("user ids do not match");

            model.UserActivityId = existingRecord.Id;
            model.AttendenceType = existingRecord.AttendanceTypeId;
            model.Credits = _dbContext.UserActivityCredits.Where(x => x.UserActivityId == existingRecord.Id).Select(x => new ActivityCreditModel { CreditTypeCategoryId = x.CreditTypeCategoryId, NumberOfCredits = x.CreditHours }).ToList();


            model.ActivityDescription = existingRecord.Activity.Description;
            model.ActivityLocation = existingRecord.Activity.Location;
            model.ActivityGroupName = existingRecord.Activity.ActivityGroup.Name;
            model.ActivityId = existingRecord.Activity.Id;
            model.ActivityGroupId = existingRecord.Activity.ActivityGroup.Id;
            model.ActivityStartTimeText = GetSimpleDateText(timeZoneId, existingRecord.Activity.StartDate);
            model.AttendenceTypes = _dbContext.ActivityAttendanceTypes.Select(x => new LookupModel { Id = x.Id, Description = x.Description }).OrderBy(x => x.Description).ToList();
            model.CreditTypes = _dbContext.CreditTypes.Select(x => new LookupModel { Id = x.Id, Description = x.Description }).OrderBy(x => x.Description).ToList();
            model.CreditCategories = _dbContext.CreditTypeCategories.Select(x => new CreditCategoryModel { Id = x.Id, CreditTypeId = x.CreditTypeId, Description = x.Description }).OrderBy(x => x.Description).ToList();
            ;
            model.Documents = _dbContext.UserActivityDocuments.Include(x => x.UserActivity).Include(x => x.Document).Where(x => x.UserActivity.Id == userActivityId).Select(x => new ViewDocumentModel
            {
                FileName = x.Document.FileName,
                Description = x.Document.Description,
                Id = x.Document.Id,
                UserActivityDocumentId = x.Id
            }).ToList();

            return model;

        }
        public CreateActivityGroupModel GetCreateActivityGroupModel(long userId, long? groupId = null)
        {
            //if groupId has value, that means the user clicked "Other" from within a group, therefore that group will by default be selected and "Is Public" will be true
            var model = new CreateActivityGroupModel();
            model.AttendenceTypes = _dbContext.ActivityAttendanceTypes.Select(x => new LookupModel { Id = x.Id, Description = x.Description }).OrderBy(x => x.Description).ToList();
            model.CreditTypes = _dbContext.CreditTypes.Select(x => new LookupModel { Id = x.Id, Description = x.Description }).OrderBy(x => x.Description).ToList();
            model.CreditCategories = _dbContext.CreditTypeCategories.Select(x => new CreditCategoryModel { Id = x.Id, CreditTypeId = x.CreditTypeId, Description = x.Description }).OrderBy(x => x.Description).ToList();

            if (groupId.HasValue)
            {
                model.MakePublic = true;
                model.AvailableGroups = new List<LookupModel>();
                foreach (var dept in _dbContext.DepartmentUsers.Include(x => x.Department).Where(x => x.User.Id == userId))
                {
                    //var deptModel = new UserDepartmentModel { DepartmentName = dept.Department.Name, Id = dept.Department.Id };
                    foreach (var userGroup in _dbContext.DepartmentUserGroupUsers.Include(x => x.User).Where(x => x.User.Id == dept.Id).Select(x => x.UserGroup))
                    {
                        model.AvailableGroups.Add(new LookupModel { Id = userGroup.Id, Description = dept.Department.Name + " - " + userGroup.Name });
                    }
                }
                model.AvailableGroups = model.AvailableGroups.OrderBy(x => x.Description).ToList();
                model.SharedGroupIds = new List<long> { groupId.Value };
            }
            else
            {
                model.MakePublic = false;
            }
            return model;

        }

        public ActivitiesDashboardModel GetDashboardModel(long userId, bool isAdmin)
        {
            var model = new ActivitiesDashboardModel();

            //var associatedGroups = _dbContext.DepartmentUserGroupUsers.Include(x => x.User).Where(x => x.User.UserId == userId).Select(x => new LookupModel { Description = x.UserGroup.Name, Id = x.UserGroup.Id }).ToList();
            foreach (var dept in _dbContext.DepartmentUsers.Include(x => x.Department).Where(x => x.User.Id == userId))
            {
                var deptModel = new UserDepartmentModel { DepartmentName = dept.Department.Name, Id = dept.Department.Id };
                foreach (var userGroup in _dbContext.DepartmentUserGroupUsers.Include(x => x.User).Where(x => x.User.Id == dept.Id).Select(x => x.UserGroup))
                {
                    var userGroupModel = new UserGroupModel { Id = userGroup.Id, Name = userGroup.Name };
                    userGroupModel.ActivityGroups = _dbContext.DepartmentUserGroupActivityGroups.Include(x => x.ActivityGroup).ThenInclude(x => x.Activities).Include(x => x.ActivityGroup).ThenInclude(x => x.Organizers).Where(x => x.UserGroupId == userGroup.Id).Select(x => new ActivityGroupModel { Id = x.ActivityGroupId, Name = x.ActivityGroup.Name, ActivityCount = x.ActivityGroup.Activities.Count, CanEdit = x.ActivityGroup.Organizers.Select(y => y.UserId).Contains(userId) }).ToList();

                    //temporarily removed per andy 4/26
                    if (isAdmin) userGroupModel.ActivityGroups.Add(new ActivityGroupModel { Id = -1, Name = "Create New" });
                    //
                    deptModel.Groups.Add(userGroupModel);
                }
                model.Departments.Add(deptModel);
            }
            model.PrivateActivities = _dbContext.ActivityGroups.Include(x => x.Activities).Where(x => x.UserId.HasValue && x.UserId.Value == userId).Select(x => new ActivityGroupModel { Name = x.Name, Id = x.Id, ActivityCount = x.Activities.Count }).ToList();
            model.PrivateActivities.Add(new ActivityGroupModel { Id = -1, Name = "Create New" });
            return model;
        }
    }
}
