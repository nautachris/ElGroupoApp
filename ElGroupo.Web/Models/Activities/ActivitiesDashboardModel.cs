using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Activities
{

    public class RecordsModel
    {
        public List<RecordsListModel> Records { get; set; } = new List<RecordsListModel>();
    }
    public class RecordsListModel
    {
        public long Id { get; set; }
        public string GroupName { get; set; }
        public string ActivityDescription { get; set; }
        public string ActivityLocation { get; set; }
        public DateTime? ActivityStartDate { get; set; }
        public DateTime? ActivityEndDate { get; set; }
        public double CreditHours { get; set; }
        public string CreditType { get; set; }
        public string CreditCategory { get; set; }
        public bool IsPresenting { get; set; }
        public string Department { get; set; }
    }
    public class ActivitiesDashboardModel
    {
        //user groups -> activity groups within user groups
        public List<UserDepartmentModel> Departments { get; set; } = new List<UserDepartmentModel>();
        public long UserId { get; set; }

        public List<ActivityGroupModel> PrivateActivities { get; set; } = new List<ActivityGroupModel>();
    }
    public class UserDepartmentModel
    {
        public long Id { get; set; }
        public string DepartmentName { get; set; }
        public List<UserGroupModel> Groups { get; set; } = new List<UserGroupModel>();
    }
    public class UserGroupModel
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public List<ActivityGroupModel> ActivityGroups { get; set; } = new List<ActivityGroupModel> { new ActivityGroupModel { Name = "Other", Id = -1 } };
    }
    public class ActivityGroupModel
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public int ActivityCount { get; set; }
        public bool CanEdit { get; set; }
    }

    public class ActivityGroupActivitiesModel
    {
        public string ActivityGroupName { get; set; }
        public long ActivityGroupId { get; set; }
        public bool CanEditGroup { get; set; }
        public List<ActivityListModel> Activities { get; set; } = new List<ActivityListModel>();
    }

    public class ActivityListModel : LookupModel
    {
        public bool CanEditActivity { get; set; }
        public bool AttendenceLogged { get; set; }
        public long? UserActivityId { get; set; }
    }

    public class CreateActivityGroupModel : AssignCreditTypesModel
    {
        //if false, this is a one-off "personal" activity that will only be visible by the user looking at his/her credits
        //no one will be able to add
        public bool CreateFirstActivity { get; set; }

        public bool MakePublic { get; set; }

        public string GroupName { get; set; }

        //if the group is set to public, we not only create a group, but create the first activity within the group

        // public string ActivityName { get; set; }

        public List<LookupModel> AvailableGroups { get; set; }
        public List<long> SharedGroupIds { get; set; }

    }
    public class CreditCategoryModel
    {
        public long Id { get; set; }
        public long CreditTypeId { get; set; }
        public string Description { get; set; }
    }
    public abstract class ActivityModel
    {
        public string PersonalNotes { get; set; }
        public string ActivityDescription { get; set; }
        public string ActivityLocation { get; set; }

        public string PublicNotes { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<ActivityCreditModel> Credits { get; set; }


        public bool IsPresenting { get; set; }
        public string PresentationName { get; set; }


    }

    public abstract class AssignCreditTypesModel : ActivityModel
    {
        public List<LookupModel> CreditTypes { get; set; }
        public List<CreditCategoryModel> CreditCategories { get; set; }
    }
    public class CreateActivityModel : AssignCreditTypesModel
    {

        public long ActivityGroupId { get; set; }
        public string ActivityGroupName { get; set; }

    }

    public class SaveDocumentsModel
    {
        public long UserActivityId { get; set; }
        public List<DocumentListModel> Documents { get; set; } = new List<DocumentListModel>();

    }
    public class DocumentListModel
    {
        public bool Shared { get; set; }
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string ContentType { get; set; }
    }

    public class LogAttendenceModel : ActivityModel
    {
        public List<ViewActivityDocumentModel> Documents { get; set; } = new List<ViewActivityDocumentModel>();
        public long ActivityId { get; set; }
        public long ActivityGroupId { get; set; }
        public string ActivityGroupName { get; set; }

        public string ActivityTimeText { get; set; }

        public List<ActivityCreditListModel> AllowedCredits { get; set; } = new List<ActivityCreditListModel>();

    }

    public class EditLogAttendenceModel : LogAttendenceModel
    {
        public List<ViewUserActivityDocumentModel> Documents { get; set; } = new List<ViewUserActivityDocumentModel>();

        public List<ViewActivityDocumentModel> ActivityDocuments { get; set; } = new List<ViewActivityDocumentModel>();
        public long UserActivityId { get; set; }

    }

    public class ViewUserActivityDocumentModel
    {
        public string Description { get; set; }
        public long Id { get; set; }
        public string FileName { get; set; }
        public long UserActivityDocumentId { get; set; }
    }
    public class ViewActivityDocumentModel
    {
        public string Description { get; set; }
        public long Id { get; set; }
        public string FileName { get; set; }
    }
    public class ActivityCreditModel
    {
        public long CreditTypeCategoryId { get; set; }
        public double NumberOfCredits { get; set; }
    }

    public class ActivityCreditListModel
    {
        public long CreditTypeCategoryId { get; set; }
        public string CreditTypeCategoryName { get; set; }
        public long CreditTypeId { get; set; }
        public string CreditTypeName { get; set; }
        public double Hours { get; set; }

    }


    public class LookupModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
    }
}
