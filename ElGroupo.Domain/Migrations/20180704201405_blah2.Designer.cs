﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ElGroupo.Domain.Data;
using ElGroupo.Domain.Enums;

namespace ElGroupo.Domain.Migrations
{
    [DbContext(typeof(ElGroupoDbContext))]
    [Migration("20180704201405_blah2")]
    partial class blah2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("Relational:Sequence:dbo.IdGenerator", "'IdGenerator', 'dbo', '1', '1', '', '', 'Int32', 'False'")
                .HasAnnotation("SqlServer:HiLoSequenceName", "IdGenerator")
                .HasAnnotation("SqlServer:HiLoSequenceSchema", "dbo")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

            modelBuilder.Entity("ElGroupo.Domain.Activities.AccreditationDocument", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentType");

                    b.Property<string>("Description")
                        .HasColumnName("Description")
                        .HasMaxLength(255);

                    b.Property<string>("FileName")
                        .HasColumnName("FileName")
                        .HasMaxLength(255);

                    b.Property<byte[]>("ImageData");

                    b.HasKey("Id");

                    b.ToTable("AccreditationDocuments");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.Activity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ActivityGroupId");

                    b.Property<string>("Description");

                    b.Property<DateTime?>("EndDate");

                    b.Property<bool>("IsPublic");

                    b.Property<string>("Location");

                    b.Property<string>("Notes");

                    b.Property<DateTime?>("StartDate");

                    b.HasKey("Id");

                    b.HasIndex("ActivityGroupId");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.ActivityCredit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ActivityId");

                    b.Property<double>("CreditHours");

                    b.Property<long>("CreditTypeCategoryId");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("CreditTypeCategoryId");

                    b.ToTable("ActivityCredits");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.ActivityDocument", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ActivityId");

                    b.Property<string>("ContentType");

                    b.Property<string>("Description");

                    b.Property<string>("FileName");

                    b.Property<byte[]>("ImageData");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.ToTable("ActivityDocuments");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.ActivityGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("DepartmentId");

                    b.Property<string>("Name");

                    b.Property<long?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("UserId");

                    b.ToTable("ActivityGroups");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.ActivityGroupOrganizer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ActivityGroupId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ActivityGroupId");

                    b.HasIndex("UserId");

                    b.ToTable("ActivityGroupOrganizers");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.ActivityOrganizer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ActivityId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("UserId");

                    b.ToTable("ActivityOrganizers");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.CreditType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("CreditTypes");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.CreditTypeCategory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<long>("CreditTypeId");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("CreditTypeId");

                    b.ToTable("CreditTypeCategories");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.Department", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<long>("OrganizationId");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.DepartmentUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("DepartmentId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("UserId");

                    b.ToTable("DepartmentUsers");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.DepartmentUserGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("DepartmentId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.ToTable("DepartmentUserGroups");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.DepartmentUserGroupActivityGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ActivityGroupId");

                    b.Property<long>("UserGroupId");

                    b.HasKey("Id");

                    b.HasIndex("ActivityGroupId");

                    b.HasIndex("UserGroupId");

                    b.ToTable("DepartmentUserGroupActivityGroups");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.DepartmentUserGroupUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsOwner");

                    b.Property<long>("UserGroupId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserGroupId");

                    b.HasIndex("UserId");

                    b.ToTable("DepartmentUserGroupUsers");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.Organization", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address1");

                    b.Property<string>("Address2");

                    b.Property<string>("Address3");

                    b.Property<string>("City");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("State");

                    b.Property<string>("Zip");

                    b.HasKey("Id");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.OrganizationAccreditation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorityId");

                    b.Property<long>("CreditTypeId");

                    b.Property<long>("OrganizationId");

                    b.HasKey("Id");

                    b.HasIndex("CreditTypeId");

                    b.HasIndex("OrganizationId");

                    b.ToTable("OrganizationAccreditations");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.UserActivity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ActivityId");

                    b.Property<long>("AttendanceTypeId");

                    b.Property<bool>("IsPresenting");

                    b.Property<string>("Notes");

                    b.Property<string>("PresentationName");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("UserId");

                    b.ToTable("UserActivities");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.UserActivityCredit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("CreditHours")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0.0);

                    b.Property<long>("CreditTypeCategoryId");

                    b.Property<long>("UserActivityId");

                    b.HasKey("Id");

                    b.HasIndex("CreditTypeCategoryId");

                    b.HasIndex("UserActivityId");

                    b.ToTable("UserActivityCredits");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.UserActivityDocument", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("DocumentId");

                    b.Property<long>("UserActivityId");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.HasIndex("UserActivityId");

                    b.ToTable("UserActivityDocuments");
                });

            modelBuilder.Entity("ElGroupo.Domain.AttendeeGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<long?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AttendeeGroup");
                });

            modelBuilder.Entity("ElGroupo.Domain.AttendeeGroupUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("AttendeeGroupId");

                    b.Property<long?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("AttendeeGroupId");

                    b.HasIndex("UserId");

                    b.ToTable("AttendeeGroupUser");
                });

            modelBuilder.Entity("ElGroupo.Domain.Event", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address1")
                        .HasMaxLength(100);

                    b.Property<string>("Address2")
                        .HasMaxLength(100);

                    b.Property<double>("CheckInLocationTolerance");

                    b.Property<int>("CheckInTimeTolerance");

                    b.Property<string>("City")
                        .HasMaxLength(100);

                    b.Property<double>("CoordinateX");

                    b.Property<double>("CoordinateY");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndTime");

                    b.Property<string>("GooglePlaceId")
                        .HasMaxLength(50);

                    b.Property<long?>("GroupId");

                    b.Property<string>("LocationName");

                    b.Property<string>("Name");

                    b.Property<long?>("RecurrenceId");

                    b.Property<DateTime>("StartTime");

                    b.Property<string>("State")
                        .HasMaxLength(10);

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("VerificationCode")
                        .HasMaxLength(10);

                    b.Property<int>("VerificationMethod");

                    b.Property<string>("Zip")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("RecurrenceId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("ElGroupo.Domain.EventAttendee", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool?>("Active")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValue", true);

                    b.Property<bool?>("AllowEventUpdates");

                    b.Property<double?>("CheckInCoordinateX");

                    b.Property<double?>("CheckInCoordinateY");

                    b.Property<DateTime?>("CheckInTime");

                    b.Property<bool>("CheckedIn");

                    b.Property<long>("EventId");

                    b.Property<bool>("IsOrganizer");

                    b.Property<DateTime?>("ResponseDate");

                    b.Property<int>("ResponseStatus");

                    b.Property<string>("ResponseText");

                    b.Property<bool?>("ShowRSVPReminder");

                    b.Property<long>("UserId");

                    b.Property<bool>("Viewed");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("UserId");

                    b.ToTable("EventAttendees");
                });

            modelBuilder.Entity("ElGroupo.Domain.EventAttendeeNotification", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AttendeeId");

                    b.Property<long>("NotificationId");

                    b.Property<bool>("Viewed");

                    b.HasKey("Id");

                    b.HasIndex("AttendeeId");

                    b.HasIndex("NotificationId");

                    b.ToTable("EventAttendeeNotifications");
                });

            modelBuilder.Entity("ElGroupo.Domain.EventNotification", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("EventId");

                    b.Property<int>("Importance");

                    b.Property<string>("MessageText");

                    b.Property<long>("PostedById");

                    b.Property<DateTime>("PostedDate");

                    b.Property<string>("Subject");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("PostedById");

                    b.ToTable("EventNotifications");
                });

            modelBuilder.Entity("ElGroupo.Domain.Lookups.ContactMethod", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("ContactMethods","dbo");
                });

            modelBuilder.Entity("ElGroupo.Domain.MessageBoardItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MessageText")
                        .IsRequired();

                    b.Property<long>("PostedById");

                    b.Property<DateTime>("PostedDate");

                    b.Property<long>("TopicId");

                    b.HasKey("Id");

                    b.HasIndex("PostedById");

                    b.HasIndex("TopicId");

                    b.ToTable("MessageBoardItems");
                });

            modelBuilder.Entity("ElGroupo.Domain.MessageBoardItemAttendee", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AttendeeId");

                    b.Property<long>("MessageBoardItemId");

                    b.Property<bool>("Viewed");

                    b.HasKey("Id");

                    b.HasIndex("AttendeeId");

                    b.HasIndex("MessageBoardItemId");

                    b.ToTable("MessageBoardItemAttendees");
                });

            modelBuilder.Entity("ElGroupo.Domain.MessageBoardTopic", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("EventId");

                    b.Property<long>("StartedById");

                    b.Property<DateTime>("StartedDate");

                    b.Property<string>("Subject");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("StartedById");

                    b.ToTable("MessageBoardTopics");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordCategory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ItemDescriptionColumnHeader");

                    b.Property<string>("ItemValueColumnHeader");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("RecordCategories");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordDefaultElement", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("CategoryId");

                    b.Property<long>("ElementId");

                    b.Property<long?>("SubCategoryId");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ElementId");

                    b.HasIndex("SubCategoryId");

                    b.ToTable("RecordDefaultElements");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordElement", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("DataTypeId");

                    b.Property<string>("DisplayName");

                    b.Property<long>("InputTypeId");

                    b.Property<long?>("LookupTableFieldTypeId");

                    b.Property<long?>("LookupTableId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("DataTypeId");

                    b.HasIndex("InputTypeId");

                    b.HasIndex("LookupTableFieldTypeId");

                    b.HasIndex("LookupTableId");

                    b.ToTable("RecordElements");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordElementDataType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("RecordElementDataTypes");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordElementDataTypeInputType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("DataTypeId");

                    b.Property<long>("InputTypeId");

                    b.HasKey("Id");

                    b.HasIndex("DataTypeId");

                    b.HasIndex("InputTypeId");

                    b.ToTable("RecordElementDataTypeInputTypes");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordElementInputType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("RecordElementInputTypes");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordElementLookupTable", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("TableName");

                    b.HasKey("Id");

                    b.ToTable("RecordElementLookupTables");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordElementLookupTableFieldType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("RecordElementLookupTableFieldTypes");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("CategoryId");

                    b.Property<string>("Name");

                    b.Property<long?>("SubCategoryId");

                    b.Property<long?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("SubCategoryId");

                    b.HasIndex("UserId");

                    b.ToTable("RecordItems");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordItemElement", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ElementId");

                    b.Property<long>("ItemId");

                    b.Property<bool>("PrimaryDisplay");

                    b.HasKey("Id");

                    b.HasIndex("ElementId");

                    b.HasIndex("ItemId");

                    b.ToTable("RecordItemElements");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordItemUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ItemId");

                    b.Property<long>("UserId");

                    b.Property<bool>("Visible")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("UserId");

                    b.ToTable("RecordItemUsers");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordItemUserData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ElementId");

                    b.Property<long>("ItemUserId");

                    b.Property<string>("Value")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("ElementId");

                    b.HasIndex("ItemUserId");

                    b.ToTable("RecordItemUserData");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordItemUserDocument", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentType");

                    b.Property<string>("Description");

                    b.Property<string>("FileName");

                    b.Property<byte[]>("ImageData");

                    b.Property<long>("ItemUserId");

                    b.HasKey("Id");

                    b.HasIndex("ItemUserId");

                    b.ToTable("RecordItemUserDocuments");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordSubCategory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ItemDescriptionColumnHeader");

                    b.Property<string>("ItemValueColumnHeader");

                    b.Property<string>("Name");

                    b.Property<long>("ParentCategoryId");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("RecordSubCategories");
                });

            modelBuilder.Entity("ElGroupo.Domain.RecurringEvent", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DaysInMonth");

                    b.Property<int>("Pattern");

                    b.Property<int>("RecurrenceDays");

                    b.Property<int>("RecurrenceInterval");

                    b.Property<int>("RecurrenceLimit");

                    b.HasKey("Id");

                    b.ToTable("RecurringEvent");
                });

            modelBuilder.Entity("ElGroupo.Domain.UnregisteredEventAttendee", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<long>("EventId");

                    b.Property<string>("Name");

                    b.Property<Guid>("RegisterToken");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.ToTable("UnregisteredEventAttendees","dbo");
                });

            modelBuilder.Entity("ElGroupo.Domain.UnregisteredUserConnection", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .HasColumnName("Email");

                    b.Property<string>("Name")
                        .HasColumnName("Name");

                    b.Property<string>("Phone1Type")
                        .HasColumnName("Phone1Type");

                    b.Property<string>("Phone1Value")
                        .HasColumnName("Phone1Value");

                    b.Property<string>("Phone2Type")
                        .HasColumnName("Phone2Type");

                    b.Property<string>("Phone2Value")
                        .HasColumnName("Phone2Value");

                    b.Property<long?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UnregisteredUserConnections");
                });

            modelBuilder.Entity("ElGroupo.Domain.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasColumnName("EmailAddress")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .HasColumnName("FirstName")
                        .HasMaxLength(100);

                    b.Property<string>("LastName")
                        .HasColumnName("LastName")
                        .HasMaxLength(100);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<long?>("PhotoId");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Specialty")
                        .HasColumnName("Specialty")
                        .HasMaxLength(30);

                    b.Property<string>("TimeZoneId")
                        .HasColumnName("TimeZoneId")
                        .HasMaxLength(100);

                    b.Property<string>("Title")
                        .HasColumnName("Title")
                        .HasMaxLength(10);

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<string>("ZipCode");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.HasIndex("PhotoId");

                    b.ToTable("User","dbo");
                });

            modelBuilder.Entity("ElGroupo.Domain.UserConnection", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ConnectedUserId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ConnectedUserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserConnection");
                });

            modelBuilder.Entity("ElGroupo.Domain.UserContactMethod", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ContactMethodId");

                    b.Property<long>("UserId");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ContactMethodId");

                    b.HasIndex("UserId");

                    b.ToTable("UserContactMethods");
                });

            modelBuilder.Entity("ElGroupo.Domain.UserPhoto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentType");

                    b.Property<byte[]>("ImageData");

                    b.HasKey("Id");

                    b.ToTable("UserPhoto");
                });

            modelBuilder.Entity("ElGroupo.Domain.UserValidationToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("Token");

                    b.Property<int>("TokenType");

                    b.Property<long?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<long>", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<long>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<long>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<long>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<long>", b =>
                {
                    b.Property<long>("UserId");

                    b.Property<long>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<long>", b =>
                {
                    b.Property<long>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.Activity", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.ActivityGroup", "ActivityGroup")
                        .WithMany("Activities")
                        .HasForeignKey("ActivityGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.ActivityCredit", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.Activity", "Activity")
                        .WithMany("Credits")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.Activities.CreditTypeCategory", "CreditTypeCategory")
                        .WithMany()
                        .HasForeignKey("CreditTypeCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.ActivityDocument", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.Activity", "Activity")
                        .WithMany("Documents")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.ActivityGroup", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.Department", "Department")
                        .WithMany("ActivityGroups")
                        .HasForeignKey("DepartmentId");

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("ActivityGroups")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.ActivityGroupOrganizer", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.ActivityGroup", "ActivityGroup")
                        .WithMany("Organizers")
                        .HasForeignKey("ActivityGroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("OrganizedActivityGroups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.ActivityOrganizer", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.Activity", "Activity")
                        .WithMany("Organizers")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("OrganizedActivities")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.CreditTypeCategory", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.CreditType", "CreditType")
                        .WithMany("Categories")
                        .HasForeignKey("CreditTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.Department", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.Organization", "Organization")
                        .WithMany("Departments")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.DepartmentUser", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.Department", "Department")
                        .WithMany("Users")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("Departments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.DepartmentUserGroup", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.Department", "Department")
                        .WithMany("UserGroups")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.DepartmentUserGroupActivityGroup", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.ActivityGroup", "ActivityGroup")
                        .WithMany("UserGroups")
                        .HasForeignKey("ActivityGroupId");

                    b.HasOne("ElGroupo.Domain.Activities.DepartmentUserGroup", "UserGroup")
                        .WithMany("ActivityGroups")
                        .HasForeignKey("UserGroupId");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.DepartmentUserGroupUser", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.DepartmentUserGroup", "UserGroup")
                        .WithMany("Users")
                        .HasForeignKey("UserGroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.Activities.DepartmentUser", "User")
                        .WithMany("UserGroupUsers")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.OrganizationAccreditation", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.CreditType", "CreditType")
                        .WithMany()
                        .HasForeignKey("CreditTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.Activities.Organization", "Organization")
                        .WithMany("Accreditations")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.UserActivity", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.Activity", "Activity")
                        .WithMany("Users")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("Activities")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.UserActivityCredit", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.CreditTypeCategory", "CreditTypeCategory")
                        .WithMany()
                        .HasForeignKey("CreditTypeCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.Activities.UserActivity", "UserActivity")
                        .WithMany("Credits")
                        .HasForeignKey("UserActivityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Activities.UserActivityDocument", b =>
                {
                    b.HasOne("ElGroupo.Domain.Activities.AccreditationDocument", "Document")
                        .WithMany("Activities")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.Activities.UserActivity", "UserActivity")
                        .WithMany("Documents")
                        .HasForeignKey("UserActivityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.AttendeeGroup", b =>
                {
                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("AttendeeGroups")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ElGroupo.Domain.AttendeeGroupUser", b =>
                {
                    b.HasOne("ElGroupo.Domain.AttendeeGroup", "AttendeeGroup")
                        .WithMany("Attendees")
                        .HasForeignKey("AttendeeGroupId");

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ElGroupo.Domain.Event", b =>
                {
                    b.HasOne("ElGroupo.Domain.RecurringEvent", "Recurrence")
                        .WithMany("Events")
                        .HasForeignKey("RecurrenceId");
                });

            modelBuilder.Entity("ElGroupo.Domain.EventAttendee", b =>
                {
                    b.HasOne("ElGroupo.Domain.Event", "Event")
                        .WithMany("Attendees")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("AttendedEvents")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.EventAttendeeNotification", b =>
                {
                    b.HasOne("ElGroupo.Domain.EventAttendee", "Attendee")
                        .WithMany("Notifications")
                        .HasForeignKey("AttendeeId");

                    b.HasOne("ElGroupo.Domain.EventNotification", "Notification")
                        .WithMany()
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.EventNotification", b =>
                {
                    b.HasOne("ElGroupo.Domain.Event", "Event")
                        .WithMany("Notifications")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.EventAttendee", "PostedBy")
                        .WithMany("PostedNotifications")
                        .HasForeignKey("PostedById");
                });

            modelBuilder.Entity("ElGroupo.Domain.MessageBoardItem", b =>
                {
                    b.HasOne("ElGroupo.Domain.User", "PostedBy")
                        .WithMany()
                        .HasForeignKey("PostedById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.MessageBoardTopic", "Topic")
                        .WithMany("Messages")
                        .HasForeignKey("TopicId");
                });

            modelBuilder.Entity("ElGroupo.Domain.MessageBoardItemAttendee", b =>
                {
                    b.HasOne("ElGroupo.Domain.EventAttendee", "Attendee")
                        .WithMany("MessageBoardItems")
                        .HasForeignKey("AttendeeId");

                    b.HasOne("ElGroupo.Domain.MessageBoardItem", "MessageBoardItem")
                        .WithMany("Attendees")
                        .HasForeignKey("MessageBoardItemId");
                });

            modelBuilder.Entity("ElGroupo.Domain.MessageBoardTopic", b =>
                {
                    b.HasOne("ElGroupo.Domain.Event", "Event")
                        .WithMany("MessageBoardTopics")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.User", "StartedBy")
                        .WithMany()
                        .HasForeignKey("StartedById")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordDefaultElement", b =>
                {
                    b.HasOne("ElGroupo.Domain.Records.RecordCategory", "Category")
                        .WithMany("DefaultElements")
                        .HasForeignKey("CategoryId");

                    b.HasOne("ElGroupo.Domain.Records.RecordElement", "Element")
                        .WithMany("DefaultElements")
                        .HasForeignKey("ElementId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.Records.RecordSubCategory", "SubCategory")
                        .WithMany("DefaultElements")
                        .HasForeignKey("SubCategoryId");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordElement", b =>
                {
                    b.HasOne("ElGroupo.Domain.Records.RecordElementDataType", "DataType")
                        .WithMany("RecordElements")
                        .HasForeignKey("DataTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.Records.RecordElementInputType", "InputType")
                        .WithMany("RecordElements")
                        .HasForeignKey("InputTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.Records.RecordElementLookupTableFieldType", "LookupTableFieldType")
                        .WithMany("RecordElements")
                        .HasForeignKey("LookupTableFieldTypeId");

                    b.HasOne("ElGroupo.Domain.Records.RecordElementLookupTable", "LookupTable")
                        .WithMany("RecordElements")
                        .HasForeignKey("LookupTableId");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordElementDataTypeInputType", b =>
                {
                    b.HasOne("ElGroupo.Domain.Records.RecordElementDataType", "DataType")
                        .WithMany("InputTypes")
                        .HasForeignKey("DataTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.Records.RecordElementInputType", "InputType")
                        .WithMany("DataTypes")
                        .HasForeignKey("InputTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordItem", b =>
                {
                    b.HasOne("ElGroupo.Domain.Records.RecordCategory", "Category")
                        .WithMany("Items")
                        .HasForeignKey("CategoryId");

                    b.HasOne("ElGroupo.Domain.Records.RecordSubCategory", "SubCategory")
                        .WithMany("Items")
                        .HasForeignKey("SubCategoryId");

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("CustomRecordItems")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordItemElement", b =>
                {
                    b.HasOne("ElGroupo.Domain.Records.RecordElement", "Element")
                        .WithMany("Items")
                        .HasForeignKey("ElementId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.Records.RecordItem", "Item")
                        .WithMany("Elements")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordItemUser", b =>
                {
                    b.HasOne("ElGroupo.Domain.Records.RecordItem", "Item")
                        .WithMany("Users")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("RecordItems")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordItemUserData", b =>
                {
                    b.HasOne("ElGroupo.Domain.Records.RecordItemElement", "Element")
                        .WithMany("UserData")
                        .HasForeignKey("ElementId");

                    b.HasOne("ElGroupo.Domain.Records.RecordItemUser", "ItemUser")
                        .WithMany("UserData")
                        .HasForeignKey("ItemUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordItemUserDocument", b =>
                {
                    b.HasOne("ElGroupo.Domain.Records.RecordItemUser", "ItemUser")
                        .WithMany("Documents")
                        .HasForeignKey("ItemUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Records.RecordSubCategory", b =>
                {
                    b.HasOne("ElGroupo.Domain.Records.RecordCategory", "ParentCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("ParentCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.UnregisteredEventAttendee", b =>
                {
                    b.HasOne("ElGroupo.Domain.Event", "Event")
                        .WithMany("UnregisteredAttendees")
                        .HasForeignKey("EventId");
                });

            modelBuilder.Entity("ElGroupo.Domain.UnregisteredUserConnection", b =>
                {
                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("UnregisteredConnections")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ElGroupo.Domain.User", b =>
                {
                    b.HasOne("ElGroupo.Domain.UserPhoto", "Photo")
                        .WithMany()
                        .HasForeignKey("PhotoId");
                });

            modelBuilder.Entity("ElGroupo.Domain.UserConnection", b =>
                {
                    b.HasOne("ElGroupo.Domain.User", "ConnectedUser")
                        .WithMany()
                        .HasForeignKey("ConnectedUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("ConnectedUsers")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ElGroupo.Domain.UserContactMethod", b =>
                {
                    b.HasOne("ElGroupo.Domain.Lookups.ContactMethod", "ContactMethod")
                        .WithMany()
                        .HasForeignKey("ContactMethodId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("ContactMethods")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.UserValidationToken", b =>
                {
                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<long>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<long>")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<long>", b =>
                {
                    b.HasOne("ElGroupo.Domain.User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<long>", b =>
                {
                    b.HasOne("ElGroupo.Domain.User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<long>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<long>")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
