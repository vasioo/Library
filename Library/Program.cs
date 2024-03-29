using Hangfire;
using Library.DataAccess;
using Library.DataAccess.MainModels;
using Library.Models.Stripe;
using Library.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Net;

namespace Modum.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer((connectionString),
                 sqlServerOptionsAction: sqlOptions =>
                 {
                     sqlOptions.EnableRetryOnFailure();
                 });
            });
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(
                options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.SignIn.RequireConfirmedAccount= true;
                })
                   .AddRoles<IdentityRole>()
                   .AddEntityFrameworkStores<DataContext>()
                   .AddDefaultTokenProviders()
                   .AddDefaultUI()
                   .AddSignInManager<SignInManager<ApplicationUser>>()
                   .AddUserManager<UserManager<ApplicationUser>>()
                   .AddRoleManager<RoleManager<IdentityRole>>()
                   .AddEntityFrameworkStores<DataContext>();

            var facebookAppId = builder.Configuration.GetSection("Facebook:AppId").Get<string>() ?? "";
            var facebookAppSecret = builder.Configuration.GetSection("Facebook:AppSecret").Get<string>() ?? "";
            var googleClientId = builder.Configuration.GetSection("Google:ClientId").Get<string>() ?? "";
            var googleClientSecret = builder.Configuration.GetSection("Google:ClientSecret").Get<string>() ?? "";


            builder.Services.AddAuthentication()
                        .AddFacebook(options =>
                        {
                            options.AppId = facebookAppId;
                            options.AppSecret = facebookAppSecret;
                        })
                        .AddGoogle(options =>
                        {
                            options.ClientId = googleClientId;
                            options.ClientSecret = googleClientSecret;
                        });

            builder.Services.AddHangfire(options =>
            {
                options.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDistributedMemoryCache(); 
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30); 
            });

            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.ConsentCookieValue = "true";
            });
            builder.Services.AddApplication();

            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 10 * 1024 * 1024; 
            });
            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            builder.Services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
                options.HttpsPort = 5001;
            });

            if (!builder.Environment.IsDevelopment())
            {
                builder.Services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
                    options.HttpsPort = 443;
                });
            }

            var app = builder.Build();

            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new Hangfire.Dashboard.BasicAuthorization.BasicAuthAuthorizationFilter
                                            (new Hangfire.Dashboard.BasicAuthorization.BasicAuthAuthorizationFilterOptions
                {
                      RequireSsl = false,
                         SslRedirect = false,
                         LoginCaseSensitive = true,
                     Users = new []
                     {
                           new Hangfire.Dashboard.BasicAuthorization.BasicAuthAuthorizationUser
                           {
                              Login = builder.Configuration.GetSection("HangfireSettings:Username").Get<string>(),
                              PasswordClear =  builder.Configuration.GetSection("HangfireSettings:Password").Get<string>()
                           }
                        }
                    })
                }
            });
            app.UseSession();

            app.MapControllerRoute(
                name: "admin",
                pattern: "{area:exists}/{controller=Admin}/{action=Statistics}/{id?}");
            app.MapControllerRoute(
                    name: "index",
                    pattern: "{controller=Home}/{action=MainPage}/{id?}");
            app.MapControllerRoute(
              name: "librarian",
              pattern: "{area:exists}/{controller=Librarian}/{action=AllBooksInformation}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}