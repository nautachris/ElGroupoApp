using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ElGroupo.Domain.Data.Configurations;
using System.Drawing;
using System.Drawing.Imaging;
using ElGroupo.Domain.Enums;
using ElGroupo.Domain.Activities;
namespace ElGroupo.Domain.Data
{


    public class ElGroupoDbContext : IdentityDbContext<User, IdentityRole<long>, long>
    {

        public DbSet<AccreditationDocument> AccreditationDocuments { get; set; }
        public DbSet<Activity> Activities { get; set; }
        //public DbSet<ActivityAttendanceType> ActivityAttendanceTypes { get; set; }
        public DbSet<ActivityCredit> ActivityCredits { get; set; }
        public DbSet<ActivityGroup> ActivityGroups { get; set; }
        public DbSet<ActivityGroupOrganizer> ActivityGroupOrganizers { get; set; }
        public DbSet<ActivityOrganizer> ActivityOrganizers { get; set; }
        public DbSet<CreditType> CreditTypes { get; set; }
        public DbSet<CreditTypeCategory> CreditTypeCategories { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentUser> DepartmentUsers { get; set; }
        public DbSet<DepartmentUserGroup> DepartmentUserGroups { get; set; }
        public DbSet<DepartmentUserGroupActivityGroup> DepartmentUserGroupActivityGroups { get; set; }
        public DbSet<DepartmentUserGroupUser> DepartmentUserGroupUsers { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationAccreditation> OrganizationAccreditations { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<UserActivityCredit> UserActivityCredits { get; set; }
        public DbSet<UserActivityDocument> UserActivityDocuments { get; set; }
        public DbSet<Lookups.ContactMethod> ContactTypes { get; set; }

        public DbSet<UnregisteredEventAttendee> UnregisteredEventAttendees { get; set; }

        public DbSet<RecurringEvent> RecurringEvents { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventAttendee> EventAttendees { get; set; }
        public DbSet<AttendeeGroup> AttendeeGroups { get; set; }


        public DbSet<MessageBoardItem> MessageBoardItems { get; set; }
        public DbSet<MessageBoardTopic> MessageBoardTopics { get; set; }
        public DbSet<EventAttendeeNotification> EventAttendeeNotifications { get; set; }
        public DbSet<EventNotification> EventNotifications { get; set; }

        public DbSet<MessageBoardItemAttendee> MessageBoardItemAttendees { get; set; }
        public DbSet<UserContactMethod> UserContacts { get; set; }

        public DbSet<UnregisteredUserConnection> UnregisteredUserConnections { get; set; }

        public DbSet<UserPhoto> UserPhotos { get; set; }

        public DbSet<UserConnection> UserConnections { get; set; }
        public DbSet<UserValidationToken> UserValidationTokens { get; set; }


        public ElGroupoDbContext(DbContextOptions<ElGroupoDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.AddConfiguration<SequenceConfiguration>();
            builder.AddConfiguration<UserConfiguration>();
            builder.AddConfiguration<ContactMethodConfiguration>();
            builder.AddConfiguration<UnregisteredEventAttendeeConfiguration>();

            builder.Entity<UnregisteredUserConnection>().ToTable("UnregisteredUserConnections");
            builder.Entity<UnregisteredUserConnection>().HasKey(x => x.Id);
            builder.Entity<UnregisteredUserConnection>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<UnregisteredUserConnection>().Property(x => x.Email).HasColumnName("Email");
            builder.Entity<UnregisteredUserConnection>().Property(x => x.Name).HasColumnName("Name");
            builder.Entity<UnregisteredUserConnection>().Property(x => x.Phone1Type).HasColumnName("Phone1Type");
            builder.Entity<UnregisteredUserConnection>().Property(x => x.Phone1Value).HasColumnName("Phone1Value");
            builder.Entity<UnregisteredUserConnection>().Property(x => x.Phone2Type).HasColumnName("Phone2Type");
            builder.Entity<UnregisteredUserConnection>().Property(x => x.Phone2Value).HasColumnName("Phone2Value");


            builder.Entity<RecurringEvent>().ToTable("RecurringEvent");
            builder.Entity<RecurringEvent>().HasKey(x => x.Id);
            builder.Entity<RecurringEvent>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<RecurringEvent>().HasMany(x => x.Events).WithOne(x => x.Recurrence);


            builder.Entity<AttendeeGroup>().ToTable("AttendeeGroup");
            builder.Entity<AttendeeGroup>().HasKey(x => x.Id);
            builder.Entity<AttendeeGroup>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<AttendeeGroup>().HasOne(x => x.User).WithMany(x => x.AttendeeGroups);
            builder.Entity<AttendeeGroup>().HasMany(x => x.Attendees);

            builder.Entity<UserConnection>().ToTable("UserConnection");
            builder.Entity<UserConnection>().HasKey(x => x.Id);
            builder.Entity<UserConnection>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<UserConnection>().HasOne(x => x.User).WithMany(x => x.ConnectedUsers).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            builder.Entity<UserConnection>().HasOne(x => x.ConnectedUser);

            builder.Entity<UserValidationToken>().ToTable("UserTokens");
            builder.Entity<UserValidationToken>().HasKey(x => x.Id);
            builder.Entity<UserValidationToken>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<UserValidationToken>().HasOne(x => x.User);





            builder.Entity<UserContactMethod>().ToTable("UserContactMethods");
            builder.Entity<UserPhoto>().ToTable("UserPhoto");
            builder.Entity<Event>().Property(x => x.Status).HasDefaultValue(EventStatus.Draft);
            builder.Entity<Event>().HasMany(x => x.Attendees).WithOne(x => x.Event);
            builder.Entity<Event>().HasMany(x => x.UnregisteredAttendees).WithOne(x => x.Event);
            builder.Entity<Event>().HasMany(x => x.MessageBoardTopics).WithOne(x => x.Event);
            builder.Entity<Event>().HasMany(x => x.Notifications).WithOne(x => x.Event);

            builder.Entity<EventNotification>().HasOne(x => x.PostedBy).WithMany(x => x.PostedNotifications).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);

            builder.Entity<MessageBoardTopic>().ToTable("MessageBoardTopics");
            builder.Entity<MessageBoardTopic>().HasKey(x => x.Id);
            builder.Entity<MessageBoardTopic>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<MessageBoardTopic>().HasMany(x => x.Messages).WithOne(x => x.Topic).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            builder.Entity<MessageBoardItem>().HasMany(x => x.Attendees).WithOne(x => x.MessageBoardItem).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            builder.Entity<EventAttendee>().HasMany(x => x.MessageBoardItems).WithOne(x => x.Attendee).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);

            builder.Entity<EventAttendee>().Property(x => x.Active).ForSqlServerHasDefaultValue(true);
            builder.Entity<EventAttendee>().HasMany(x => x.Notifications).WithOne(x => x.Attendee).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);





            //builder.Entity<EventOrganizer>().HasOne(x => x.Event).WithMany(y => y.Organizers).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            //builder.Entity<EventOrganizer>().HasOne(x => x.User).WithMany(y => y.OrganizedEvents).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            builder.Entity<MessageBoardItemAttendee>().HasOne(x => x.MessageBoardItem).WithMany(x => x.Attendees);


            //accreditationdocument

            //accreditedorganization
            //organization
            //cmeactivity
            //activity
            //cmeactivitytype
            //organization
            //useractivity


            //this works if an activity would only have a sinlge document, but if multiple we need a junction table
            builder.Entity<AccreditationDocument>().ToTable("AccreditationDocuments");
            builder.Entity<AccreditationDocument>().HasKey(x => x.Id);
            builder.Entity<AccreditationDocument>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<AccreditationDocument>().Property(x => x.Description).HasColumnName("Description").HasMaxLength(255);
            builder.Entity<AccreditationDocument>().Property(x => x.FileName).HasColumnName("FileName").HasMaxLength(255);
            builder.Entity<AccreditationDocument>().HasMany(x => x.Activities).WithOne(x => x.Document).HasForeignKey(x => x.DocumentId);

            builder.Entity<Activity>().ToTable("Activities");
            builder.Entity<Activity>().HasKey(x => x.Id);
            builder.Entity<Activity>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Activity>().HasMany(x => x.Credits).WithOne(x => x.Activity).HasForeignKey(x => x.ActivityId);
            builder.Entity<Activity>().HasMany(x => x.Users).WithOne(x => x.Activity).HasForeignKey(x => x.ActivityId);
            builder.Entity<Activity>().HasOne(x => x.ActivityGroup).WithMany(x => x.Activities).HasForeignKey(x => x.ActivityGroupId).IsRequired();

            //builder.Entity<ActivityAttendanceType>().ToTable("ActivityAttendanceTypes");
            //builder.Entity<ActivityAttendanceType>().HasKey(x => x.Id);
            //builder.Entity<ActivityAttendanceType>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            //builder.Entity<ActivityAttendanceType>().Property(x => x.Description).HasMaxLength(255).IsRequired();

            builder.Entity<ActivityCredit>().ToTable("ActivityCredits");
            builder.Entity<ActivityCredit>().HasKey(x => x.Id);
            builder.Entity<ActivityCredit>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<ActivityCredit>().HasOne(x => x.CreditTypeCategory).WithMany().HasForeignKey(x => x.CreditTypeCategoryId);
            builder.Entity<ActivityCredit>().HasOne(x => x.Activity).WithMany(x => x.Credits).HasForeignKey(x => x.ActivityId);

            builder.Entity<ActivityGroup>().ToTable("ActivityGroups");
            builder.Entity<ActivityGroup>().HasKey(x => x.Id);
            builder.Entity<ActivityGroup>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<ActivityGroup>().HasOne(x => x.Department).WithMany(x => x.ActivityGroups).HasForeignKey(x => x.DepartmentId);
            builder.Entity<ActivityGroup>().HasOne(x => x.User).WithMany(x => x.ActivityGroups).HasForeignKey(x => x.UserId);
            builder.Entity<ActivityGroup>().Property(x => x.DateCreated).HasMaxLength(255).IsRequired();
            builder.Entity<ActivityGroup>().HasMany(x => x.Activities).WithOne(x => x.ActivityGroup).HasForeignKey(x => x.ActivityGroupId);
            builder.Entity<ActivityGroup>().HasMany(x => x.UserGroups).WithOne(x => x.ActivityGroup).HasForeignKey(x => x.ActivityGroupId);
            builder.Entity<ActivityGroup>().HasMany(x => x.Organizers).WithOne(x => x.ActivityGroup).HasForeignKey(x => x.ActivityGroupId);

            builder.Entity<ActivityGroupOrganizer>().ToTable("ActivityGroupOrganizers");
            builder.Entity<ActivityGroupOrganizer>().HasKey(x => x.Id);
            builder.Entity<ActivityGroupOrganizer>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<ActivityGroupOrganizer>().HasOne(x => x.ActivityGroup).WithMany(x => x.Organizers).HasForeignKey(x => x.ActivityGroupId);
            builder.Entity<ActivityGroupOrganizer>().HasOne(x => x.User).WithMany(x => x.OrganizedActivityGroups).HasForeignKey(x => x.UserId);

            builder.Entity<ActivityOrganizer>().ToTable("ActivityOrganizers");
            builder.Entity<ActivityOrganizer>().HasKey(x => x.Id);
            builder.Entity<ActivityOrganizer>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<ActivityOrganizer>().HasOne(x => x.Activity).WithMany(x => x.Organizers).HasForeignKey(x => x.ActivityId);
            builder.Entity<ActivityOrganizer>().HasOne(x => x.User).WithMany(x => x.OrganizedActivities).HasForeignKey(x => x.UserId);

            builder.Entity<CreditType>().ToTable("CreditTypes");
            builder.Entity<CreditType>().HasKey(x => x.Id);
            builder.Entity<CreditType>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<CreditType>().Property(x => x.Description).HasMaxLength(255).IsRequired();
            builder.Entity<CreditType>().HasMany(x => x.Categories).WithOne(x => x.CreditType).HasForeignKey(x => x.CreditTypeId);

            builder.Entity<CreditTypeCategory>().ToTable("CreditTypeCategories");
            builder.Entity<CreditTypeCategory>().HasKey(x => x.Id);
            builder.Entity<CreditTypeCategory>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<CreditTypeCategory>().Property(x => x.Description).HasMaxLength(255).IsRequired();
            builder.Entity<CreditTypeCategory>().Property(x => x.Active).HasDefaultValue(true);
            builder.Entity<CreditTypeCategory>().HasOne(x => x.CreditType).WithMany(x => x.Categories).HasForeignKey(x => x.CreditTypeId);

            builder.Entity<Department>().ToTable("Departments");
            builder.Entity<Department>().HasKey(x => x.Id);
            builder.Entity<Department>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Department>().Property(x => x.Name).IsRequired().HasMaxLength(255);
            builder.Entity<Department>().HasOne(x => x.Organization).WithMany(x => x.Departments).HasForeignKey(x => x.OrganizationId);
            builder.Entity<Department>().HasMany(x => x.Users).WithOne(x => x.Department).HasForeignKey(x => x.DepartmentId);
            builder.Entity<Department>().HasMany(x => x.UserGroups).WithOne(x => x.Department).HasForeignKey(x => x.DepartmentId);
            builder.Entity<Department>().HasMany(x => x.ActivityGroups).WithOne(x => x.Department).HasForeignKey(x => x.DepartmentId);

            builder.Entity<DepartmentUser>().ToTable("DepartmentUsers");
            builder.Entity<DepartmentUser>().HasKey(x => x.Id);
            builder.Entity<DepartmentUser>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<DepartmentUser>().HasOne(x => x.User).WithMany(x => x.Departments).HasForeignKey(x => x.UserId);
            builder.Entity<DepartmentUser>().HasOne(x => x.Department).WithMany(x => x.Users).HasForeignKey(x => x.DepartmentId);
            builder.Entity<DepartmentUser>().HasMany(x => x.UserGroupUsers).WithOne(x => x.User).HasForeignKey(x => x.UserId);

            builder.Entity<DepartmentUserGroup>().ToTable("DepartmentUserGroups");
            builder.Entity<DepartmentUserGroup>().HasKey(x => x.Id);
            builder.Entity<DepartmentUserGroup>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<DepartmentUserGroup>().Property(x => x.Name).HasMaxLength(255).IsRequired();
            builder.Entity<DepartmentUserGroup>().HasOne(x => x.Department).WithMany(x => x.UserGroups).HasForeignKey(x => x.DepartmentId);
            builder.Entity<DepartmentUserGroup>().HasMany(x => x.Users).WithOne(x => x.UserGroup).HasForeignKey(x => x.UserGroupId);
            builder.Entity<DepartmentUserGroup>().HasMany(x => x.ActivityGroups).WithOne(x => x.UserGroup).HasForeignKey(x => x.UserGroupId);

            builder.Entity<DepartmentUserGroupActivityGroup>().ToTable("DepartmentUserGroupActivityGroups");
            builder.Entity<DepartmentUserGroupActivityGroup>().HasKey(x => x.Id);
            builder.Entity<DepartmentUserGroupActivityGroup>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<DepartmentUserGroupActivityGroup>().HasOne(x => x.UserGroup).WithMany(x => x.ActivityGroups).HasForeignKey(x => x.UserGroupId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            builder.Entity<DepartmentUserGroupActivityGroup>().HasOne(x => x.ActivityGroup).WithMany(x => x.UserGroups).HasForeignKey(x => x.ActivityGroupId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);

            builder.Entity<DepartmentUserGroupUser>().ToTable("DepartmentUserGroupUsers");
            builder.Entity<DepartmentUserGroupUser>().HasKey(x => x.Id);
            builder.Entity<DepartmentUserGroupUser>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<DepartmentUserGroupUser>().Property(x => x.IsOwner).IsRequired();
            builder.Entity<DepartmentUserGroupUser>().HasOne(x => x.UserGroup).WithMany(x => x.Users).HasForeignKey(x => x.UserGroupId);
            builder.Entity<DepartmentUserGroupUser>().HasOne(x => x.User).WithMany(x => x.UserGroupUsers).HasForeignKey(x => x.UserId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);

            builder.Entity<Organization>().ToTable("Organizations");
            builder.Entity<Organization>().HasKey(x => x.Id);
            builder.Entity<Organization>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Organization>().Property(x => x.Name).IsRequired();
            builder.Entity<Organization>().HasMany(x => x.Accreditations).WithOne(x => x.Organization).HasForeignKey(x => x.OrganizationId);
            builder.Entity<Organization>().HasMany(x => x.Departments).WithOne(x => x.Organization).HasForeignKey(x => x.OrganizationId);

            builder.Entity<OrganizationAccreditation>().ToTable("OrganizationAccreditations");
            builder.Entity<OrganizationAccreditation>().HasKey(x => x.Id);
            builder.Entity<OrganizationAccreditation>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<OrganizationAccreditation>().HasOne(x => x.Organization).WithMany(x => x.Accreditations).HasForeignKey(x => x.OrganizationId);
            builder.Entity<OrganizationAccreditation>().HasOne(x => x.CreditType).WithMany().HasForeignKey(x => x.CreditTypeId);

            builder.Entity<UserActivity>().ToTable("UserActivities");
            builder.Entity<UserActivity>().HasKey(x => x.Id);
            builder.Entity<UserActivity>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            //builder.Entity<UserActivity>().HasOne(x => x.AttendanceType).WithMany().HasForeignKey(x => x.AttendanceTypeId);
            builder.Entity<UserActivity>().HasOne(x => x.Activity).WithMany(x => x.Users).HasForeignKey(x => x.ActivityId);
            builder.Entity<UserActivity>().HasOne(x => x.User).WithMany(x => x.Activities).HasForeignKey(x => x.UserId);
            builder.Entity<UserActivity>().HasMany(x => x.Documents).WithOne(x => x.UserActivity).HasForeignKey(x => x.UserActivityId);
            builder.Entity<UserActivity>().HasMany(x => x.Credits).WithOne(x => x.UserActivity).HasForeignKey(x => x.UserActivityId);

            builder.Entity<UserActivityCredit>().ToTable("UserActivityCredits");
            builder.Entity<UserActivityCredit>().HasKey(x => x.Id);
            builder.Entity<UserActivityCredit>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<UserActivityCredit>().HasOne(x => x.UserActivity).WithMany(x => x.Credits).HasForeignKey(x => x.UserActivityId);
            builder.Entity<UserActivityCredit>().HasOne(x => x.CreditTypeCategory).WithMany().HasForeignKey(x => x.CreditTypeCategoryId);
            builder.Entity<UserActivityCredit>().Property(x => x.CreditHours).IsRequired().HasDefaultValue(0);

            builder.Entity<UserActivityDocument>().ToTable("UserActivityDocuments");
            builder.Entity<UserActivityDocument>().HasKey(x => x.Id);
            builder.Entity<UserActivityDocument>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<UserActivityDocument>().HasOne(x => x.UserActivity).WithMany(x => x.Documents).HasForeignKey(x => x.UserActivityId);
            builder.Entity<UserActivityDocument>().HasOne(x => x.Document).WithMany(x => x.Activities).HasForeignKey(x => x.DocumentId);
        }
        public static async Task SeedActivityTables2(IServiceProvider provider)
        {
            try
            {


                var ctx = provider.GetRequiredService<ElGroupoDbContext>();
                var dept = ctx.Departments.First();
                foreach (var u in ctx.Users.Take(100))
                {
                    var du = new DepartmentUser { Department = dept, User = u };
                    ctx.Add(du);
                }


                dept = ctx.Departments.Skip(1).First();
                foreach (var u in ctx.Users.Skip(100).Take(100))
                {
                    var du = new DepartmentUser { Department = dept, User = u };
                    ctx.Add(du);
                }

                await ctx.SaveChangesAsync();
                var dug = new DepartmentUserGroup { Department = ctx.Departments.First(), Name = "Group 1" };
                ctx.Add(dug);
                foreach (var du in ctx.Departments.Include(x => x.Users).First().Users.Take(15))
                {
                    var dugu = new DepartmentUserGroupUser { UserGroup = dug, User = du };
                    ctx.Add(dugu);
                }

                dug = new DepartmentUserGroup { Department = ctx.Departments.First(), Name = "Group 2" };
                ctx.Add(dug);
                foreach (var du in ctx.Departments.Include(x => x.Users).First().Users.Skip(15).Take(15))
                {
                    var dugu = new DepartmentUserGroupUser { UserGroup = dug, User = du };
                    ctx.Add(dugu);
                }


                dug = new DepartmentUserGroup { Department = ctx.Departments.Skip(1).First(), Name = "Group 1" };
                ctx.Add(dug);
                foreach (var du in ctx.Departments.Include(x => x.Users).Skip(1).First().Users.Take(15))
                {
                    var dugu = new DepartmentUserGroupUser { UserGroup = dug, User = du };
                    ctx.Add(dugu);
                }

                dug = new DepartmentUserGroup { Department = ctx.Departments.Skip(1).First(), Name = "Group 2" };
                ctx.Add(dug);
                foreach (var du in ctx.Departments.Include(x => x.Users).Skip(1).First().Users.Skip(15).Take(15))
                {
                    var dugu = new DepartmentUserGroupUser { UserGroup = dug, User = du };
                    ctx.Add(dugu);
                }
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var ddd = 3;
            }
        }
        public static async Task SeedActivityTables(IServiceProvider provider)
        {
            try
            {


                var ctx = provider.GetRequiredService<ElGroupoDbContext>();
                var creditType = new CreditType { Description = "CME" };
                ctx.Add(creditType);
                var ctc = new CreditTypeCategory { CreditType = creditType, Description = "AMA Category 1" };
                ctx.Add(ctc);
                ctc = new CreditTypeCategory { CreditType = creditType, Description = "AMA Category 1" };
                ctx.Add(ctc);
                ctc = new CreditTypeCategory { CreditType = creditType, Description = "NM Category 1" };
                ctx.Add(ctc);
                ctc = new CreditTypeCategory { CreditType = creditType, Description = "Specialty Certification" };
                ctx.Add(ctc);
                ctc = new CreditTypeCategory { CreditType = creditType, Description = "Post Graduate Education" };
                ctx.Add(ctc);
                ctc = new CreditTypeCategory { CreditType = creditType, Description = "Advanced Degrees" };
                ctx.Add(ctc);
                ctc = new CreditTypeCategory { CreditType = creditType, Description = "Self-Assessment Tests" };
                ctx.Add(ctc);
                ctc = new CreditTypeCategory { CreditType = creditType, Description = "Teaching" };
                ctx.Add(ctc);
                ctc = new CreditTypeCategory { CreditType = creditType, Description = "Physician Preceptors" };
                ctx.Add(ctc);
                ctc = new CreditTypeCategory { CreditType = creditType, Description = "Papers and Publications" };
                ctx.Add(ctc);
                ctc = new CreditTypeCategory { CreditType = creditType, Description = "CPR" };
                ctx.Add(ctc);


                creditType = new CreditType { Description = "AMEP" };
                ctx.Add(creditType);
                ctc = new CreditTypeCategory { CreditType = creditType, Description = "General" };
                ctx.Add(ctc);
                await ctx.SaveChangesAsync();
                var org = new Organization
                {
                    Name = "University of New Mexico Health Sciences Center",
                    Address1 = "2425 Camino de Salud",
                    City = "Albuquerque",
                    State = "NM",
                    Zip = "87106",
                };
                foreach (var cats in ctx.CreditTypes)
                {
                    var acc = new OrganizationAccreditation { Organization = org, CreditType = cats };
                    org.Accreditations.Add(acc);
                }
                var dept = new Department { Organization = org, Name = "Radiology" };
                ctx.Add(dept);
                dept = new Department { Organization = org, Name = "Psychiatry" };
                ctx.Add(dept);

                //var aat = new ActivityAttendanceType { Description = "Attendee" };
                //ctx.Add(aat);
                //aat = new ActivityAttendanceType { Description = "Presenter" };
                //ctx.Add(aat);
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var ddd = 4;
            }
        }

        public static async Task CreateAdminAccount(IServiceProvider provider, IConfiguration configuration)
        {
            UserManager<User> userManager = provider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole<long>> roleManager = provider.GetRequiredService<RoleManager<IdentityRole<long>>>();

            string adminName = "admin";
            string roleName = "admin";
            if (await userManager.FindByNameAsync(adminName) == null)
            {
                if (await roleManager.FindByNameAsync(roleName) == null) await roleManager.CreateAsync(new IdentityRole<long>(roleName));
                User newUser = new User { UserName = adminName, Email = "nautachris@gmail.com", EmailConfirmed = true };
                var createResult = await userManager.CreateAsync(newUser, "flyguy23");
                if (createResult.Succeeded) await userManager.AddToRoleAsync(newUser, roleName);


            }
        }

        public static async Task SplitNames(IServiceProvider provider)
        {
            var ctx = provider.GetRequiredService<ElGroupoDbContext>();
            try
            {

                var cnt = 0;
                foreach (var u in ctx.Users.ToList())
                {
                    cnt++;
                    if (u.Name == null) continue;
                    var ary = u.Name.Split(' ');
                    if (ary.Length == 1) u.FirstName = ary[0];
                    else if (ary.Length == 2)
                    {
                        u.FirstName = ary[0];
                        u.LastName = ary[1];
                    }
                    else
                    {
                        var wtf = 4;
                    }
                    ctx.Update(u);

                    if (cnt % 500 == 0)
                    {
                        await ctx.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                var dd = 4;
            }


            await ctx.SaveChangesAsync();
        }

        public static async Task AddUserPhotos(IServiceProvider provider)
        {
            var ctx = provider.GetRequiredService<ElGroupoDbContext>();

            var photoDict = new Dictionary<int, string>();
            var count = 0;
            foreach (var photo in new DirectoryInfo("C:\\Photos").GetFiles("*.jpg", SearchOption.TopDirectoryOnly))
            {
                count++;
                photoDict.Add(count, photo.FullName);
            }


            count = 0;
            Random r = new Random(4845);
            var usersWithoutPhotos = ctx.Set<User>().Include(x => x.Photo).Where(x => x.Photo == null).ToList();
            foreach (var u in usersWithoutPhotos)
            {
                count++;
                var idx = r.Next(1, photoDict.Count);

                var imageBytes = CreateThumbnail(photoDict[idx], "image/jpeg");
                var newPhoto = new UserPhoto
                {
                    ContentType = "image/jpeg",
                    ImageData = imageBytes
                };
                ctx.UserPhotos.Add(newPhoto);
                u.Photo = newPhoto;
                ctx.Update(u);

                if (count % 20 == 0)
                {
                    await ctx.SaveChangesAsync();
                    count = 0;
                }
            }
            await ctx.SaveChangesAsync();
        }

        public static bool ThumbnailCallback()
        {
            return false;
        }
        private static byte[] CreateThumbnail(string fileName, string contentType)
        {
            //all images will now be 300 px wide
            int width = 300;
            Image i = Image.FromFile(fileName);

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

        public static async Task CreateUsers(IServiceProvider provider)
        {
            var ctx = provider.GetRequiredService<ElGroupoDbContext>();
            UserManager<User> userManager = provider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole<long>> roleManager = provider.GetRequiredService<RoleManager<IdentityRole<long>>>();

            var user = new User { UserName = "andy", Name = "Andy Ortegon", Email = "aoimba21@gmail.com", ZipCode = "87103", EmailConfirmed = true, PhoneNumber = "5022350804" };
            await userManager.CreateAsync(user, "505Albuquerque");

            user = new User { UserName = "eric", Name = "Eric Reskin", Email = "eric40222@gmail.com", ZipCode = "40018", EmailConfirmed = true, PhoneNumber = "5024724294" };
            await userManager.CreateAsync(user, "505Albuquerque");

            //user = new User { UserName = "chris", Name = "Chris Saylor", Email = "nautachris@gmail.com", ZipCode = "87111", EmailConfirmed = true, PhoneNumber = "3039083635" };
            //await userManager.CreateAsync(user, "505Albuquerque");

            user = new User { UserName = "user1", Name = "user1", Email = "elgroupouser1@gmail.com", ZipCode = "87111", EmailConfirmed = true, PhoneNumber = "3039083635" };
            await userManager.CreateAsync(user, "flyguy23");

            user = new User { UserName = "user2", Name = "user2", Email = "elgroupouser2@gmail.com", ZipCode = "87111", EmailConfirmed = true, PhoneNumber = "3039083635" };
            await userManager.CreateAsync(user, "flyguy23");
            user = new User { UserName = "user3", Name = "user3", Email = "elgroupouser3@gmail.com", ZipCode = "87111", EmailConfirmed = true, PhoneNumber = "3039083635" };
            await userManager.CreateAsync(user, "flyguy23");

            user = new User { UserName = "user4", Name = "user4", Email = "elgroupouser4@gmail.com", ZipCode = "87111", EmailConfirmed = true, PhoneNumber = "3039083635" };
            await userManager.CreateAsync(user, "flyguy23");
            user = new User { UserName = "user5", Name = "user5", Email = "elgroupouser5@gmail.com", ZipCode = "87111", EmailConfirmed = true, PhoneNumber = "3039083635" };
            await userManager.CreateAsync(user, "flyguy23");


            var count = 0;
            string filePath = @"C:\Projects\ElGroupo\ElGroupoApp\fakecontacts.csv";
            var reader = File.OpenText(filePath);
            var line = reader.ReadLine();
            line = reader.ReadLine();
            while (line != null)
            {
                count++;
                var ary = line.Split('|');
                string name = ary[4] + " " + ary[6];
                string zip = ary[11];
                string email = ary[14];
                string username = ary[15];
                var phone = ary[18];
                if (zip.Length < 5)
                {
                    var dddid = 4;
                    zip = "0" + zip;
                }
                var newUser = new User { UserName = username, Name = name, Email = email, ZipCode = zip, EmailConfirmed = true, PhoneNumber = phone };
                var result = await userManager.CreateAsync(newUser, "flyguy23");
                if (!result.Succeeded)
                {
                    var fff = 4;
                }

                line = reader.ReadLine();
            }


            count = 0;
            foreach (var u in await ctx.Users.Include(x => x.ContactMethods).Where(x => !x.ContactMethods.Any()).ToListAsync())
            {
                count++;
                foreach (var ct in ctx.ContactTypes)
                {
                    var cm = new UserContactMethod
                    {
                        User = u,
                        ContactMethod = ct,
                        Value = "contact"
                    };
                    ctx.Add(cm);

                }

                if (count % 100 == 0)
                {
                    await ctx.SaveChangesAsync();
                    count = 0;
                }
            }
            await ctx.SaveChangesAsync();


        }
        public static List<User> ReadUsers(string path)
        {
            var users = new List<User>();
            var reader = File.OpenText(path);
            var line = reader.ReadLine();
            while (line != null)
            {
                var ary = line.Split('|');
                var newUser = new User();
                newUser.LastName = ary[0];
                newUser.FirstName = ary[1];
                newUser.Email = ary[2];
                newUser.PhoneNumber = "505 555-1212";
                newUser.UserName = newUser.Email;
                newUser.ZipCode = "87111";
                newUser.TimeZoneId = "Mountain Standard Time";
                newUser.EmailConfirmed = true;
                users.Add(newUser);
                line = reader.ReadLine();
            }
            return users;
        }

        public static async Task PopulateRealActivities(IServiceProvider provider)
        {
            var ctx = provider.GetRequiredService<ElGroupoDbContext>();
            UserManager<User> userManager = provider.GetRequiredService<UserManager<User>>();
            //var user = ctx.Users.First(x => x.Id == 8);

            //var users = ReadUsers("C:\\Projects\\ElGroupo\\ElGroupoApp\\radiology_attendings.csv");
            //var facGroup = ctx.DepartmentUserGroups.Include(x => x.Department).First(x => x.Id == 4293);
            //foreach (var u in users)
            //{
            //    var user = ctx.Users.FirstOrDefault(x => x.Email == u.Email);
            //    if (user == null) continue;
            //    var deptUser = ctx.DepartmentUsers.FirstOrDefault(x => x.User.Id == user.Id && x.Department.Id == 4042);
            //    if (deptUser == null)
            //    {
            //        deptUser = new DepartmentUser { User = user, Department = ctx.Departments.First(x => x.Id == 4042) };
            //        ctx.Add(deptUser);
            //    }
            //    var facGroupUser = ctx.DepartmentUserGroupUsers.Include(x => x.User).ThenInclude(x => x.User).Include(x => x.UserGroup).FirstOrDefault(x => x.User.User.Id == user.Id && x.UserGroup.Id == 4293);
            //    if (facGroupUser == null)
            //    {
            //        facGroupUser = new DepartmentUserGroupUser { UserGroup = facGroup, User = deptUser };
            //        ctx.Add(facGroupUser);
            //    }
            //}

            //await ctx.SaveChangesAsync();
            var resGroup = ctx.DepartmentUserGroups.First(x => x.Id == 4277);
            var users = ReadUsers("C:\\Projects\\ElGroupo\\ElGroupoApp\\radiology_residents.csv");
            
            foreach (var u in users)
            {
                var user = ctx.Users.FirstOrDefault(x => x.Email == u.Email);
                if (user == null) {
                    await userManager.CreateAsync(u, "scatterbrain1");
                    user = ctx.Users.FirstOrDefault(x => x.Email == u.Email);
                }
                var deptUser = ctx.DepartmentUsers.Include(x=>x.User).FirstOrDefault(x => x.User.Id == user.Id && x.Department.Id == 4042);
                if (deptUser == null)
                {
                    deptUser = new DepartmentUser { User = user, Department = ctx.Departments.First(x => x.Id == 4042) };
                    ctx.Add(deptUser);
                }
                var resGroupUser = ctx.DepartmentUserGroupUsers.Include(x => x.User).ThenInclude(x => x.User).Include(x => x.UserGroup).FirstOrDefault(x => x.User.User.Id == user.Id && x.UserGroup.Id == 4277);
                if (resGroupUser == null)
                {
                    resGroupUser = new DepartmentUserGroupUser { UserGroup = resGroup, User = deptUser };
                    ctx.Add(resGroupUser);
                }
            }
            await ctx.SaveChangesAsync();
            return;

            //var actGroup = new ActivityGroup { Department = facGroup.Department, Name = "Tumor Board", User = user };

            //ctx.Add(actGroup);
            //var actUserGroup = new DepartmentUserGroupActivityGroup { ActivityGroup = actGroup, UserGroup = facGroup };
            //ctx.Add(actUserGroup);
            //actGroup = new ActivityGroup { Department = facGroup.Department, Name = "Conference", User = user };
            //ctx.Add(actGroup);
            //actUserGroup = new DepartmentUserGroupActivityGroup { ActivityGroup = actGroup, UserGroup = facGroup };
            //ctx.Add(actUserGroup);
            //actGroup = new ActivityGroup { Department = facGroup.Department, Name = "Teaching", User = user };
            //ctx.Add(actGroup);
            //actUserGroup = new DepartmentUserGroupActivityGroup { ActivityGroup = actGroup, UserGroup = facGroup };
            //ctx.Add(actUserGroup);
            //actGroup = new ActivityGroup { Department = facGroup.Department, Name = "Preceptorship", User = user };
            //ctx.Add(actGroup);
            //actUserGroup = new DepartmentUserGroupActivityGroup { ActivityGroup = actGroup, UserGroup = facGroup };
            //ctx.Add(actUserGroup);
            //actGroup = new ActivityGroup { Department = facGroup.Department, Name = "CPR", User = user };
            //ctx.Add(actGroup);
            //actUserGroup = new DepartmentUserGroupActivityGroup { ActivityGroup = actGroup, UserGroup = facGroup };
            //ctx.Add(actUserGroup);
            //actGroup = new ActivityGroup { Department = facGroup.Department, Name = "AMEP Workshop", User = user };
            //ctx.Add(actGroup);
            //actUserGroup = new DepartmentUserGroupActivityGroup { ActivityGroup = actGroup, UserGroup = facGroup };
            //ctx.Add(actUserGroup);
            //actGroup = new ActivityGroup { Department = facGroup.Department, Name = "Other", User = user };
            //ctx.Add(actGroup);
            //actUserGroup = new DepartmentUserGroupActivityGroup { ActivityGroup = actGroup, UserGroup = facGroup };
            //ctx.Add(actUserGroup);
            ctx.SaveChanges();

            //read users

            //foreach (var newUser in users)
            //{

            //    await userManager.CreateAsync(newUser, "scatterbrain1");
            //    var deptUser = new DepartmentUser { User = newUser, Department = facGroup.Department };
            //    ctx.Add(deptUser);
            //    var userGroupUser = new DepartmentUserGroupUser { User = deptUser, UserGroup = facGroup };
            //    ctx.Add(deptUser);
            //}
            //ctx.SaveChanges();

            //actGroup = new ActivityGroup { Department = resGroup.Department, Name = "Resident Noon Conference", User = user };
            //ctx.Add(actGroup);
            //actUserGroup = new DepartmentUserGroupActivityGroup { ActivityGroup = actGroup, UserGroup = resGroup };
            //ctx.Add(actUserGroup);

            //actGroup = new ActivityGroup { Department = resGroup.Department, Name = "Resident Case Conference", User = user };
            //ctx.Add(actGroup);
            //actUserGroup = new DepartmentUserGroupActivityGroup { ActivityGroup = actGroup, UserGroup = resGroup };
            //ctx.Add(actUserGroup);

            //actGroup = new ActivityGroup { Department = resGroup.Department, Name = "Journal Club", User = user };
            //ctx.Add(actGroup);
            //actUserGroup = new DepartmentUserGroupActivityGroup { ActivityGroup = actGroup, UserGroup = resGroup };
            //ctx.Add(actUserGroup);

            //actGroup = new ActivityGroup { Department = resGroup.Department, Name = "Brandt Helms Conference", User = user };
            //ctx.Add(actGroup);
            //actUserGroup = new DepartmentUserGroupActivityGroup { ActivityGroup = actGroup, UserGroup = resGroup };
            //ctx.Add(actUserGroup);


            //ctx.SaveChanges();

            //users = ReadUsers("C:\\Projects\\ElGroupo\\ElGroupoApp\\radiology_residents.csv");
            //foreach (var newUser in users)
            //{

            //    await userManager.CreateAsync(newUser, "scatterbrain");
            //    var deptUser = new DepartmentUser { User = newUser, Department = resGroup.Department };
            //    ctx.Add(deptUser);
            //    var userGroupUser = new DepartmentUserGroupUser { User = deptUser, UserGroup = resGroup };
            //    ctx.Add(deptUser);
            //}

            //ctx.SaveChanges();
        }
        public static void PopulateSomeFakeActivities(IServiceProvider provider)
        {
            try
            {
                var ctx = provider.GetRequiredService<ElGroupoDbContext>();

                //var creditType = ctx.CreditTypes.First(x => x.Description == "AMEP");
                //var cat = new CreditTypeCategory { CreditType = creditType, Description = "Evidence-Based Practices for EffectiveLarge group Teaching and Learning" };
                //ctx.Add(cat);

                //cat = new CreditTypeCategory { CreditType = creditType, Description = "Evidence-based practices for effective small-group teaching and learning" };
                //ctx.Add(cat);

                //cat = new CreditTypeCategory { CreditType = creditType, Description = "Best practices in providing feedback with good judgment to learners and colleagues" };
                //ctx.Add(cat);
                //cat = new CreditTypeCategory { CreditType = creditType, Description = "Learning science foundations for an evidence-based framework for teaching practice" };
                //ctx.Add(cat);

                //cat = new CreditTypeCategory { CreditType = creditType, Description = "Best practices for mentoring researchers and developing research skills, offered by the CTSC Faculty Mentor Development Program" };
                //ctx.Add(cat);

                //cat = new CreditTypeCategory { CreditType = creditType, Description = "Evidence-based practices for teaching clinical reasoning and bedside practice" };
                //ctx.Add(cat);

                //cat = new CreditTypeCategory { CreditType = creditType, Description = "Developing curriculum at session to course level: objectives, learning activities, assessment" };
                //ctx.Add(cat);

                //cat = new CreditTypeCategory { CreditType = creditType, Description = "Participate in Peer Observation in Support of Effective Teaching (POSET)" };
                //ctx.Add(cat);

                //cat = new CreditTypeCategory { CreditType = creditType, Description = "Training session led by an OMED facilitator" };
                //ctx.Add(cat);

                //cat = new CreditTypeCategory { CreditType = creditType, Description = "Write and submit a teaching philosophy" };
                //ctx.Add(cat);


                var user = ctx.Users.First(x => x.Id == 8);
                //populated activity groups linked to user groups
                foreach (var deptUser in ctx.DepartmentUsers.Include(x => x.User).Include(x => x.Department).Where(x => x.User.Id == 8))
                {
                    foreach (var deptUserGroup in ctx.DepartmentUserGroupUsers.Include(x => x.User).Include(x => x.UserGroup).Where(x => x.User.Id == deptUser.Id))
                    {
                        //i.e. radiologoy residents
                        var activityGroup = new ActivityGroup
                        {
                            Department = deptUser.Department,
                            Name = "Happy Hour (" + deptUserGroup.UserGroup.Name + ")"
                        };
                        ctx.Add(activityGroup);
                        var activity = new Activity
                        {
                            ActivityGroup = activityGroup,
                            Description = "Wednesday, 4/25/18",
                            Location = "In Hell",
                            Credits = new List<ActivityCredit>()
                        };
                        //add all credit types
                        foreach (var creditTypeActivity in ctx.CreditTypeCategories)
                        {
                            activity.Credits.Add(new ActivityCredit { CreditTypeCategory = creditTypeActivity });
                        }
                        ctx.Add(activity);
                        var activityGroupUserGroup = new DepartmentUserGroupActivityGroup
                        {
                            ActivityGroup = activityGroup,
                            UserGroup = deptUserGroup.UserGroup
                        };
                        ctx.Add(activityGroupUserGroup);
                        var userActivity = new UserActivity { Activity = activity, User = deptUser.User, Credits = new List<UserActivityCredit>() };
                        //userActivity.AttendanceType = ctx.ActivityAttendanceTypes.First();
                        userActivity.IsPresenting = false;
                        foreach (var c in activity.Credits)
                        {
                            userActivity.Credits.Add(new UserActivityCredit { CreditHours = 1, CreditTypeCategory = c.CreditTypeCategory });
                        }
                        ctx.Add(userActivity);

                    }
                }

                //populates "private" groups
                //for (var x = 1; x < 5; x++)
                //{
                //    var activityGroup = new ActivityGroup
                //    {
                //        User = user,
                //        Name = "My Own Activity " + x.ToString()

                //    };
                //    ctx.Add(activityGroup);
                //    var activity = new Activity
                //    {
                //        ActivityGroup = activityGroup,
                //        Description = "Wednesday, 4/25/18",
                //        Location = "Office",
                //        StartDate = DateTime.Now.AddDays(Convert.ToDouble(x)),
                //        EndDate = DateTime.Now.AddDays(Convert.ToDouble(x)).AddHours(1),
                //        IsPublic = false,
                //        Credits = new List<ActivityCredit>()
                //    };
                //    //add all credit types
                //    foreach (var creditTypeActivity in ctx.CreditTypeCategories)
                //    {
                //        activity.Credits.Add(new ActivityCredit { CreditTypeCategory = creditTypeActivity });
                //    }
                //    ctx.Add(activity);

                //    var userActivity = new UserActivity { Activity = activity, User = user, Credits = new List<UserActivityCredit>() };
                //    userActivity.AttendanceType = ctx.ActivityAttendanceTypes.First();
                //    foreach (var c in activity.Credits)
                //    {
                //        userActivity.Credits.Add(new UserActivityCredit { CreditHours = 1, CreditTypeCategory = c.CreditTypeCategory });
                //    }
                //    ctx.Add(userActivity);
                //}


                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                var fff = 4;
            }

        }

        public static async Task PopulateUserContacts(IServiceProvider provider)
        {
            try
            {
                var ctx = provider.GetRequiredService<ElGroupoDbContext>();
                foreach (var u in ctx.Users)
                {
                    foreach (var ct in ctx.ContactTypes)
                    {
                        u.AddContact(ct, "contact");

                    }
                    ctx.Update(u);
                }
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }


        }


        public static async Task CreateContactTypes(IServiceProvider provider)
        {
            var ctx = provider.GetRequiredService<ElGroupoDbContext>();
            if (await ctx.Set<Lookups.ContactMethod>().CountAsync() > 0) return;


            var ct = new Lookups.ContactMethod();
            ct.Value = "Home Phone";
            ctx.ContactTypes.Add(ct);

            ct = new Lookups.ContactMethod();
            ct.Value = "Mobile Phone";
            ctx.ContactTypes.Add(ct);

            ct = new Lookups.ContactMethod();
            ct.Value = "Email";
            ctx.ContactTypes.Add(ct);

            ct = new Lookups.ContactMethod();
            ct.Value = "Facebook";
            ctx.ContactTypes.Add(ct);

            ct = new Lookups.ContactMethod();
            ct.Value = "Twitter";
            ctx.ContactTypes.Add(ct);

            ct = new Lookups.ContactMethod();
            ct.Value = "Instagram";
            ctx.ContactTypes.Add(ct);

            await ctx.SaveChangesAsync();
        }
    }

    public class ElGroupoContextFactory : IDbContextFactory<ElGroupoDbContext>
    {
        public ElGroupoDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<ElGroupoDbContext>();
            //builder.UseSqlServer("Server=(local);Database=Tribes;Trusted_Connection=True;MultipleActiveResultSets=true");
            //builder.UseSqlServer("Server=aa1ewoc6epra7at.chazths3rr6k.us-east-2.rds.amazonaws.com,1433;Database=Footprint;UID=footprintapp;PWD=505Albuquerque;MultipleActiveResultSets=true");
            builder.UseSqlServer("Server=aal7m7n920130o.chazths3rr6k.us-east-2.rds.amazonaws.com,1433;Database=footprint;UID=footprintapp;PWD=505Albuquerque;MultipleActiveResultSets=true");
            return new ElGroupoDbContext(builder.Options);
        }
    }



}
