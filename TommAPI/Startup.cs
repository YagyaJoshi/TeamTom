using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TommBLL.Interface;
using TommBLL.Repository;
using TommAPI.Provider;
using TommDAL.Models;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Http.Features;
using TommDAL;
using TommAPI.Providers;
using TommDAL.ViewModel;

namespace TommAPI
{
    public class Startup
    {
        #region Dependency Injection
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        #endregion
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            /* It is used for cros origin start*/
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
         //.AllowAnyMethod()
         //.AllowAnyHeader()));
         //   services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().WithMethods("GET", "POST", "PUT", "DELETE")
         //.AllowAnyMethod()
         //.AllowAnyHeader()));

            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                                                                .WithMethods("GET", "POST", "PUT", "DELETE")
                                                                .AllowAnyHeader()));
            /* It is used for cros origin End*/

            /* It is start used for Dependency injection*/
            services.AddScoped<IUser, UserRepo>();
            services.AddScoped<IAuth, AuthRepo>();
            services.AddScoped<IJobs, JobsRepo>();
            services.AddScoped<INotes, NotesRepo>();
            services.AddScoped<IServices, ServicesRepo>();
            services.AddScoped<IFoods, FoodsRepo>();
            services.AddScoped<IChritmas, ChritmasRepo>();
            services.AddScoped<IPlayList, PlayListRepo>();
            services.AddScoped<IBootcamp, BootcampRepo>();
            services.AddScoped<IAdmin, AdminRepo>();
            services.AddScoped<IHoliday, HolidayRepo>();
            services.AddScoped<IPost, PostsRepo>();
            services.AddScoped<IUserSubscription, UserSubscriptionRepo>();
            services.AddScoped<IArticle, ArticleRepo>();

            services.AddScoped<ICacheService, CacheService>();
            /* It is end used for Dependency injection*/

            /*Start It is Used to connect the database first approac*/
            services.Add(new ServiceDescriptor(typeof(organizedmumContext), new organizedmumContext()));
            /* End It is Used to connect the database first approac*/

            /* It is used for jwt token start */
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = false,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = Configuration["Jwt:Issuer"],
                       ValidAudience = Configuration["Jwt:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"])),
                       ClockSkew = TimeSpan.Zero,
                   };
               }).AddCookie(cfg => cfg.SlidingExpiration = true);

            /* It is used for jwt token End */

            // All endpoints need authorization using our custom authorization filter start
            services.AddMvc(options =>
            {
                options.Filters.Add(new ApiExceptionFilterAttribute());
                options.Filters.Add(new CustomAuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            });
            
            services.AddMvc()
            .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            // services.AddMvc(options => { options.Filters.Add(typeof(CustomExceptionFilterAttribute)); });

            // It is use to access Resource File start
            services.AddLocalization(o => o.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
            new CultureInfo("en-US"),
            new CultureInfo("en-GB"),
            new CultureInfo("de-DE")
        };
                options.DefaultRequestCulture = new RequestCulture("en-US", "en-US");

                // You must explicitly state which cultures your application supports.
                // These are the cultures the app supports for formatting 
                // numbers, dates, etc.

                options.SupportedCultures = supportedCultures;

                // These are the cultures the app supports for UI strings, 
                // i.e. we have localized resources for.

                options.SupportedUICultures = supportedCultures;
            });

            //services.AddSingleton<IHostedService, ScheduleTask>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Core Api", Description = "Swagger Core Api" });
            });

            // It is use to access Resource File end

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            #region All Midellware
            app.UseMiddleware<CorsMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            // app.UsePathBase("/aspdotnetcoredemo");
            // get the directory

            app.UseCors("AllowAll");
            // It is use to access static image,audio file 
            app.UseStaticFiles();
            //It is use to  Allow Cors Policy
           
            // It is use to Check Authenticate Use
            app.UseAuthentication();
            // It is use to Mvc
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "MyAPI V1");
            });

            #endregion
        }
    }
}
