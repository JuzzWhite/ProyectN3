using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using TecnologyShop.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using TecnologyShop.Services;
using TecnologyShop.Utility;

using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Localization;
using Stripe;

namespace TecnologyShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //antiguo
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            //services.AddControllersWithViews();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();



            services.AddIdentity<IdentityUser, IdentityRole>()
                      .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddSingleton<IEmailSender, EmailSender>();

            //stripe Configuration 
            services.Configure<StripeSettings>
                (Configuration.GetSection("Stripe"));
            //end stripe configuration

            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            //add service
            services.AddMvc();
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            }
           );
            //Authorization Update
            //Microsoft has changed few things with Identity vs DefaultIdentity 
            //which we updated in startup.cs a while ago.
            //Because of which the default paths to login / logout and access denied fails.
            //Authentication and authorization
            //website to explain it:
            //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity?view=aspnetcore-3.0&tabs=visual-studio#create-full-identity-ui-source
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseMigrationsEndPoint();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            //app.UseHttpsRedirection();
            //app.UseStaticFiles();

            //app.UseRouting();

            //app.UseAuthentication();
            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
            //    endpoints.MapRazorPages();
            //});

            //to manage the decimal point and others
            //characteristic of global setting
            var supportedCultures = new string[] { "en-GB", "en-US" };
            app.UseRequestLocalization(options =>
                        options
                        .AddSupportedCultures(supportedCultures)
                        .AddSupportedUICultures(supportedCultures)
                        .SetDefaultCulture("en-GB")
                        .RequestCultureProviders.Insert(0,
                               new CustomRequestCultureProvider(context =>
                               {
                                   return Task.FromResult(new ProviderCultureResult("en-GB"));
                               }))
                );
            //end global setting
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseRouting();
            //use for Stripe configuation
            StripeConfiguration.ApiKey =
                Configuration.GetSection("Stripe")["SecretKey"];
            //end configuration

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });


        }
    }
}
