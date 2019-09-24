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
using Flights.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Flights.Data.Entities;
using System.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Flights.Extensions;
using System.IO;
using Flights.Binders;

namespace Flights
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

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("IdentityConnection")));

            services.AddIdentity<Person, IdentityRole>(options => {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<IdentityErrorDescriberFR>()
            .AddUserManager<CUserManager<Person>>()
            .AddSignInManager<CSignInManager<Person>>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login";
                options.LogoutPath = "/Login/Disconnect";
                options.AccessDeniedPath = "/Login/ForbiddenAccess";
                options.Cookie.Expiration = TimeSpan.FromDays(14);
            });

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc(options =>
            {
                // add custom binder to beginning of collection
                options.ModelBinderProviders.Insert(0, new ModelBinderProvider());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, CUserManager<Person> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // Page d'erreur
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // S'assurer que la base de données et les tables existent et si inexistant, les créer
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if(context.Database.EnsureCreated())
                {
                    // On initilialise la base de données si elle est inexistante !
                    var initResult = Initialize(userManager, roleManager);
                    initResult.Wait();

                    if (!initResult.Result) throw new Exception("Database init failed !");
                }
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        private async Task<bool> CreateAccountAndAddToRole(CUserManager<Person> userManager, RoleManager<IdentityRole> roleManager, Person accountToCreate, string password, string roleName)
        {
            bool result = true;

            try
            {
                // Création du rôle si inexistant !
                if (await roleManager.RoleExistsAsync(roleName) == false)
                {
                    // Création du rôle
                    var role = new IdentityRole { Name = roleName };
                    var identityRoleResult = await roleManager.CreateAsync(role);
                    result &= identityRoleResult.Succeeded;
                }

                // Création du rôle
                var identityResult = await userManager.CreateAsync(accountToCreate, password);
                result &= identityResult.Succeeded;

                // Ajout du compte au rôle
                identityResult = await userManager.AddToRoleAsync(accountToCreate, roleName);
                result &= identityResult.Succeeded;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private async Task<bool> Initialize(CUserManager<Person> userManager, RoleManager<IdentityRole> roleManager)
        {
            bool result = true;

            // Compte Administrateur
            Person admin = new Person
            {
                Email = Configuration["AdminInitEmail"],
                UserName = CUserManager<Person>.ADMINISTRATOR,
                Surname = string.Empty,
                Name = string.Empty,
                Address = string.Empty,
                Town = string.Empty,
                ZIPCode = string.Empty,
                PhoneNumber = string.Empty,
                Status = EntitiesEnums.EStatus.ACTIVE,
                Civility = Person.ECivility.SIR
            };

            // Compte Client test
            Person client = new Person
            {
                Email = Configuration["TestAccountEmail"],
                Surname = "te",
                Name = "st",
                Address = string.Empty,
                Town = string.Empty,
                ZIPCode = string.Empty,
                PhoneNumber = string.Empty,
                Status = EntitiesEnums.EStatus.ACTIVE,
                Civility = Person.ECivility.SIR
            };

            result &= await CreateAccountAndAddToRole(userManager, roleManager, admin, Configuration["AdminInitPassword"], GlobalResources.ROLE_ADMIN);
            result &= await CreateAccountAndAddToRole(userManager, roleManager, client, Configuration["TestAccountPassword"], GlobalResources.ROLE_CLIENT);

            return result;
        }
    }
}
