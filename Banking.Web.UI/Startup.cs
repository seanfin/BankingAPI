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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;

using Banking.Core.Utils;
using Banking.Core.Helper;
using Banking.Web.UI.Data;
using Banking.Web.UI.Models;


namespace Banking.Web.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        

        


        public IConfiguration Configuration { get; }





        //// Set cache options.
        //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
        //    // Keep in cache for this time, reset time if accessed.
        //    .SetSlidingExpiration(TimeSpan.FromSeconds(3));

        //IMemoryCache memoryCache = new MemoryCache();




        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            MemoryCache cache = new MemoryCache(new MemoryCacheOptions());

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<ApplicationDbContext>(options => { options.UseMemoryCache(cache) ; });
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager<BankSignInManager<IdentityUser>>()
                .AddUserManager<BankUserManager<IdentityUser>>();


            services.AddControllersWithViews();
            services.AddRazorPages();

            ExternalAppSettings externalAppSettings = AppSettingHelper.GetExternalApplicationConfiguration();


            services.AddHttpClient("BankTransactions", client =>
            {
                client.BaseAddress = new Uri(externalAppSettings.WebApiURLBankTransaction);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("Users", client =>
            {
                client.BaseAddress = new Uri(externalAppSettings.WebApiURLUser);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("Profiles", client =>
            {
                client.BaseAddress = new Uri(externalAppSettings.WebApiURLProfile);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });



            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.AccessDeniedPath = new PathString("/Identity/Account/Access");
                        options.LoginPath = new PathString("/Identity/Account/Login");
                    });






            services.AddHttpContextAccessor();
            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

           


            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax,
            };

            app.UseCookiePolicy();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
                      

            app.UseHttpsRedirection();
            

            

            
            app.UseStaticFiles();

            

            app.UseMvc();

           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
