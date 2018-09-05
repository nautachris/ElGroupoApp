using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ElGroupo.Web.Models.Configuration;
using Microsoft.Extensions.Configuration;
using RazorLight;
using ElGroupo.Domain.Data;
using ElGroupo.Domain;
using ElGroupo.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Microsoft.Extensions.Options;

namespace ElGroupo.Web
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            builder.AddUserSecrets<Startup>();
            Configuration = builder.Build();

        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ElGroupoDbContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<DbContext>(p => p.GetService<ElGroupoDbContext>());
            services.AddSingleton(p => new LookupTableService(p.GetService<ElGroupoDbContext>()));
            services.AddIdentity<User, IdentityRole<long>>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                //opts.SignIn.RequireConfirmedEmail = true;
                opts.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            }).AddEntityFrameworkStores<ElGroupoDbContext, long>();
            services.AddMvc();
            //services.AddTransient<IEmailSender, SendGridEmailSender>();





            services.Configure<AmazonSESConfig>(Configuration.GetSection("Email"));
            services.Configure<GoogleConfigOptions>(Configuration);
            services.AddSingleton(Configuration);

            //amazon email
            services.AddSingleton<AWSCredentials>(sp => {
                var opts = sp.GetRequiredService<IOptions<AmazonSESConfig>>();
                return new BasicAWSCredentials(opts.Value.AmazonEmailKey, opts.Value.AmazonEmailSecret);

            });
            //services.AddSingleton<AWSCredentials>(new BasicAWSCredentials(Configuration["AmazonEmailKey"], Configuration["AmazonEmailSecret"]));
            services.AddSingleton<Amazon.RegionEndpoint>(Amazon.RegionEndpoint.USEast1);
            services.AddSingleton<IAmazonSimpleEmailService>(x => 
                new AmazonSimpleEmailServiceClient(x.GetRequiredService<AWSCredentials>(), x.GetRequiredService<Amazon.RegionEndpoint>())
            );


            //services.AddTransient<IEmailSender, MailgunEmailSender>();
            services.AddTransient<EventService, EventService>();
            services.AddTransient<AccountService, AccountService>();
            services.AddTransient<UserService, UserService>();
            services.AddTransient<ActivitiesService, ActivitiesService>();
            services.AddTransient<RecordsService, RecordsService>();

            services.AddSingleton(EngineFactory.CreateEmbedded(typeof(Mail.Templates.TemplatePointer)));
            services.AddSingleton<IEmailService, MailService>();

            //filters

            services.AddScoped<ElGroupo.Web.Filters.EventOrganizerFilterAttribute>();





            var idBuilder = new IdentityBuilder(typeof(User), typeof(IdentityRole<int>), services);
            idBuilder.AddDefaultTokenProviders();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();
            //if (env.IsDevelopment())
            //{

            //}

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "content", "resources", "images")),
                RequestPath = new PathString("/Images")
            });

            app.UseIdentity();
            app.UseGoogleAuthentication(new GoogleOptions
            {
                ClientId = Configuration["GoogleClientId"],
                ClientSecret = Configuration["GoogleClientSecret"]

            });
            app.UseMvcWithDefaultRoute();
            //ElGroupoDbContext.SeedStates(app.ApplicationServices);
            //ElGroupoDbContext.SeedCollegeList(app.ApplicationServices);
            //ElGroupoDbContext.SeedNewInputTypes(app.ApplicationServices).Wait();
            //var sm = app.ApplicationServices.GetRequiredService<SignInManager<User>>();
            //ElGroupoDbContext.SeedDataTypes(app.ApplicationServices).Wait();
            //ElGroupoDbContext.SeedRecordTables(app.ApplicationServices).Wait();
           //ElGroupoDbContext.PopulateRealActivities(app.ApplicationServices).Wait();
            //Models.Configuration.EmailConfigOptions.SendTestEmail().Wait();
            //ElGroupoDbContext.CreateAdminAccount(app.ApplicationServices, Configuration).Wait();
            //ElGroupoDbContext.CreateUsers(app.ApplicationServices).Wait();
            //ElGroupoDbContext.CreateContactTypes(app.ApplicationServices).Wait();
            //ElGroupoDbContext.PopulateUserContacts(app.ApplicationServices).Wait();
            //ElGroupoDbContext.SeedActivityTables2(app.ApplicationServices).Wait();
            //ElGroupoDbContext.SplitNames(app.ApplicationServices).Wait();
            //



            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
