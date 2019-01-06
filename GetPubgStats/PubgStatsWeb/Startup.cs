using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using PubgStatsWeb.Authentication.Twitch;
using Microsoft.AspNetCore.Authentication;
using PubgStatsWeb.Authentication.Data;
using PubgStatsWeb.Code.Config;
using PubgStatsWeb.Code.Extensions;

namespace PubgStatsWeb
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<TwitchOAuthConfig>("TwitchOAuth", this.Configuration);

            services.AddAuthentication().AddOAuth<TwitchOAuthOptions, TwitchOAuthHandler>("Twitch", "Twitch", _options =>
            {
                TwitchOAuthConfig config = this.Configuration.GetTwitchOAuthConfig("TwitchOAuth");

                foreach (string scope in config.Scopes)
                {
                    _options.Scope.Add(scope);
                }

                _options.CallbackPath = new PathString(config.RedirectUri);
                _options.AlwaysForceVerification = config.ForceVerify;
                _options.ClientSecret = config.ClientSecret;
                _options.ClientId = config.ClientId;
                _options.SaveTokens = true;

            });

            services.AddDbContext<UsersDbContext>(_options => _options.UseMySql(this.Configuration.GetValue<string>("ConnectionStrings:UsersDbConnection")));
            services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<UsersDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
