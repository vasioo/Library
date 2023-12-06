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

            // Add services to the container.
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
            builder.Services.AddApplication();//DI

            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB limit
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
            // Configure the HTTP request pipeline.
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


            app.MapControllerRoute(
                name: "admin",
                pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

            app.MapControllerRoute(
                    name: "index",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}