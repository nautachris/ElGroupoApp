using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ElGroupo.Domain.Data;
using ElGroupo.Domain.Enums;

namespace ElGroupo.Domain.Migrations
{
    [DbContext(typeof(ElGroupoDbContext))]
    [Migration("20170817211503_unregisteredeventattendee")]
    partial class unregisteredeventattendee
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("Relational:Sequence:dbo.IdGenerator", "'IdGenerator', 'dbo', '1', '1', '', '', 'Int32', 'False'")
                .HasAnnotation("SqlServer:HiLoSequenceName", "IdGenerator")
                .HasAnnotation("SqlServer:HiLoSequenceSchema", "dbo")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

            modelBuilder.Entity("ElGroupo.Domain.ContactGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("Name");

                    b.Property<string>("UserCreated");

                    b.Property<int?>("UserId");

                    b.Property<string>("UserUpdated");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ContactGroups");
                });

            modelBuilder.Entity("ElGroupo.Domain.ContactGroupUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<long>("GroupId");

                    b.Property<string>("UserCreated");

                    b.Property<int>("UserId");

                    b.Property<string>("UserUpdated");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("UserId");

                    b.ToTable("ContactGroupUser","dbo");
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

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndTime");

                    b.Property<string>("GooglePlaceId")
                        .HasMaxLength(50);

                    b.Property<long?>("GroupId");

                    b.Property<string>("LocationName");

                    b.Property<string>("Name");

                    b.Property<bool>("SavedAsDraft");

                    b.Property<DateTime>("StartTime");

                    b.Property<string>("State")
                        .HasMaxLength(10);

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserUpdated");

                    b.Property<string>("Zip")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("ElGroupo.Domain.EventAttendee", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool?>("AllowEventUpdates");

                    b.Property<double?>("CheckInCoordinateX");

                    b.Property<double?>("CheckInCoordinateY");

                    b.Property<DateTime?>("CheckInTime");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<long>("EventId");

                    b.Property<DateTime?>("ResponseDate");

                    b.Property<int>("ResponseStatus");

                    b.Property<string>("ResponseText");

                    b.Property<string>("UserCreated");

                    b.Property<int>("UserId");

                    b.Property<string>("UserUpdated");

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

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<long>("NotificationId");

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserUpdated");

                    b.Property<bool>("Viewed");

                    b.HasKey("Id");

                    b.HasIndex("AttendeeId");

                    b.HasIndex("NotificationId");

                    b.ToTable("EventAttendeeNotifications");
                });

            modelBuilder.Entity("ElGroupo.Domain.EventGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("OwnerId");

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserUpdated");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("EventGroups");
                });

            modelBuilder.Entity("ElGroupo.Domain.EventNotification", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<long>("EventId");

                    b.Property<long>("EventOrganizerId");

                    b.Property<int>("Importance");

                    b.Property<string>("MessageText");

                    b.Property<DateTime>("PostedDate");

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserUpdated");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("EventOrganizerId");

                    b.ToTable("EventNotifications");
                });

            modelBuilder.Entity("ElGroupo.Domain.EventOrganizer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<long>("EventId");

                    b.Property<bool>("Owner");

                    b.Property<string>("UserCreated");

                    b.Property<int>("UserId");

                    b.Property<string>("UserUpdated");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("UserId");

                    b.ToTable("EventOrganizers");
                });

            modelBuilder.Entity("ElGroupo.Domain.Lookups.ContactType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserUpdated");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("ContactTypes","dbo");
                });

            modelBuilder.Entity("ElGroupo.Domain.MessageBoardItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<long>("EventId");

                    b.Property<string>("MessageText")
                        .IsRequired();

                    b.Property<DateTime>("PostedDate");

                    b.Property<string>("UserCreated");

                    b.Property<int>("UserId");

                    b.Property<string>("UserUpdated");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("UserId");

                    b.ToTable("MessageBoardItems");
                });

            modelBuilder.Entity("ElGroupo.Domain.MessageBoardItemAttendee", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AttendeeId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<long>("MessageBoardItemId");

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserUpdated");

                    b.Property<bool>("Viewed");

                    b.HasKey("Id");

                    b.HasIndex("AttendeeId");

                    b.HasIndex("MessageBoardItemId");

                    b.ToTable("MessageBoardItemAttendees");
                });

            modelBuilder.Entity("ElGroupo.Domain.UnregisteredEventAttendee", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("Email");

                    b.Property<long>("EventId");

                    b.Property<string>("Name");

                    b.Property<Guid>("RegisterToken");

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserUpdated");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.ToTable("UnregisteredEventAttendees","dbo");
                });

            modelBuilder.Entity("ElGroupo.Domain.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasColumnName("EmailAddress")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

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

            modelBuilder.Entity("ElGroupo.Domain.UserContact", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ContactTypeId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("UserCreated");

                    b.Property<int>("UserId");

                    b.Property<string>("UserUpdated");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ContactTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("UserContacts");
                });

            modelBuilder.Entity("ElGroupo.Domain.UserPhoto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentType");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<byte[]>("ImageData");

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserUpdated");

                    b.HasKey("Id");

                    b.ToTable("UserPhoto");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<int>", b =>
                {
                    b.Property<int>("Id")
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

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ElGroupo.Domain.ContactGroup", b =>
                {
                    b.HasOne("ElGroupo.Domain.User")
                        .WithMany("ContactGroups")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ElGroupo.Domain.ContactGroupUser", b =>
                {
                    b.HasOne("ElGroupo.Domain.ContactGroup", "Group")
                        .WithMany("Users")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.Event", b =>
                {
                    b.HasOne("ElGroupo.Domain.EventGroup", "Group")
                        .WithMany("Events")
                        .HasForeignKey("GroupId");
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
                        .HasForeignKey("AttendeeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.EventNotification", "Notification")
                        .WithMany()
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.EventGroup", b =>
                {
                    b.HasOne("ElGroupo.Domain.User", "Owner")
                        .WithMany("EventGroups")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.EventNotification", b =>
                {
                    b.HasOne("ElGroupo.Domain.Event", "Event")
                        .WithMany("Notifications")
                        .HasForeignKey("EventId");

                    b.HasOne("ElGroupo.Domain.EventOrganizer", "PostedBy")
                        .WithMany()
                        .HasForeignKey("EventOrganizerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.EventOrganizer", b =>
                {
                    b.HasOne("ElGroupo.Domain.Event", "Event")
                        .WithMany("Organizers")
                        .HasForeignKey("EventId");

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("OrganizedEvents")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ElGroupo.Domain.MessageBoardItem", b =>
                {
                    b.HasOne("ElGroupo.Domain.Event", "Event")
                        .WithMany("MessageBoardItems")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElGroupo.Domain.MessageBoardItemAttendee", b =>
                {
                    b.HasOne("ElGroupo.Domain.EventAttendee", "Attendee")
                        .WithMany("MessageBoardItems")
                        .HasForeignKey("AttendeeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.MessageBoardItem", "MessageBoardItem")
                        .WithMany("Attendees")
                        .HasForeignKey("MessageBoardItemId");
                });

            modelBuilder.Entity("ElGroupo.Domain.UnregisteredEventAttendee", b =>
                {
                    b.HasOne("ElGroupo.Domain.Event", "Event")
                        .WithMany("UnregisteredAttendees")
                        .HasForeignKey("EventId");
                });

            modelBuilder.Entity("ElGroupo.Domain.User", b =>
                {
                    b.HasOne("ElGroupo.Domain.UserPhoto", "Photo")
                        .WithMany()
                        .HasForeignKey("PhotoId");
                });

            modelBuilder.Entity("ElGroupo.Domain.UserContact", b =>
                {
                    b.HasOne("ElGroupo.Domain.Lookups.ContactType", "ContactType")
                        .WithMany()
                        .HasForeignKey("ContactTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElGroupo.Domain.User", "User")
                        .WithMany("Contacts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<int>")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("ElGroupo.Domain.User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("ElGroupo.Domain.User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<int>")
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
