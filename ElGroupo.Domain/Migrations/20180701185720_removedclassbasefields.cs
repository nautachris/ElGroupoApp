using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class removedclassbasefields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "UserPhoto");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "UserPhoto");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "UserPhoto");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "UserPhoto");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "UserContactMethods");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "UserContactMethods");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "UserContactMethods");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "UserContactMethods");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "UserConnection");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "UserConnection");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "UserConnection");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "UserConnection");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "UnregisteredUserConnections");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "UnregisteredUserConnections");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "UnregisteredUserConnections");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "UnregisteredUserConnections");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                schema: "dbo",
                table: "UnregisteredEventAttendees");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                schema: "dbo",
                table: "UnregisteredEventAttendees");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                schema: "dbo",
                table: "UnregisteredEventAttendees");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                schema: "dbo",
                table: "UnregisteredEventAttendees");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "RecurringEvent");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "RecurringEvent");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "RecurringEvent");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "RecurringEvent");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "RecordSubCategories");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "RecordSubCategories");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "RecordSubCategories");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "RecordSubCategories");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "RecordItemUserDocuments");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "RecordItemUserDocuments");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "RecordItemUserDocuments");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "RecordItemUserDocuments");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "RecordItemUserData");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "RecordItemUserData");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "RecordItemUserData");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "RecordItemUserData");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "RecordItemUsers");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "RecordItemUsers");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "RecordItemUsers");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "RecordItemUsers");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "RecordItemElements");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "RecordItemElements");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "RecordItemElements");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "RecordItemElements");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "RecordItems");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "RecordItems");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "RecordItems");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "RecordItems");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "RecordElementLookupTables");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "RecordElementLookupTables");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "RecordElementLookupTables");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "RecordElementLookupTables");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "RecordElementDataTypes");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "RecordElementDataTypes");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "RecordElementDataTypes");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "RecordElementDataTypes");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "RecordElements");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "RecordElements");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "RecordElements");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "RecordElements");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "RecordCategories");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "RecordCategories");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "RecordCategories");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "RecordCategories");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "MessageBoardTopics");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "MessageBoardTopics");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "MessageBoardTopics");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "MessageBoardTopics");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "MessageBoardItemAttendees");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "MessageBoardItemAttendees");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "MessageBoardItemAttendees");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "MessageBoardItemAttendees");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "MessageBoardItems");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "MessageBoardItems");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "MessageBoardItems");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "MessageBoardItems");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                schema: "dbo",
                table: "ContactMethods");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                schema: "dbo",
                table: "ContactMethods");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                schema: "dbo",
                table: "ContactMethods");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                schema: "dbo",
                table: "ContactMethods");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "EventNotifications");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "EventNotifications");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "EventNotifications");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "EventNotifications");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "EventAttendeeNotifications");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "EventAttendeeNotifications");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "EventAttendeeNotifications");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "EventAttendeeNotifications");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "EventAttendees");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "EventAttendees");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "EventAttendees");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "EventAttendees");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "AttendeeGroupUser");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "AttendeeGroupUser");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "AttendeeGroupUser");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "AttendeeGroupUser");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "AttendeeGroup");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "AttendeeGroup");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "AttendeeGroup");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "AttendeeGroup");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "UserActivityDocuments");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "UserActivityDocuments");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "UserActivityDocuments");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "UserActivityDocuments");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "UserActivityCredits");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "UserActivityCredits");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "UserActivityCredits");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "UserActivityCredits");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "OrganizationAccreditations");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "OrganizationAccreditations");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "OrganizationAccreditations");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "OrganizationAccreditations");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "DepartmentUserGroupUsers");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "DepartmentUserGroupUsers");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "DepartmentUserGroupUsers");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "DepartmentUserGroupUsers");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "DepartmentUserGroupActivityGroups");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "DepartmentUserGroupActivityGroups");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "DepartmentUserGroupActivityGroups");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "DepartmentUserGroupActivityGroups");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "DepartmentUserGroups");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "DepartmentUserGroups");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "DepartmentUserGroups");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "DepartmentUserGroups");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "DepartmentUsers");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "DepartmentUsers");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "DepartmentUsers");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "DepartmentUsers");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "CreditTypeCategories");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "CreditTypeCategories");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "CreditTypeCategories");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "CreditTypeCategories");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "CreditTypes");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "CreditTypes");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "CreditTypes");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "CreditTypes");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ActivityOrganizers");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "ActivityOrganizers");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "ActivityOrganizers");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "ActivityOrganizers");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ActivityGroupOrganizers");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "ActivityGroupOrganizers");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "ActivityGroupOrganizers");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "ActivityGroupOrganizers");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ActivityGroups");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "ActivityGroups");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "ActivityGroups");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "ActivityGroups");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ActivityDocuments");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "ActivityDocuments");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "ActivityDocuments");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "ActivityDocuments");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ActivityCredits");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "ActivityCredits");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "ActivityCredits");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "ActivityCredits");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "AccreditationDocuments");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "AccreditationDocuments");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "AccreditationDocuments");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "AccreditationDocuments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "UserTokens",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "UserTokens",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "UserTokens",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "UserTokens",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "UserPhoto",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "UserPhoto",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "UserPhoto",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "UserPhoto",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "UserContactMethods",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "UserContactMethods",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "UserContactMethods",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "UserContactMethods",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "UserConnection",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "UserConnection",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "UserConnection",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "UserConnection",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "UnregisteredUserConnections",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "UnregisteredUserConnections",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "UnregisteredUserConnections",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "UnregisteredUserConnections",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                schema: "dbo",
                table: "UnregisteredEventAttendees",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                schema: "dbo",
                table: "UnregisteredEventAttendees",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                schema: "dbo",
                table: "UnregisteredEventAttendees",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                schema: "dbo",
                table: "UnregisteredEventAttendees",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "RecurringEvent",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "RecurringEvent",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "RecurringEvent",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "RecurringEvent",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "RecordSubCategories",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "RecordSubCategories",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "RecordSubCategories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "RecordSubCategories",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "RecordItemUserDocuments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "RecordItemUserDocuments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "RecordItemUserDocuments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "RecordItemUserDocuments",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "RecordItemUserData",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "RecordItemUserData",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "RecordItemUserData",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "RecordItemUserData",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "RecordItemUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "RecordItemUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "RecordItemUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "RecordItemUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "RecordItemElements",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "RecordItemElements",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "RecordItemElements",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "RecordItemElements",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "RecordItems",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "RecordItems",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "RecordItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "RecordItems",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "RecordElementLookupTables",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "RecordElementLookupTables",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "RecordElementLookupTables",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "RecordElementLookupTables",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "RecordElementDataTypes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "RecordElementDataTypes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "RecordElementDataTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "RecordElementDataTypes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "RecordElements",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "RecordElements",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "RecordElements",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "RecordElements",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "RecordCategories",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "RecordCategories",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "RecordCategories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "RecordCategories",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "MessageBoardTopics",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "MessageBoardTopics",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "MessageBoardTopics",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "MessageBoardTopics",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "MessageBoardItemAttendees",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "MessageBoardItemAttendees",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "MessageBoardItemAttendees",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "MessageBoardItemAttendees",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "MessageBoardItems",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "MessageBoardItems",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "MessageBoardItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "MessageBoardItems",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                schema: "dbo",
                table: "ContactMethods",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                schema: "dbo",
                table: "ContactMethods",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                schema: "dbo",
                table: "ContactMethods",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                schema: "dbo",
                table: "ContactMethods",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "EventNotifications",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "EventNotifications",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "EventNotifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "EventNotifications",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "EventAttendeeNotifications",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "EventAttendeeNotifications",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "EventAttendeeNotifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "EventAttendeeNotifications",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "EventAttendees",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "EventAttendees",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "EventAttendees",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "EventAttendees",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Events",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "Events",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "AttendeeGroupUser",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "AttendeeGroupUser",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "AttendeeGroupUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "AttendeeGroupUser",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "AttendeeGroup",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "AttendeeGroup",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "AttendeeGroup",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "AttendeeGroup",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "UserActivityDocuments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "UserActivityDocuments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "UserActivityDocuments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "UserActivityDocuments",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "UserActivityCredits",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "UserActivityCredits",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "UserActivityCredits",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "UserActivityCredits",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "UserActivities",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "UserActivities",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "UserActivities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "UserActivities",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "OrganizationAccreditations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "OrganizationAccreditations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "OrganizationAccreditations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "OrganizationAccreditations",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Organizations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "Organizations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Organizations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Organizations",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "DepartmentUserGroupUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "DepartmentUserGroupUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "DepartmentUserGroupUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "DepartmentUserGroupUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "DepartmentUserGroupActivityGroups",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "DepartmentUserGroupActivityGroups",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "DepartmentUserGroupActivityGroups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "DepartmentUserGroupActivityGroups",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "DepartmentUserGroups",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "DepartmentUserGroups",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "DepartmentUserGroups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "DepartmentUserGroups",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "DepartmentUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "DepartmentUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "DepartmentUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "DepartmentUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Departments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "Departments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Departments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Departments",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "CreditTypeCategories",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "CreditTypeCategories",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "CreditTypeCategories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "CreditTypeCategories",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "CreditTypes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "CreditTypes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "CreditTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "CreditTypes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "ActivityOrganizers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "ActivityOrganizers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "ActivityOrganizers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "ActivityOrganizers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "ActivityGroupOrganizers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "ActivityGroupOrganizers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "ActivityGroupOrganizers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "ActivityGroupOrganizers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "ActivityGroups",
                maxLength: 255,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "ActivityGroups",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "ActivityGroups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "ActivityGroups",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "ActivityDocuments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "ActivityDocuments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "ActivityDocuments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "ActivityDocuments",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "ActivityCredits",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "ActivityCredits",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "ActivityCredits",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "ActivityCredits",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Activities",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "Activities",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Activities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Activities",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "AccreditationDocuments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "AccreditationDocuments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "AccreditationDocuments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "AccreditationDocuments",
                nullable: true);
        }
    }
}
