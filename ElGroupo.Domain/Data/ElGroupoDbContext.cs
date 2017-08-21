using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ElGroupo.Domain.Data.Configurations;


namespace ElGroupo.Domain.Data
{


    public class ElGroupoDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Lookups.ContactType> ContactTypes { get; set; }
        public DbSet<ContactGroup> ContactGroups { get; set; }

        public DbSet<ContactGroupUser> ContactGroupUsers { get; set; }
        public DbSet<UnregisteredEventAttendee> UnregisteredEventAttendees { get; set; }


        public DbSet<Event> Events { get; set; }
        public DbSet<EventAttendee> EventAttendees { get; set; }
        public DbSet<EventGroup> EventGroups { get; set; }
        public DbSet<MessageBoardItem> MessageBoardItems { get; set; }
        public DbSet<EventAttendeeNotification> EventAttendeeNotifications { get; set; }
        public DbSet<EventNotification> EventNotifications { get; set; }
        public DbSet<EventOrganizer> EventOrganizers { get; set; }
        public DbSet<MessageBoardItemAttendee> MessageBoardItemAttendees { get; set; }
        public DbSet<UserContact> UserContacts { get; set; }

        public DbSet<UserPhoto> UserPhotos { get; set; }


        public ElGroupoDbContext(DbContextOptions<ElGroupoDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.AddConfiguration<SequenceConfiguration>();
            builder.AddConfiguration<UserConfiguration>();
            builder.AddConfiguration<ContactTypeConfiguration>();
            builder.AddConfiguration<ContactGroupUserConfiguration>();
            builder.AddConfiguration<UnregisteredEventAttendeeConfiguration>();

            builder.Entity<UserPhoto>().ToTable("UserPhoto");


            builder.Entity<EventNotification>().HasOne(x => x.Event).WithMany(y => y.Notifications).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            builder.Entity<EventOrganizer>().HasOne(x => x.Event).WithMany(y => y.Organizers).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            builder.Entity<EventOrganizer>().HasOne(x => x.User).WithMany(y => y.OrganizedEvents).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            builder.Entity<MessageBoardItemAttendee>().HasOne(x => x.MessageBoardItem).WithMany(x => x.Attendees).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            //builder.Entity<ContactGroup>().HasMany(x => x.Users).WithOne(x => x.Group).HasForeignKey<ContactGroupUser>(y => y.ContactGroupId);
        }


        public static async Task CreateAdminAccount(IServiceProvider provider, IConfiguration configuration)
        {
            UserManager<User> userManager = provider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole<int>> roleManager = provider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            string adminName = "admin";
            string roleName = "admin";
            if (await userManager.FindByNameAsync(adminName) == null)
            {
                if (await roleManager.FindByNameAsync(roleName) == null) await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                User newUser = new User { UserName = adminName, Email = "nautachris@gmail.com" };
                var createResult = await userManager.CreateAsync(newUser, "cS!102381");
                if (createResult.Succeeded) await userManager.AddToRoleAsync(newUser, roleName);
            }
        }


        public static async Task CreateUsers(IServiceProvider provider)
        {
            var ctx = provider.GetRequiredService<ElGroupoDbContext>();
            //UserManager<User> userManager = provider.GetRequiredService<UserManager<User>>();
            //RoleManager<IdentityRole<int>> roleManager = provider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            //for (var x = 0; x < 25; x++)
            //{
            //    string name = "user" + x.ToString();
            //    string email = name + "@email.com";
            //    var newUser = new User { UserName = name, Email = email };
            //    var createResult = await userManager.CreateAsync(newUser, "cS!102381");


            //}
            //foreach (var u in ctx.Users.Include("Contacts").Where(x=>x.Contacts.Count == 0))
            //{
            //    foreach (var ct in ctx.ContactTypes)
            //    {
            //        u.AddContact(ct, "contact");

            //    }
            //    ctx.Update(u);
            //}

            foreach(var u in ctx.Users.Where(x=>x.Name == null))
            {
                u.Name = u.UserName;
                u.ZipCode = "87111";
                ctx.Update(u);
            }
            await ctx.SaveChangesAsync();


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
            if (await ctx.Set<Lookups.ContactType>().CountAsync() > 0) return;


            var ct = new Lookups.ContactType();
            ct.Value = "Home Phone";
            ctx.ContactTypes.Add(ct);

            ct = new Lookups.ContactType();
            ct.Value = "Mobile Phone";
            ctx.ContactTypes.Add(ct);

            ct = new Lookups.ContactType();
            ct.Value = "Email";
            ctx.ContactTypes.Add(ct);

            ct = new Lookups.ContactType();
            ct.Value = "Facebook";
            ctx.ContactTypes.Add(ct);

            ct = new Lookups.ContactType();
            ct.Value = "Twitter";
            ctx.ContactTypes.Add(ct);

            ct = new Lookups.ContactType();
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
            builder.UseSqlServer("Server=(local);Database=ElGroupo;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new ElGroupoDbContext(builder.Options);
        }
    }



}
