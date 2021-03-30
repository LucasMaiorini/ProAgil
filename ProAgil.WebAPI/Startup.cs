using System.IO;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using ProAgil.Domain.Identity;
using ProAgil.Repository;

namespace ProAgil.WebAPI
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
            services.
            AddDbContext<ProAgilContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            //This configuration sets requirements to the password field
            IdentityBuilder builder = services.AddIdentityCore<User>(option =>
            {
                option.Password.RequireDigit = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireLowercase  = false;
                option.Password.RequireUppercase = false;
                option.Password.RequiredLength = 4;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            //The Context still defines the others entities
            builder.AddEntityFrameworkStores<ProAgilContext>();
            //The configurations that follows defines details of the permission and validations 
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            //JWT Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //Validates through the Emitter Key
                    ValidateIssuerSigningKey = true,
                    //AppSettings is configured in appsettings.Development.json
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.
                    GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddScoped<IProAgilRepository, ProAgilRepository>();
            services.AddAutoMapper();
            //Defines the MVC to project
            services.AddMvc(option =>
            {
                //Everytime a controller is called, follows this configuration which ensure that users must be authenticated
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                option.Filters.Add(new AuthorizeFilter(policy));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(option =>
            {
                option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Validates all the JWT and Authentication section.
            app.UseAuthentication();

            // app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                RequestPath = new PathString("/Resources")
            });
            app.UseMvc();
        }
    }
}
