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

namespace ElGroupo.Domain.Data
{


    public class ElGroupoDbContext : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public DbSet<Lookups.ContactMethod> ContactTypes { get; set; }

        public DbSet<UnregisteredEventAttendee> UnregisteredEventAttendees { get; set; }

        public DbSet<RecurringEvent> RecurringEvents { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventAttendee> EventAttendees { get; set; }
        public DbSet<AttendeeGroup> AttendeeGroups { get; set; }


        public DbSet<MessageBoardItem> MessageBoardItems { get; set; }
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
            builder.Entity<Event>().HasMany(x => x.MessageBoardItems).WithOne(x => x.Event);
            builder.Entity<Event>().HasMany(x => x.Notifications).WithOne(x => x.Event);

            builder.Entity<EventNotification>().HasOne(x => x.PostedBy).WithMany(x => x.PostedNotifications).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);

            builder.Entity<MessageBoardItem>().HasMany(x => x.Attendees).WithOne(x => x.MessageBoardItem).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            builder.Entity<EventAttendee>().HasMany(x => x.MessageBoardItems).WithOne(x => x.Attendee).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            builder.Entity<EventAttendee>().HasMany(x => x.Notifications).WithOne(x => x.Attendee).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);





            //builder.Entity<EventOrganizer>().HasOne(x => x.Event).WithMany(y => y.Organizers).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            //builder.Entity<EventOrganizer>().HasOne(x => x.User).WithMany(y => y.OrganizedEvents).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            builder.Entity<MessageBoardItemAttendee>().HasOne(x => x.MessageBoardItem).WithMany(x => x.Attendees);

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
