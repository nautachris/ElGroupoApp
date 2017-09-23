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
            services.AddIdentity<User, IdentityRole<int>>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            }).AddEntityFrameworkStores<ElGroupoDbContext, int>();
            services.AddMvc();
            //services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.AddTransient<IEmailSender, MailgunEmailSender>();
            services.AddTransient<EventService, EventService>();

            services.AddSingleton(EngineFactory.CreateEmbedded(typeof(Mail.Templates.TemplatePointer)));
            services.AddSingleton<MailService, MailService>();
            services.Configure<EmailConfigOptions>(Configuration);
            services.Configure<GoogleConfigOptions>(Configuration);

            //filters

            services.AddScoped<ElGroupo.Web.Filters.EventOrganizerFilterAttribute>();

            var idBuilder = new IdentityBuilder(typeof(User), typeof(IdentityRole<int>), services);
            idBuilder.AddDefaultTokenProviders();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

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


            //var sm = app.ApplicationServices.GetRequiredService<SignInManager<User>>();


            //Models.Configuration.EmailConfigOptions.SendTestEmail().Wait();
            //ElGroupoDbContext.CreateUsers(app.ApplicationServices).Wait();
            //ElGroupoDbContext.PopulateUserContacts(app.ApplicationServices).Wait();
            //ElGroupoDbContext.CreateAdminAccount(app.ApplicationServices, Configuration).Wait();
            //ElGroupoDbContext.CreateContactTypes(app.ApplicationServices).Wait();
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
